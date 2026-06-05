using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryProducto
{
    Task<IEnumerable<Producto>> ObtenerActivosAsync();
    Task<IEnumerable<Producto>> ObtenerPorCategoriaAsync(int idCategoria);
    Task<IEnumerable<Producto>> ObtenerPorNombreAsync(string termino, int? idCategoria = null);
    Task ActualizarStockAsync(int idProducto, int cantidad);
    Task<Producto?> ObtenerPorIdAsync(int id);
    Task CrearNuevoProducto(Producto producto);
    Task EditarProducto(Producto producto);

}
public class RepositoryProducto : IRepositoryProducto
{
    private readonly GymPosContext _context;
    public RepositoryProducto(GymPosContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Producto>> ObtenerActivosAsync()
    {
        var obtenerProductos= await _context.Productos.Where(p => p.EstadoProducto)
                                     .Include(p=>p.Categoria)
                                     .OrderBy(p => p.NombreProducto)
                                     .ToListAsync();
        return obtenerProductos;
    }

    public async Task<IEnumerable<Producto>>ObtenerPorCategoriaAsync(int idCategoria)
    {
        var obtenerProductos = await _context.Productos.Where(p => p.EstadoProducto && p.IdCategoria == idCategoria)
                                     .Include(p => p.DetallesVenta)
                                     .OrderBy(p => p.NombreProducto)
                                     .ToListAsync();
        return obtenerProductos;
    }

    public async Task<IEnumerable<Producto>> ObtenerPorNombreAsync(string termino, int? idCategoria = null)
    {
        var query = _context.Productos.AsQueryable();

        if (idCategoria.HasValue && idCategoria.Value != 0)
            query = query.Where(p => p.IdCategoria == idCategoria.Value);

        if (!string.IsNullOrWhiteSpace(termino))
        {
            var t = termino.Trim().ToLower();
            query = query.Where(p => p.EstadoProducto && p.NombreProducto.ToLower().Contains(t));
        }
        else
        {
            query = query.Where(p => p.EstadoProducto);
        }

        var obtenerProductos = await query.Include(p => p.Categoria)
                                          .OrderBy(p => p.NombreProducto)
                                          .ToListAsync();
        return obtenerProductos;
    }

    public async Task<Producto?> ObtenerPorIdAsync(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        return producto;
    }

    public async Task ActualizarStockAsync(int idProducto, int cantidad)
    {
        var producto = await _context.Productos.FindAsync(idProducto);
        if (producto == null)
        {
            throw new InvalidOperationException($"Producto {idProducto} no encontrado.");
        }
        producto.StockProducto -= cantidad;
        await _context.SaveChangesAsync();
    }
    public async Task CrearNuevoProducto(Producto producto)
    {
       var nuevoProducto = new Producto
       {
           IdCategoria = producto.IdCategoria,
           NombreProducto = producto.NombreProducto,
           StockProducto = producto.StockProducto,
           PrecioProducto = producto.PrecioProducto,
           EstadoProducto = true
       };
        _context.Productos.Add(nuevoProducto);
        await _context.SaveChangesAsync();
    }
    public async Task EditarProducto(Producto producto)
    {
        var productoExistente = await _context.Productos.FindAsync(producto.IdProducto);
        if (productoExistente == null)
        {
            throw new InvalidOperationException($"Producto {producto.IdProducto} no encontrado.");
        }
        productoExistente.IdCategoria = producto.IdCategoria;
        productoExistente.NombreProducto = producto.NombreProducto;
        productoExistente.StockProducto = producto.StockProducto;
        productoExistente.PrecioProducto = producto.PrecioProducto;
        productoExistente.EstadoProducto = producto.EstadoProducto;
        await _context.SaveChangesAsync();
    }
}
