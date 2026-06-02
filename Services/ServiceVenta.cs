using GymPos.Models;
using GymPos.Repository;
using System;
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

    public ServiceVenta(IRepositoryVenta repoVenta, IRepositoryProducto repoProducto, IRepositoryCaja repoCaja, IRepositoryMovimientoCaja repoMovimiento)
    {
        _repoVenta = repoVenta;
        _repoProducto = repoProducto;
        _repoCaja = repoCaja;
        _repoMovimiento = repoMovimiento;
    }

    public async Task<Venta> CrearVentaAsync(Venta venta, int idCaja)
    {
        // 1. Obtener la caja abierta. Si no coincide el id pasado, usar la caja abierta.
        var caja = await _repoCaja.ObtenerCajaAbiertaAsync();
        if (idCaja == 0 || caja.IdCaja != idCaja)
        {
            // Usar la caja abierta en lugar del id pasado
            idCaja = caja.IdCaja;
        }
        foreach (var detalle in venta.VentaDetalles)
        {
            var producto = await _repoProducto.ObtenerPorIdAsync(detalle.IdProducto) ?? throw new InvalidOperationException($"Producto con id {detalle.IdProducto} no encontrado.");

            if (producto.StockProducto < detalle.Cantidad)
            {
                throw new InvalidOperationException($"Stock insuficiente para '{producto.NombreProducto}'. " + $"Disponible: {producto.StockProducto}, solicitado: {detalle.Cantidad}.");
            }

        }
        var ventaCreada = await _repoVenta.CrearVentaAsync(venta);
        foreach (var detalle in ventaCreada.VentaDetalles)
        {
            await _repoProducto.ActualizarStockAsync(detalle.IdProducto, detalle.Cantidad);
        }
        var movimiento = MovimientoCaja.CrearIngresoVenta(idCaja: idCaja, monto: ventaCreada.Total, concepto: $"Venta #{ventaCreada.IdVenta}", idVenta: ventaCreada.IdVenta);
        await _repoMovimiento.AddAsync(movimiento);
        await _repoMovimiento.SaveChangesAsync();
        return ventaCreada;
    }
}

