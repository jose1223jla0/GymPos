using CommunityToolkit.Mvvm.ComponentModel;
using GymPos.Models;

namespace GymPos.ViewModels.ProductoVM;

public partial class ProductoViewModel : ObservableObject
{
    private readonly Producto _model;

    public int IdProducto => _model.IdProducto;
    public string NombreProducto => _model.NombreProducto;
    public decimal PrecioProducto => _model.PrecioProducto;
    public string PrecioFormateado => $"S/ {_model.PrecioProducto:F2}";

    // Stock es observable para que la UI refleje cambios tras una venta
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TieneStock))]
    private int _stockProducto;

    public bool TieneStock => StockProducto > 0;

    public ProductoViewModel(Producto model)
    {
        _model = model;
        _stockProducto = model.StockProducto;
    }
}
