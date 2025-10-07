using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PrestamosController : ControllerBase
{
    private readonly BibliotecaContext _context;

    public PrestamosController(BibliotecaContext context)
    {
        _context = context;
    }

    [HttpGet("prestamos/no-devueltos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPrestamosNoDevueltos()
    {
        var resultados = await _context.Prestamos
            .Where(p => p.FechaDevolucion == null)
            .Include(p => p.Libro)
                .ThenInclude(l => l.Autor)
            .Select(p => new
            {
                p.Libro.Autor.AutorId,
                p.Libro.Autor.Nombre,
                p.LibroId,
                p.Libro.Titulo
            })
            .ToListAsync();

        if (!resultados.Any())
        {
            return NotFound("No hay préstamos pendientes de devolución.");
        }

        return Ok(resultados);
    }

    [HttpPut("prestamos/{id}")]
    [Authorize(Roles = "admin,usuario_regular")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PutPrestamo(int id, [FromBody] PrestamoActualizacionDto dto)
    {
        if (dto.FechaDevolucion > DateTime.Now)
        {
            return BadRequest("La fecha de devolución no puede ser futura.");
        }

        var prestamo = await _context.Prestamos.FindAsync(id);

        if (prestamo == null)
        {
            return NotFound($"Préstamo con ID {id} no encontrado.");
        }

        prestamo.FechaDevolucion = dto.FechaDevolucion.Date;
        
        _context.Entry(prestamo).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!_context.Prestamos.Any(e => e.PrestamoId == id))
        {
            return NotFound($"Préstamo con ID {id} no encontrado después de la actualización.");
        }

        return NoContent();
    }

    [HttpDelete("prestamos/{id}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePrestamo(int id)
    {
        var prestamo = await _context.Prestamos.FindAsync(id);

        if (prestamo == null)
        {
            return NotFound(new { Message = $"Préstamo con ID {id} no existe." });
        }

        _context.Prestamos.Remove(prestamo);
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"Préstamo con ID {id} eliminado correctamente." });
    }
}