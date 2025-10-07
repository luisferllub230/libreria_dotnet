using System.ComponentModel.DataAnnotations;

public class NewPrestamoDto
{   
    [Required]
    public int PrestamoId { get; set; }
    [Required]
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; }
    [Required]
    public int LibroId { get; set; }

    public LibroDto? Libro { get; set; }
}