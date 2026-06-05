using GymPos.Models;
using GymPos.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IServiceVenta
{
    Task<Venta> CrearVentaAsync(Venta venta, int idCaja);
}

public class ServiceVenta : IServiceVenta
{
    private readonly IRepositoryVenta _repoVenta;
    private readonly IRepositoryProducto _repoProducto;
    private readonly IRepositoryCaja _repoCaja;
    private readonly IRepositoryMovimientoCaja _repoMovimiento;
    private readonly INotificationService _notificationService;

    public ServiceVenta(IRepositoryVenta repoVenta, IRepositoryProducto repoProducto, IRepositoryCaja repoCaja, IRepositoryMovimientoCaja repoMovimiento, INotificationService notificationService)
    {
        _repoVenta = repoVenta;
        _repoProducto = repoProducto;
        _repoCaja = repoCaja;
        _repoMovimiento = repoMovimiento;
        _notificationService = notificationService;
    }

    public async Task<Venta> CrearVentaAsync(Venta venta, int idCaja)
    {
        Caja caja;
        try
        {
            caja = await _repoCaja.ObtenerCajaAbiertaAsync();
        }
        catch (InvalidOperationException)
        {
            _notificationService?.ShowError("Caja cerrada", "No hay una caja abierta. Por favor, aperture la caja antes de continuar.");
            throw new InvalidOperationException("Caja cerrada. Por favor aperture la caja.");
        }
        if (idCaja == 0 || caja.IdCaja != idCaja)
        {
            idCaja = caja.IdCaja;
        }

        foreach (var detalle in venta.VentaDetalles)
        {
            var producto = await _repoProducto.ObtenerPorIdAsync(detalle.IdProducto)
                ?? throw new InvalidOperationException($"Producto con id {detalle.IdProducto} no encontrado.");

            if (producto.StockProducto < detalle.Cantidad)
            {
                throw new InvalidOperationException( $"Stock insuficiente para '{producto.NombreProducto}'. " +  $"Disponible: {producto.StockProducto}, solicitado: {detalle.Cantidad}.");
            }
        }
        var ventaCreada = await _repoVenta.CrearVentaAsync(venta);
        foreach (var detalle in ventaCreada.VentaDetalles)
        {
            await _repoProducto.ActualizarStockAsync(detalle.IdProducto, detalle.Cantidad);
        }

        var productos = string.Join(", ", ventaCreada.VentaDetalles.Select(d => d.Producto?.NombreProducto ?? $"Producto #{d.IdProducto}"));

        var movimiento = MovimientoCaja.CrearIngresoVenta(
            idCaja: idCaja,
            monto: ventaCreada.Total,
            concepto: $"Venta - {productos}", 
            idVenta: ventaCreada.IdVenta
        );
        await _repoMovimiento.AddAsync(movimiento);
        await _repoMovimiento.SaveChangesAsync();
        return ventaCreada;
    }
}