
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ProductoVM;

public partial class CreateProductoViewModel : ObservableObject
{
    private readonly IRepositoryProducto _repositoryProducto;
    private readonly IRepositoryCategoria _repositoryCategoria;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Categoria> Categorias { get; } = new();

    [ObservableProperty] private string nombreProducto = string.Empty;
    [ObservableProperty] private int stockProducto;
    [ObservableProperty] private decimal precioProducto;

    // Validación para stock
    [ObservableProperty] private bool stockValido;
    [ObservableProperty] private string stockMensaje = string.Empty;

    public double PrecioProductoDouble
    {
        get => (double)PrecioProducto;
        set => PrecioProducto = (decimal)value;
    }

    partial void OnPrecioProductoChanged(decimal value)
    {
        OnPropertyChanged(nameof(PrecioProductoDouble));
        ValidatePrecio(value);
    }

    [ObservableProperty] private Categoria? categoriaSeleccionada;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool hasError;
    // Validaciones en tiempo real
    [ObservableProperty] private bool nombreValido;
    [ObservableProperty] private string nombreMensaje = string.Empty;
    [ObservableProperty] private bool precioValido;
    [ObservableProperty] private string precioMensaje = string.Empty;
    [ObservableProperty] private bool categoriaValida;
    [ObservableProperty] private string categoriaMensaje = string.Empty;
    [ObservableProperty] private bool puedeGuardar;

    public event EventHandler? ProductoCreado;

    public CreateProductoViewModel(
        IRepositoryProducto repositoryProducto,
        IRepositoryCategoria repositoryCategoria,
        INotificationService notificationService)
    {
        _repositoryProducto = repositoryProducto;
        _repositoryCategoria = repositoryCategoria;
        _notificationService = notificationService;
    }

    private void ValidateNombre(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 3)
        {
            NombreValido = false;
            NombreMensaje = "El nombre debe tener al menos 3 caracteres.";
        }
        else
        {
            NombreValido = true;
            NombreMensaje = string.Empty;
        }
        UpdateCanSave();
    }

    private void ValidateCategoria(Categoria? cat)
    {
        if (cat == null)
        {
            CategoriaValida = false;
            CategoriaMensaje = "Selecciona una categoría.";
        }
        else
        {
            CategoriaValida = true;
            CategoriaMensaje = string.Empty;
        }
        UpdateCanSave();
    }

    private void ValidatePrecio(decimal value)
    {
        // No permitir números negativos; 0 se considera válido
        if (value < 0)
        {
            PrecioValido = false;
            PrecioMensaje = "El precio no puede ser negativo.";
        }
        else
        {
            PrecioValido = true;
            PrecioMensaje = string.Empty;
        }
        UpdateCanSave();
    }

    private void UpdateCanSave()
    {
        PuedeGuardar = NombreValido && CategoriaValida && PrecioValido && StockValido;
    }

    partial void OnNombreProductoChanged(string value)
    {
        ValidateNombre(value);
    }

    partial void OnCategoriaSeleccionadaChanged(Categoria? value)
    {
        ValidateCategoria(value);
    }

    partial void OnStockProductoChanged(int value)
    {
        // No permitir valores negativos en stock
        if (value < 0)
        {
            StockValido = false;
            StockMensaje = "El stock no puede ser negativo.";
        }
        else
        {
            StockValido = true;
            StockMensaje = string.Empty;
        }
        UpdateCanSave();
    }

    public async Task LoadCategoriasAsync()
    {
        var cats = await _repositoryCategoria.ObtenerActivasAsync();
        Categorias.Clear();
        foreach (var c in cats)
            Categorias.Add(c);
    }

    public async Task CargarParaAgregar()
    {
        await LoadCategoriasAsync();
        NombreProducto = string.Empty;
        StockProducto = 0;
        PrecioProducto = 0;
        CategoriaSeleccionada = null;
        HasError = false;
        MensajeError = string.Empty;
    }

    [RelayCommand]
    public async Task GuardarProducto()
    {
        HasError = false;
        if (string.IsNullOrWhiteSpace(NombreProducto))
        {
            MensajeError = "El nombre del producto es obligatorio.";
            HasError = true;
            _notificationService.ShowWarning("Validación", MensajeError);
            return;
        }
        if (CategoriaSeleccionada == null)
        {
            MensajeError = "Selecciona una categoría.";
            HasError = true;
            _notificationService.ShowWarning("Validación", MensajeError);
            return;
        }
        if (PrecioProducto <= 0)
        {
            MensajeError = "El precio debe ser mayor a 0.";
            HasError = true;
            _notificationService.ShowWarning("Validación", MensajeError);
            return;
        }

        IsLoading = true;
        try
        {
            var producto = new Producto
            {
                IdCategoria = CategoriaSeleccionada.IdCategoria,
                NombreProducto = NombreProducto.Trim(),
                StockProducto = StockProducto,
                PrecioProducto = PrecioProducto,
                EstadoProducto = true
            };

            await _repositoryProducto.CrearNuevoProducto(producto);
            _notificationService.ShowSuccess("Producto agregado",
                $"\"{NombreProducto}\" se agregó correctamente.");
            ProductoCreado?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
            HasError = true;
            _notificationService.ShowError("Error al guardar", ex.Message);
            throw;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LimpiarFormulario()
    {
        NombreProducto = string.Empty;
        StockProducto = 0;
        PrecioProducto = 0;
        CategoriaSeleccionada = null;
        HasError = false;
        MensajeError = string.Empty;
    }
}
