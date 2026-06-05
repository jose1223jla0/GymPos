using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using GymPos.Services;

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
    private readonly IAuthService _authService;
    private readonly ICajaEventService _cajaEventService;

    public ServiceCaja(GymPosContext context, IAuthService authService, ICajaEventService cajaEventService)
    {
        _context = context;
        _authService = authService;
        _cajaEventService = cajaEventService;
    }

    public async Task<Caja> AbrirCajaAsync(decimal montoInicial)
    {
        var existeCajaAbierta = await _context.Cajas.AnyAsync(c => c.Abierta);
        if (existeCajaAbierta)
        {
            throw new InvalidOperationException("Ya hay una caja abierta.");
        }
        var usuario = _authService.UsuarioActual;
        if (usuario == null)
        {
            throw new InvalidOperationException("No hay un usuario autenticado.");
        }

        var caja = new Caja(montoInicial)
        {
            IdUsuario = usuario.IdUsuario
        };
        _context.Cajas.Add(caja);
        await _context.SaveChangesAsync();
        try
        {
            _cajaEventService?.NotificarCajaAperturada();
        }
        catch { }
        return caja;
    }
    public async Task<Caja> CerrarCajaAsync()
    {
        var caja = await ObtenerCajaAbiertaAsync();
        caja.Cerrar();
        await _context.SaveChangesAsync();
        try
        {
            _cajaEventService?.NotificarCajaCerrada();
        }
        catch { }
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

        // Notificar que hubo un movimiento
        try
        {
            _cajaEventService?.NotificarMovimiento();
        }
        catch { }

        return movimiento;
    }
}
