using System.ComponentModel.DataAnnotations;

public class PrestamoNoDevueltosDto
{   
    [Required]
    public int PrestamoId { get; set; }
    [Required]
    public DateTime FechaPrestamo { get; set; }
    [Required]
    public LibroDto? Libro { get; set; }
}