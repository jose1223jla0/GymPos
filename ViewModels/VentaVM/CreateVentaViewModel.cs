using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.VentaVM;

public partial class CreateVentaViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Producto> Productos { get; } = new();

    private readonly IRepositoryProducto _repoProducto;
    private readonly IServiceVenta _serviceVenta;

    private Producto? _selectedProducto;
    private int _cantidad = 1;
    private string _errorMessage = string.Empty;

    public Producto? SelectedProducto
    {
        get => _selectedProducto;
        set { _selectedProducto = value; OnPropertyChanged(); }
    }

    public int Cantidad
    {
        get => _cantidad;
        set { _cantidad = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public event Action? VentaCreada;

    public CreateVentaViewModel(IRepositoryProducto repoProducto, IServiceVenta serviceVenta)
    {
        _repoProducto = repoProducto;
        _serviceVenta = serviceVenta;
    }

    public async Task InitAsync()
    {
        await LoadProductosAsync();
    }

    private async Task LoadProductosAsync()
    {
        Productos.Clear();
        var list = await _repoProducto.ObtenerActivosAsync();
        foreach (var p in list)
            Productos.Add(p);
    }

    [RelayCommand]
    private async Task CrearVenta()
    {
        if (SelectedProducto == null)
        {
            ErrorMessage = "Selecciona un producto.";
            return;
        }
        if (Cantidad <= 0)
        {
            ErrorMessage = "La cantidad debe ser mayor a cero.";
            return;
        }

        try
        {
            var venta = new Venta
            {
                FechaVenta = DateOnly.FromDateTime(DateTime.Now),
                Total = SelectedProducto.PrecioProducto * Cantidad
            };
            venta.VentaDetalles.Add(new DetalleVenta
            {
                IdProducto = SelectedProducto.IdProducto,
                Cantidad = Cantidad,
                PrecioUnitario = SelectedProducto.PrecioProducto
            });

            await _serviceVenta.CrearVentaAsync(venta, idCaja: 0);
            VentaCreada?.Invoke();
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
