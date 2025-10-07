using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly BibliotecaContext _context;

        public LibrosController(BibliotecaContext context)
        {
            _context = context;
        }

        [HttpGet("libros/antes-de-2000")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLibrosAntesDe2000()
        {
            try
            {
                var libros = await _context.Libros
                    .Where(l => l.AnioPublicacion < 2000)
                    .Select(l => new {
                        l.LibroId,
                        l.Titulo,
                        l.AnioPublicacion
                    })
                    .ToListAsync();

                if (!libros.Any())
                {
                    return NotFound("No se encontraron libros antes del año 2000.");
                }

                return Ok(libros);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al recuperar libros: {ex.Message}");
            }
        }

        [HttpPost("libros")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLibro([FromBody] LibroCreacionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var libro = new Libro
            {
                Titulo = dto.Titulo,
                AnioPublicacion = dto.AnioPublicacion,
                AutorId = dto.AutorId
            };

            if (libro.AutorId != null)
            {
                var autor = await _context.Autores.FindAsync(libro.AutorId);
                if (autor == null)
                {
                    return BadRequest("El autor especificado no existe.");
                }
            }

            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLibro), new { id = libro.LibroId }, libro);
        }
    }
}