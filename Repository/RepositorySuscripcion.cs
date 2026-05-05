using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositorySuscripcion
{
    Task<IEnumerable<Suscripcion>> GetAllSuscripcion();
    Task<Suscripcion?> GetByIdAsync(int id);
    Task AddSuscripcionAsync(Suscripcion suscripcion);
    Task<List<Suscripcion>> GetActivasByCliente(int clienteId);
    Task<List<Suscripcion>> GetSuscripcionesActivas();
}
public class RepositorySuscripcion : IRepositorySuscripcion
{
    private readonly GymPosContext _context;

    public RepositorySuscripcion(GymPosContext context)
    {
        _context = context;
    }

    public async Task AddSuscripcionAsync(Suscripcion suscripcion)
    {
        await _context.Suscripciones.AddAsync(suscripcion);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Suscripcion>> GetActivasByCliente(int clienteId)
    {
        return await _context.Suscripciones
            .Where(s => s.IdCliente == clienteId && s.EstadoSuscripcion == EstadoSuscripcion.Activa)
            .ToListAsync();
    }
    public async Task<IEnumerable<Suscripcion>> GetAllSuscripcion()
    {
        var listaSuscripcion = await _context.Suscripciones
                                    .Include(s => s.Membresia)
                                    .Include(s=>s.Cliente).ToListAsync();
        return listaSuscripcion;
    }

    public async Task<Suscripcion?> GetByIdAsync(int id)
    {
        var suscripcion = await _context.Suscripciones
                                    .Include(s => s.Membresia)
                                    .Include(s => s.Cliente)
                                    .FirstOrDefaultAsync(s => s.IdSuscripcion == id);
        return suscripcion;
    }

    public async Task<List<Suscripcion>> GetSuscripcionesActivas()
    {
        return await _context.Suscripciones
            .Include(s => s.Cliente)
            .Include(s => s.Membresia)
            .Where(s => s.EstadoSuscripcion == EstadoSuscripcion.Activa)
            .ToListAsync();
    }
}
