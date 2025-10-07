using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Autor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AutorId { get; set; }
    [Required]
    public string Nombre { get; set; } = string.Empty;
    [Required]
    public string Nacionalidad { get; set; } = string.Empty;

    // Relación
    public ICollection<Libro> Libros { get; set; } = new List<Libro>();
}