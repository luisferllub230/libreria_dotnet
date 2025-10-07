using System.ComponentModel.DataAnnotations;

public class AutorDto
{
    [Required]
    public int AutorId { get; set; }
    public string Nombre { get; set; } = string.Empty; 
    public string Nacionalidad { get; set; } = string.Empty;
}