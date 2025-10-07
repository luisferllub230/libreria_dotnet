using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Libro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LibroId { get; set; }
    [Required]
    public string Titulo { get; set; } = string.Empty;
    [Required]
    public int AnioPublicacion { get; set; }
    public string Genero { get; set; } = string.Empty;

    // Clave Foránea
    public int AutorId { get; set; }
    [ForeignKey("AutorId")]
    public Autor Autor { get; set; } = null!; // Relación

    // Relación
    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}