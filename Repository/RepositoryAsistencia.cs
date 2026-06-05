using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryAsistencia
{
    Task<IEnumerable<Asistencia>> GetAllAsistenciaAsync();
    Task<int> ContarAsistenciasAsync(int idSuscripcion);
    Task<bool> ExisteAsistenciaHoyAsync(int idSuscripcion);
    Task AddAsistenciaAsync(Asistencia asistencia);
    Task<int> ContarAsistenciasHoyAsync();
}

public class RepositoryAsistencia : IRepositoryAsistencia
{
    private readonly GymPosContext _context;
    public RepositoryAsistencia(GymPosContext context)
    {
        _context = context;
    }
    public async Task<int> ContarAsistenciasAsync(int idSuscripcion)
    {
        var conteo = await _context.Asistencias.CountAsync(a => a.IdSuscripcion == idSuscripcion);
        return conteo;

    }

    public async Task<bool> ExisteAsistenciaHoyAsync(int idSuscripcion)
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        var existeAsistencia = await _context.Asistencias.AnyAsync(a => a.IdSuscripcion == idSuscripcion && a.Fecha == hoy);
        return existeAsistencia;
    }

    public async Task<IEnumerable<Asistencia>> GetAllAsistenciaAsync()
    {
        var listaAsistencia = await _context.Asistencias.Include(a => a.Suscripcion).ToListAsync();
        return listaAsistencia;
    }

    public async Task AddAsistenciaAsync(Asistencia asistencia)
    {
        _context.Asistencias.Add(asistencia);
        await _context.SaveChangesAsync();
    }
    public async Task<int> ContarAsistenciasHoyAsync()
    {
        var hoy = DateOnly.FromDateTime(DateTime.Now);
        return await _context.Asistencias.CountAsync(a => a.Fecha == hoy);
    }
}
