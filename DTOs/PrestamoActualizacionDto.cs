using System.ComponentModel.DataAnnotations;

public class PrestamoActualizacionDto
{
    [Required]
    public DateTime FechaDevolucion { get; set; }
}