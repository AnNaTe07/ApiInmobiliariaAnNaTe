using Microsoft.EntityFrameworkCore;

using ApiInmobiliariaAnNaTe.Models;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Propietario> Propietarios { get; set; }
    public DbSet<Inquilino> Inquilinos { get; set; } = null;
    public DbSet<Inmueble> Inmuebles { get; set; } = null;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Inmueble>(entity =>
        {
            entity.Property(i => i.Latitud)
                .HasColumnType("decimal(12,7)");

            entity.Property(i => i.Longitud)
                .HasColumnType("decimal(12,7)");

            entity.Property(i => i.Superficie)
                .HasColumnType("decimal(6,1)");

            entity.Property(i => i.Precio)
                .HasColumnType("decimal(9,1)");
        });

        modelBuilder.Entity<Contrato>(entity =>
        {
            entity.Property(c => c.Monto)
                .HasColumnType("decimal(9,1)");

            entity.Property(c => c.Desde)
                .HasColumnType("date");

            entity.Property(c => c.Hasta)
                .HasColumnType("date");
        });
        modelBuilder.Entity<Pago>(entity =>
        {
            entity.Property(p => p.Monto)
                .HasColumnType("decimal(9,1)");

            entity.Property(p => p.Fecha)
                .HasColumnType("date");
        });
        modelBuilder.Entity<UsoInmueble>()
      .HasKey(u => u.Id);

        modelBuilder.Entity<Inmueble>()
            .HasOne(i => i.UsoInmueble)
            .WithMany()
            .HasForeignKey(i => i.UsoInmuebleId);
    }
    public DbSet<Contrato> Contratos { get; set; } = null;
    public DbSet<Pago> Pagos { get; set; } = null;
    public DbSet<Tipo> Tipos { get; set; }
    public DbSet<UsoInmueble> UsoInmuebles { get; set; } = null;

    /* dotnet ef migrations add InitialCreate
 dotnet ef database update
 dotnet ef migrations add InitialCreate: Esto crea una migración que describe los cambios en tu modelo desde la última migración(o la creación inicial).
 dotnet ef database update: Esto aplica la migración a la base de datos.Si la base de datos no existe, se creará automáticamente, y se crearán las tablas basadas en tus modelos.*/
}