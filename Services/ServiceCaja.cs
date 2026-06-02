using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IServiceCaja
{
    Task<Caja> AbrirCajaAsync(decimal montoInicial);
    Task<Caja> CerrarCajaAsync();
    Task<Caja> ObtenerCajaAbiertaAsync();
    Task<ResumenCaja> ObtenerResumenAsync();
    Task<MovimientoCaja> RegistrarIngresoAsync(decimal monto, string concepto, int? idPago = null);
}

public class ServiceCaja:IServiceCaja
{
    private readonly GymPosContext _context;
    public ServiceCaja(GymPosContext context)
    {
        _context = context;
    }

    public async Task<Caja> AbrirCajaAsync(decimal montoInicial)
    {
        var existeCajaAbierta = await _context.Cajas.AnyAsync(c => c.Abierta);
        if (existeCajaAbierta)
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
        var caja = await _context.Cajas
                        .Include(c => c.Movimientos)
                        .OrderByDescending(c => c.FechaApertura)
                        .FirstOrDefaultAsync(c => c.Abierta);
        if (caja == null)
        {
            throw new InvalidOperationException("No hay caja abierta.");
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

    public async Task<MovimientoCaja> RegistrarIngresoAsync(decimal monto, string concepto, int? idPago = null)
    {
        var caja = await ObtenerCajaAbiertaAsync(); 
        var movimiento = MovimientoCaja.CrearIngreso(caja.IdCaja, monto, concepto, idPago);
        _context.MovimientosCaja.Add(movimiento);
        await _context.SaveChangesAsync();

        return movimiento;
    }
}
