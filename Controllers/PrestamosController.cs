using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BibliotecaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                .Select(p => new NewPrestamoDto
                {
                    PrestamoId = p.PrestamoId,
                    FechaPrestamo = p.FechaPrestamo,
                    FechaDevolucion = p.FechaDevolucion,
                    LibroId = p.LibroId,
                    Libro = new LibroDto
                    {
                        LibroId = p.Libro.LibroId,
                        Titulo = p.Libro.Titulo,
                        AutorId = p.Libro.AutorId,
                        Autor = new AutorDto
                        {
                            AutorId = p.Libro.Autor.AutorId,
                            Nombre = p.Libro.Autor.Nombre,
                            Nacionalidad = p.Libro.Autor.Nacionalidad
                        }
                    }
                    
                })
                .ToListAsync();

            if (!resultados.Any())
            {
                return NotFound("No hay préstamos pendientes de devolución.");
            }

            return Ok(resultados);
        }

        [HttpPost("prestamos")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostPrestamo([FromBody] NewPrestamoDto dto)
        {
            if (dto.FechaDevolucion > DateTime.Now)
            {
                return BadRequest("La fecha de devolución no puede ser futura.");
            }

            var libro = await _context.Libros.FindAsync(dto.LibroId);
            if (libro == null)
            {
                return BadRequest("El libro especificado no existe.");
            }

            var prestamo = new Prestamo
            {
                FechaPrestamo = DateTime.UtcNow,
                FechaDevolucion = dto.FechaDevolucion.HasValue ? dto.FechaDevolucion.Value.ToUniversalTime() : null,
                LibroId = dto.LibroId
            };

            _context.Prestamos.Add(prestamo);
            await _context.SaveChangesAsync();

            var newPrestamoDto = new NewPrestamoDto
            {
                PrestamoId = prestamo.PrestamoId,
                FechaPrestamo = prestamo.FechaPrestamo,
                FechaDevolucion = prestamo.FechaDevolucion,
                LibroId = prestamo.LibroId
            };

            if (prestamo == null)
            {
                return BadRequest("No se pudo crear el préstamo.");
                
            }

            return CreatedAtAction(nameof(GetPrestamo), new { id = prestamo.PrestamoId }, newPrestamoDto);
        }
        
        [HttpGet("prestamos/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPrestamo(int id)
        {
            var prestamo = await _context.Prestamos.FindAsync(id);

            if (prestamo == null)
            {
                return NotFound($"Préstamo con ID {id} no encontrado.");
            }

            return Ok(prestamo);
        }

        [HttpPut("prestamos/{id}")]
        [Authorize]
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

            prestamo.FechaDevolucion = dto.FechaDevolucion.ToUniversalTime();
            
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
        [Authorize]
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
}