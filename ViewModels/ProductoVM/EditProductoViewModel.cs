using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ProductoVM;

public partial class EditProductoViewModel : ObservableObject
{
    private readonly IRepositoryProducto _repositoryProducto;
    private readonly IRepositoryCategoria _repositoryCategoria;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Categoria> Categorias { get; } = new();

    [ObservableProperty] private int idProducto;
    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private int stockProducto;
    [ObservableProperty]
    private decimal precioProducto;

    public double PrecioProductoDouble
    {
        get => (double)PrecioProducto;
        set => PrecioProducto = (decimal)value;
    }

    partial void OnPrecioProductoChanged(decimal value)
    {
        OnPropertyChanged(nameof(PrecioProductoDouble));
    }
    [ObservableProperty] private bool estadoProducto = true;
    [ObservableProperty] private Categoria? categoriaSeleccionada;

    // true = editar, false = agregar
    [ObservableProperty] private bool isEditMode;

    public string TituloDialogo => IsEditMode ? "Editar Producto" : "Agregar Producto";

    public EditProductoViewModel(
        IRepositoryProducto repositoryProducto,
        IRepositoryCategoria repositoryCategoria,
        INotificationService notificationService)
    {
        _repositoryProducto = repositoryProducto;
        _repositoryCategoria = repositoryCategoria;
        _notificationService = notificationService;
    }

    public async Task LoadCategoriasAsync()
    {
        var cats = await _repositoryCategoria.ObtenerActivasAsync();
        Categorias.Clear();
        foreach (var c in cats)
            Categorias.Add(c);
    }

    /// <summary>Prepara el ViewModel para editar un producto existente.</summary>
    public async Task CargarParaEditar(Producto producto)
    {
        IsEditMode = true;
        await LoadCategoriasAsync();

        IdProducto = producto.IdProducto;
        NombreProducto = producto.NombreProducto;
        StockProducto = producto.StockProducto;
        PrecioProducto = producto.PrecioProducto;
        EstadoProducto = producto.EstadoProducto;
        CategoriaSeleccionada = producto.Categoria ?? FindCategoria(producto.IdCategoria);
    }

    /// <summary>Prepara el ViewModel para agregar un producto nuevo.</summary>
    public async Task CargarParaAgregar()
    {
        IsEditMode = false;
        await LoadCategoriasAsync();

        IdProducto = 0;
        NombreProducto = string.Empty;
        StockProducto = 0;
        PrecioProducto = 0;
        EstadoProducto = true;
        CategoriaSeleccionada = null;
    }

    private Categoria? FindCategoria(int id)
    {
        foreach (var c in Categorias)
            if (c.IdCategoria == id) return c;
        return null;
    }

    [RelayCommand]
    public async Task GuardarProducto()
    {
        if (string.IsNullOrWhiteSpace(NombreProducto))
        {
            _notificationService.ShowWarning("Validación", "El nombre del producto es obligatorio.");
            return;
        }
        if (CategoriaSeleccionada == null)
        {
            _notificationService.ShowWarning("Validación", "Selecciona una categoría.");
            return;
        }
        if (PrecioProducto <= 0)
        {
            _notificationService.ShowWarning("Validación", "El precio debe ser mayor a 0.");
            return;
        }

        try
        {
            var producto = new Producto
            {
                IdProducto = IdProducto,
                IdCategoria = CategoriaSeleccionada.IdCategoria,
                NombreProducto = NombreProducto.Trim(),
                StockProducto = StockProducto,
                PrecioProducto = PrecioProducto,
                EstadoProducto = EstadoProducto
            };

            if (IsEditMode)
            {
                await _repositoryProducto.EditarProducto(producto);
                _notificationService.ShowSuccess("Producto actualizado",
                    $"\"{NombreProducto}\" se actualizó correctamente.");
            }
            else
            {
                await _repositoryProducto.CrearNuevoProducto(producto);
                _notificationService.ShowSuccess("Producto agregado",
                    $"\"{NombreProducto}\" se agregó correctamente.");
            }
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Error al guardar", ex.Message);
            throw; // relanzar para que el diálogo no se cierre
        }
    }
}