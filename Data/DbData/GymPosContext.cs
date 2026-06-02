using GymPos.Models;
using Microsoft.EntityFrameworkCore;

namespace GymPos.Data.DbData;

public class GymPosContext : DbContext
{
    public GymPosContext()
    {
    }

    public GymPosContext(DbContextOptions<GymPosContext> options)
        : base(options)
    {
    }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Membresia> Membresias { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Suscripcion> Suscripciones { get; set; }
    public DbSet<Producto> Productos{ get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<Caja> Cajas { get; set; }
    public DbSet<DetalleVenta> DetalleVentas { get; set; }
    public DbSet<Pago> Pagos { get; set; }
    public DbSet<MovimientoCaja> MovimientosCaja{ get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configurar índices para optimizar consultas
        modelBuilder.Entity<Cliente>().HasIndex(c => c.Dni).IsUnique();
        modelBuilder.Entity<Cliente>().HasIndex(c => c.IdCliente);
        // Desactivar lazy loading automático para relaciones
        modelBuilder.Entity<Cliente>().Navigation(c => c.Suscripciones) .AutoInclude(false);
    }
}
