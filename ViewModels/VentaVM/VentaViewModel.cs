using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Services;
using GymPos.ViewModels.CarritoVM;
using GymPos.ViewModels.CategoriaVM;
using GymPos.ViewModels.ProductoVM;
using System;
using System.Collections.Generic;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.ViewModels;

public partial class VentaViewModel : ObservableObject
{
    private readonly IServiceVenta _ventaService;
    private readonly IProductoService _productoService;
    private readonly ICategoriaService _categoriaService;
    private readonly IRepositoryCaja _repoCaja;

    // ── Colecciones públicas ────────────────────────────────────────────────
    public ObservableCollection<ProductoViewModel> Productos { get; } = new();
    private readonly List<ProductoViewModel> _productosCache = new();
    public ObservableCollection<CategoriaViewModel> Categorias { get; } = new();
    public ObservableCollection<CarritoItemViewModel> Carrito { get; } = new();

    // ── Estado UI ───────────────────────────────────────────────────────────
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _mensajeError;
    [ObservableProperty] private bool _ventaExitosa;
    [ObservableProperty] private int _idCajaActiva;

    [ObservableProperty]
    private string? _terminoBusqueda;

    private CategoriaViewModel? _categoriaSeleccionada;

    // ── Propiedades calculadas ──────────────────────────────────────────────
    public decimal Total => Carrito.Sum(x => x.Subtotal);
    public string TotalFormateado => Total.ToString("F2");
    public bool HayItems => Carrito.Count > 0;
    public int TotalItems => Carrito.Sum(x => x.Cantidad);

    // ── Constructor ─────────────────────────────────────────────────────────
    public VentaViewModel(
        IServiceVenta ventaService,
        IProductoService productoService,
        ICategoriaService categoriaService,
        IRepositoryCaja repoCaja)
    {
        _ventaService = ventaService;
        _productoService = productoService;
        _categoriaService = categoriaService;
        _repoCaja = repoCaja;

        Carrito.CollectionChanged += Carrito_CollectionChanged;
    }

    // ── Reactividad del carrito ─────────────────────────────────────────────
    private void Carrito_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
            foreach (CarritoItemViewModel item in e.NewItems)
                item.PropertyChanged += CarritoItem_PropertyChanged;

        if (e.OldItems != null)
            foreach (CarritoItemViewModel item in e.OldItems)
                item.PropertyChanged -= CarritoItem_PropertyChanged;

        Recalcular();
    }

    private void CarritoItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CarritoItemViewModel.Cantidad)
                           or nameof(CarritoItemViewModel.Subtotal))
            Recalcular();
    }

    private void Recalcular()
    {
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalFormateado));
        OnPropertyChanged(nameof(HayItems));
        OnPropertyChanged(nameof(TotalItems));
        RegistrarVentaCommand.NotifyCanExecuteChanged();
    }

    // ── Helper: crea un CarritoItemViewModel con los comandos ya inyectados ─
    // Así los botones +/- y Eliminar dentro del Flyout pueden usar
    // {x:Bind AgregarCommand} / {x:Bind QuitarCommand} / {x:Bind EliminarCommand}
    // directamente sobre el DataType del item, sin necesitar ElementName.
    private CarritoItemViewModel CrearItem(Producto producto) => new(producto)
    {
        AgregarCommand = AgregarAlCarritoCommand,
        QuitarCommand = QuitarDelCarritoCommand,
        EliminarCommand = EliminarItemCommand,
    };

    // ════════════════════════════════════════════════════════════════════════
    // COMANDOS
    // ════════════════════════════════════════════════════════════════════════

    [RelayCommand]
    public async Task CargarDatosAsync()
    {
        IsLoading = true;
        MensajeError = null;

        try
        {
            var cats = await _categoriaService.ObtenerCategoriasAsync();

            Categorias.Clear();

            var todos = new CategoriaViewModel(new Categoria
            {
                IdCategoria = 0,
                NombreCategoria = "Todos",
            })
            { IsSeleccionada = true };

            Categorias.Add(todos);
            _categoriaSeleccionada = todos;

            foreach (var c in cats)
                Categorias.Add(new CategoriaViewModel(c));

            await RefrescarProductosAsync();
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar datos: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task SeleccionarCategoriaAsync(CategoriaViewModel cat)
    {
        if (_categoriaSeleccionada != null)
            _categoriaSeleccionada.IsSeleccionada = false;

        _categoriaSeleccionada = cat;
        cat.IsSeleccionada = true;

        await RefrescarProductosAsync(
            cat.IdCategoria == 0 ? null : (int?)cat.IdCategoria);
    }

    private async Task RefrescarProductosAsync(int? idCategoria = null)
    {
        IsLoading = true;
        try
        {
            var lista = await _productoService.ObtenerProductosAsync(idCategoria, TerminoBusqueda);
            Productos.Clear();
            _productosCache.Clear();
            foreach (var p in lista)
                _productosCache.Add(new ProductoViewModel(p));

            AplicarFiltroBusqueda();
        }
        catch (Exception ex)
        {
            MensajeError = $"Error al cargar productos: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AplicarFiltroBusqueda()
    {
        Productos.Clear();

        if (string.IsNullOrWhiteSpace(TerminoBusqueda))
        {
            foreach (var p in _productosCache)
                Productos.Add(p);
            return;
        }

        var termino = TerminoBusqueda.Trim().ToLowerInvariant();
        foreach (var p in _productosCache)
            if (p.NombreProducto != null && p.NombreProducto.ToLowerInvariant().Contains(termino))
                Productos.Add(p);
    }

    partial void OnTerminoBusquedaChanged(string? value) => AplicarFiltroBusqueda();

    // ── Agregar al carrito ──────────────────────────────────────────────────
    [RelayCommand]
    public void AgregarAlCarrito(object parametro)
    {
        MensajeError = null;

        // Viene del botón "+" dentro del carrito: aumentar cantidad
        if (parametro is CarritoItemViewModel itemExistente)
        {
            var prodVm = Productos.FirstOrDefault(p => p.IdProducto == itemExistente.IdProducto);
            if (prodVm == null || prodVm.StockProducto <= 0)
            {
                MensajeError = $"Sin stock suficiente para '{itemExistente.NombreProducto}'.";
                return;
            }

            itemExistente.Cantidad++;
            prodVm.StockProducto--;
            Recalcular();
            return;
        }

        // Viene de una tarjeta de producto
        if (parametro is not ProductoViewModel producto) return;

        if (producto.StockProducto <= 0)
        {
            MensajeError = $"Sin stock suficiente para '{producto.NombreProducto}'.";
            return;
        }

        var item = Carrito.FirstOrDefault(x => x.IdProducto == producto.IdProducto);

        if (item is not null)
        {
            item.Cantidad++;
            producto.StockProducto--;
        }
        else
        {
            // ← Usar CrearItem() en lugar de new CarritoItemViewModel(...)
            //   para que los comandos queden inyectados desde el inicio.
            Carrito.Add(CrearItem(new Producto
            {
                IdProducto = producto.IdProducto,
                NombreProducto = producto.NombreProducto,
                PrecioProducto = producto.PrecioProducto,
                StockProducto = producto.StockProducto,
                EstadoProducto = true,
                IdCategoria = 0
            }));
            producto.StockProducto--;
            OnPropertyChanged(nameof(Carrito));
        }

        Recalcular();
    }

    // ── Quitar uno ──────────────────────────────────────────────────────────
    [RelayCommand]
    public void QuitarDelCarrito(CarritoItemViewModel item)
    {
        var prodVm = Productos.FirstOrDefault(p => p.IdProducto == item.IdProducto);

        if (item.Cantidad > 1)
        {
            item.Cantidad--;
            if (prodVm != null) prodVm.StockProducto++;
        }
        else
        {
            if (prodVm != null) prodVm.StockProducto++;
            Carrito.Remove(item);
            OnPropertyChanged(nameof(Carrito));
        }

        Recalcular();
    }

    // ── Eliminar item completo ──────────────────────────────────────────────
    [RelayCommand]
    public void EliminarItem(CarritoItemViewModel item)
    {
        var prodVm = Productos.FirstOrDefault(p => p.IdProducto == item.IdProducto);
        if (prodVm != null) prodVm.StockProducto += item.Cantidad;

        Carrito.Remove(item);
        OnPropertyChanged(nameof(Carrito));
        Recalcular();
    }

    // ── Limpiar carrito ─────────────────────────────────────────────────────
    [RelayCommand]
    public void LimpiarCarrito()
    {
        foreach (var it in Carrito.ToList())
        {
            var prodVm = Productos.FirstOrDefault(p => p.IdProducto == it.IdProducto);
            if (prodVm != null) prodVm.StockProducto += it.Cantidad;
        }
        Carrito.Clear();
        OnPropertyChanged(nameof(Carrito));
        Recalcular();
    }

    // ── Registrar venta ─────────────────────────────────────────────────────
    private bool PuedeRegistrarVenta() => Carrito.Any() && !IsLoading;

    [RelayCommand(CanExecute = nameof(PuedeRegistrarVenta))]
    public async Task RegistrarVentaAsync()
    {
        IsLoading = true;
        MensajeError = null;
        VentaExitosa = false;

        try
        {
            var venta = new Venta
            {
                FechaVenta = DateOnly.FromDateTime(DateTime.Now),
                Total = Total,
                VentaDetalles = Carrito.Select(x => x.ToDetalle()).ToList()
            };

            var idCajaAUsar = IdCajaActiva;
            if (idCajaAUsar == 0)
            {
                var cajaAbierta = await _repoCaja.ObtenerCajaAbiertaAsync();
                idCajaAUsar = cajaAbierta.IdCaja;
            }
            await _ventaService.CrearVentaAsync(venta, idCajaAUsar);
            VentaExitosa = true;
            LimpiarCarrito();
            await RefrescarProductosAsync( _categoriaSeleccionada?.IdCategoria == 0  ? null   : (int?)_categoriaSeleccionada?.IdCategoria);
        }
        catch (Exception ex)
        {
            MensajeError = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }
}