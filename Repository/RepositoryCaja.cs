using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryCaja
{
    Task<Caja> AbrirCajaAsync(decimal montoInicial);
    Task<Caja> CerrarCajaAsync();
    Task<Caja> ObtenerCajaAbiertaAsync();
    Task<ResumenCaja> ObtenerResumenAsync();
}
public class RepositoryCaja : IRepositoryCaja
{
    private readonly GymPosContext _context;

    public RepositoryCaja(GymPosContext context)
    {
        _context = context;
    }

    public async Task<Caja> AbrirCajaAsync(decimal montoInicial)
    {
        var cajaYaAbierta = await _context.Cajas.AnyAsync(c => c.Abierta);
        if (cajaYaAbierta)
        {
            throw new InvalidOperationException("Ya hay una caja abierta.");
        }
        var caja = new Caja(montoInicial);
        _context.Cajas.Add(caja);
        await _context.SaveChangesAsync();
        return caja;
    }

    public async Task<Caja> CerrarCajaAsync()
    {
        var caja = await ObtenerCajaAbiertaAsync();
        caja.Cerrar();
        await _context.SaveChangesAsync();
        return caja;
    }

    public async Task<Caja> ObtenerCajaAbiertaAsync()
    {
        var caja = await _context.Cajas.Include(c => c.Movimientos).FirstOrDefaultAsync(c => c.Abierta);
        if (caja == null)
        {
            throw new InvalidOperationException("No hay una caja abierta.");
        }
        return caja;
    }

    public async Task<ResumenCaja> ObtenerResumenAsync()
    {
        var caja = await ObtenerCajaAbiertaAsync();
        return new ResumenCaja
        {
            FechaApertura = caja.FechaApertura,
            MontoInicial = caja.MontoInicial,
            TotalIngresos = caja.Movimientos.Where(m => m.EsIngreso()).Sum(m => m.Monto),
            TotalEgresos = caja.Movimientos.Where(m => m.EsEgreso()).Sum(m => m.Monto),
            SaldoActual = caja.CalcularSaldo(),
            CantMovimientos = caja.Movimientos.Count
        };
    }
}
