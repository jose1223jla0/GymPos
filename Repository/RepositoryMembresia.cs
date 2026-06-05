using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryMembresia
{
    Task<IEnumerable<Membresia>> GetAllAsync();
    Task<Membresia?> GetById(int id);
    Task UpdateAsync(Membresia membresia);
    Task CrearAsync(Membresia membresia);
    Task<int> CountMembresiaAsync();
}
public class RepositoryMembresia : IRepositoryMembresia
{
    private readonly GymPosContext _context;
    public RepositoryMembresia(GymPosContext contex)
    {
        _context = contex;
    }
    public async Task<IEnumerable<Membresia>> GetAllAsync()
        => await _context.Membresias.ToListAsync();

    public async Task<Membresia?> GetById(int id)
        => await _context.Membresias.FindAsync(id);

    public async Task UpdateAsync(Membresia membresia)
    {
        var existing = await _context.Membresias.FindAsync(membresia.IdMembresia)
            ?? throw new System.InvalidOperationException($"Membresía {membresia.IdMembresia} no encontrada.");
        existing.Nombre = membresia.Nombre;
        existing.Sesiones = membresia.Sesiones;
        existing.Precio = membresia.Precio;
        await _context.SaveChangesAsync();
    }

    public async Task CrearAsync(Membresia membresia)
    {
        var nueva = new Membresia
        {
            Nombre = membresia.Nombre,
            Sesiones = membresia.Sesiones,
            Precio = membresia.Precio
        };
        _context.Membresias.Add(nueva);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountMembresiaAsync()
    {
        return await _context.Membresias.CountAsync();
    }
}
