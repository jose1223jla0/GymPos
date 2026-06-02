using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryMovimientoCaja
{
    Task<IEnumerable<MovimientoCaja>> GetAllAsync();
    Task<IEnumerable<MovimientoCaja>> GetByCajaAsync(int idCaja);
    Task<IEnumerable<MovimientoCaja>> GetByFechaAsync(int idCaja, DateOnly fecha);
    Task<MovimientoCaja?> GetByIdAsync(int idMovimiento);
    Task AddAsync(MovimientoCaja movimiento);
    Task SaveChangesAsync();
}
public class RepositoryMovimientoCaja : IRepositoryMovimientoCaja
{
    private readonly GymPosContext _context;
    public RepositoryMovimientoCaja(GymPosContext context)
    {
        _context = context;
    }

    // ─── Todos los movimientos ─────────────────────────────────────────────────
    public async Task<IEnumerable<MovimientoCaja>> GetAllAsync()
    {
        var movimientos = await _context.MovimientosCaja
                                .Include(m => m.Caja)
                                .Include(m => m.Pago)
                                .Include(m => m.Venta)
                                .OrderByDescending(m => m.Fecha)
                                .ToListAsync();
        return movimientos;
    }


    // ─── Movimientos de una caja específica ───────────────────────────────────
    public async Task<IEnumerable<MovimientoCaja>> GetByCajaAsync(int idCaja)
    {
        var movimientoCaja = await _context.MovimientosCaja
                            .Include(m => m.Pago)
                            .Include(m => m.Venta)
                            .Where(m => m.IdCaja == idCaja)
                            .OrderByDescending(m => m.Fecha)
                            .ToListAsync();
        return movimientoCaja;
    }

    // ─── Movimientos de una caja en una fecha (útil para cierre de caja) ──────
    public async Task<IEnumerable<MovimientoCaja>> GetByFechaAsync(int idCaja, DateOnly fecha)
    {

        var movimientoPorFecha = await _context.MovimientosCaja
                                    .Include(m => m.Pago)
                                    .Include(m => m.Venta)
                                    .Where(m => m.IdCaja == idCaja && m.Fecha == fecha)
                                    .OrderByDescending(m => m.Fecha)
                                    .ToListAsync();
        return movimientoPorFecha;
    }

    // ─── Por ID ───────────────────────────────────────────────────────────────
    public async Task<MovimientoCaja?> GetByIdAsync(int idMovimiento)
    {
        var movimientoId = await _context.MovimientosCaja
                                .Include(m => m.Caja)
                                .Include(m => m.Pago)
                                .Include(m => m.Venta)
                                .FirstOrDefaultAsync(m => m.IdMovimientoCaja == idMovimiento);
        return movimientoId;
    }
    // ─── Persistencia ─────────────────────────────────────────────────────────
    public async Task AddAsync(MovimientoCaja movimiento)
    {
        await _context.MovimientosCaja.AddAsync(movimiento);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}
