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
}
public class RepositoryMembresia : IRepositoryMembresia
{
    private readonly GymPosContext _context;
    public RepositoryMembresia(GymPosContext contex)
    {
        _context = contex;
    }
    public async Task<IEnumerable<Membresia>> GetAllAsync()
    {
        var listMembresias = await _context.Membresias.ToListAsync();
        return listMembresias;
    }

    public async Task<Membresia?> GetById(int id)
    {
        return await _context.Membresias.FindAsync(id);
    }

    public async Task UpdateAsync(Membresia membresia)
    {
        var nuevaMembresia = new Membresia
        {
            IdMembresia = membresia.IdMembresia,
            Nombre = membresia.Nombre,
            Sesiones = membresia.Sesiones
        };
        _context.Membresias.Update(nuevaMembresia);
        await _context.SaveChangesAsync();
    }
}
