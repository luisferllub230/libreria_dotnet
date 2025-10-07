using System.ComponentModel.DataAnnotations;

public class LibroDto
{
    public int LibroId { get; set; }
    public string Titulo { get; set; } = string.Empty; 
    public string Genero { get; set; } = string.Empty;
    public int AutorId { get; set; } 
    public AutorDto? Autor { get; set; }
}