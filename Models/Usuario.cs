using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{
    [Key]
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? login { get; set; }
    public string? Password { get; set; }
}