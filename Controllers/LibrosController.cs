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
                    .Select(l => new LibroCreacionDto {
                        LibroId = l.LibroId,
                        Titulo = l.Titulo,
                        AnioPublicacion = l.AnioPublicacion,
                        AutorId = l.AutorId,
                        Genero = l.Genero
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLibro([FromBody] LibroCreacionDto dto)
        {
            
            var libro = new Libro
            {
                Titulo = dto.Titulo,
                AnioPublicacion = dto.AnioPublicacion,
                AutorId = dto.AutorId,
                Genero = dto.Genero
            };

            var autor = await _context.Autores.FindAsync(libro.AutorId);
            if (autor == null)
            {
                return BadRequest("El autor especificado no existe.");
            }

            libro.Autor = autor; 
            _context.Libros.Add(libro);
            await _context.SaveChangesAsync();

            var responseDto = new LibroCreacionDto
            {
                LibroId = libro.LibroId,
                Titulo = libro.Titulo,
                Genero = libro.Genero,
                AnioPublicacion = libro.AnioPublicacion,
            };
            
            return CreatedAtAction(nameof(GetLibro), new { id = libro.LibroId }, responseDto); 
        }

        [HttpGet("libros/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLibro(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                return NotFound($"Libro con ID {id} no encontrado.");
            }

            return Ok(libro);
        }
    }
}