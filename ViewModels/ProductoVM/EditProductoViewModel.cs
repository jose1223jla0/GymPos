using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ProductoVM;

public partial class EditProductoViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Producto> ListProductos { get; } = new();
    private readonly IRepositoryProducto _repositoryProducto;
    private int _idProducto;
    private int _idCategoria;
    private string _nombreProducto = string.Empty;
    private int _stockProducto;
    private decimal _precioProducto;
    private bool _estadoProducto;
    public int IdProducto
    {
        get => _idProducto;
        set
        {
            if (_idProducto != value)
            {
                _idProducto = value;
                OnPropertyChanged(nameof(IdProducto));
            }
        }
    }
    public int IdCategoria
    {
        get => _idCategoria;
        set
        {
            if (_idCategoria != value)
            {
                _idCategoria = value;
                OnPropertyChanged(nameof(IdCategoria));
            }
        }
    }
    public string NombreProducto
    {
        get => _nombreProducto;
        set
        {
            if (_nombreProducto != value)
            {
                _nombreProducto = value;
                OnPropertyChanged(nameof(NombreProducto));
            }
        }
    }
    public int StockProducto
    {
        get => _stockProducto;
        set
        {
            if (_stockProducto != value)
            {
                _stockProducto = value;
                OnPropertyChanged(nameof(StockProducto));
            }
        }
    }
    public decimal PrecioProducto
    {
        get => _precioProducto;
        set
        {
            if (_precioProducto != value)
            {
                _precioProducto = value;
                OnPropertyChanged(nameof(PrecioProducto));
            }
        }
    }
    public bool EstadoProducto
    {
        get => _estadoProducto;
        set
        {
            if (_estadoProducto != value)
            {
                _estadoProducto = value;
                OnPropertyChanged(nameof(EstadoProducto));
            }
        }
    }
    public EditProductoViewModel(IRepositoryProducto repositoryProducto)
    {
        _repositoryProducto = repositoryProducto;
    }

    [RelayCommand]
    public async Task SaveChangesProducto()
    {
        try
        {
            var producto = new Producto
            {
                IdProducto = IdProducto,
                IdCategoria = IdCategoria,
                NombreProducto = NombreProducto,
                StockProducto = StockProducto,
                PrecioProducto = PrecioProducto,
                EstadoProducto = EstadoProducto
            };
            //await _repositoryProducto.UpdateProducto(producto);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al guardar los cambios del producto: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
