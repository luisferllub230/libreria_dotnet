using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Prestamo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PrestamoId { get; set; }
    [Required]
    public DateTime FechaPrestamo { get; set; }
    public DateTime? FechaDevolucion { get; set; } // Puede ser nulo

    // Clave Foránea
    public int LibroId { get; set; }
    [ForeignKey("LibroId")]
    public Libro Libro { get; set; } = null!; // Relación
}