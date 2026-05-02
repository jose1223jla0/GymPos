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
}
