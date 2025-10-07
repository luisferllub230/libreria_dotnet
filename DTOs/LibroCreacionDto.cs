using System.ComponentModel.DataAnnotations;

public class LibroCreacionDto
{
    [Required]
    public string Titulo { get; set; } = string.Empty;
    [Required]
    public int AutorId { get; set; }
    [Required]
    [Range(1000, 2024)]
    public int AnioPublicacion { get; set; }
    public string Genero { get; set; } = string.Empty;
}