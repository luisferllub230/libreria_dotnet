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