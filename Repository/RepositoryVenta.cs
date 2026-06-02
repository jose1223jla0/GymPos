using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryVenta
{
    Task<Venta> CrearVentaAsync(Venta venta);
    Task<IEnumerable<Venta>> ObtenerTodasAsync();
    Task<Venta?> ObtenerPorIdAsync(int id);
}
public class RepositoryVenta : IRepositoryVenta
{
    private readonly GymPosContext _context;
    public RepositoryVenta(GymPosContext context)
    {
        _context = context;
    }
    public async Task<Venta> CrearVentaAsync(Venta venta)
    {
        _context.Ventas.Add(venta);
        await _context.SaveChangesAsync();
        return venta;
        //var ventaNueva = new Venta
        //{
        //    FechaVenta = venta.FechaVenta,
        //    Total = venta.Total,
        //    MovimientoCaja = venta.MovimientoCaja,
        //    VentaDetalles = venta.VentaDetalles.Select(d => new DetalleVenta
        //    {
        //        IdProducto = d.IdProducto,
        //        Cantidad = d.Cantidad,
        //        PrecioUnitario = d.PrecioUnitario
        //    }).ToList()
        //};
        //_context.Ventas.Add(ventaNueva);
        //return ventaNueva;
    }
    public async Task<IEnumerable<Venta>> ObtenerTodasAsync()
    {
        var ventas = await _context.Ventas.Include(v => v.VentaDetalles)
                                         .ThenInclude(d => d.Producto)
                                         .OrderByDescending(v => v.FechaVenta)
                                         .ToListAsync();
        return ventas;
    }
    public async Task<Venta?> ObtenerPorIdAsync(int id)
    {
        var ventas = _context.Ventas.Include(v => v.VentaDetalles)
                                 .ThenInclude(d => d.Producto)
                                 .FirstOrDefaultAsync(v => v.IdVenta == id);
        return await ventas;
    }

}
