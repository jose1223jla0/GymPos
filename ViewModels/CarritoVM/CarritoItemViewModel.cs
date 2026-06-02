using CommunityToolkit.Mvvm.ComponentModel;
using GymPos.Models;

namespace GymPos.ViewModels.CarritoVM;

public partial class CarritoItemViewModel : ObservableObject
{
    // ── Datos del producto ──────────────────────────────────────────────────
    public int IdProducto { get; }
    public string NombreProducto { get; }
    public decimal PrecioUnitario { get; }
    public int StockDisponible { get; }

    // ── Cantidad reactiva ───────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Subtotal))]
    [NotifyPropertyChangedFor(nameof(SubtotalFormateado))]
    private int _cantidad = 1;

    // ── Calculadas ──────────────────────────────────────────────────────────
    public decimal Subtotal => PrecioUnitario * Cantidad;
    public string SubtotalFormateado => Subtotal.ToString("F2");

    public CarritoItemViewModel(Producto producto)
    {
        IdProducto = producto.IdProducto;
        NombreProducto = producto.NombreProducto;
        PrecioUnitario = producto.PrecioProducto;
        StockDisponible = producto.StockProducto;
    }

    /// <summary>Mapea al modelo de persistencia DetalleVenta.</summary>
    public DetalleVenta ToDetalle() => new()
    {
        IdProducto = IdProducto,
        Cantidad = Cantidad,
        PrecioUnitario = PrecioUnitario,
    };
}