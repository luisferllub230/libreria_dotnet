using Microsoft.EntityFrameworkCore;

public class BibliotecaContext : DbContext
{
    public BibliotecaContext(DbContextOptions<BibliotecaContext> options)
        : base(options)
    {
    }

    public DbSet<Autor> Autores { get; set; } = null!;
    public DbSet<Libro> Libros { get; set; } = null!;
    public DbSet<Prestamo> Prestamos { get; set; } = null!;
}