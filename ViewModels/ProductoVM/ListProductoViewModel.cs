using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ProductoVM;

public partial class ListProductoViewModel : ObservableObject
{
    public ObservableCollection<Producto> ListProductos { get; } = new();

    private readonly IRepositoryProducto _repositoryProducto;
    private readonly IRepositoryCategoria _repositoryCategoria;
    private readonly INotificationService _notificationService;

    public event Action<Producto>? OpenEditDialogRequest;
    public event Action? OpenAddDialogRequest;

    [ObservableProperty]
    private bool isLoading = false;

    public ListProductoViewModel(
        IRepositoryProducto repositoryProducto,
        IRepositoryCategoria repositoryCategoria,
        INotificationService notificationService)
    {
        _repositoryProducto = repositoryProducto;
        _repositoryCategoria = repositoryCategoria;
        _notificationService = notificationService;
    }

    public async Task InitAsync()
    {
        await LoadProductos();
    }

    public async Task LoadProductos()
    {
        try
        {
            IsLoading = true;
            var productos = await _repositoryProducto.ObtenerActivosAsync();
            ListProductos.Clear();
            foreach (var product in productos)
                ListProductos.Add(product);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Error", $"No se pudieron cargar los productos: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Edit(Producto producto)
    {
        OpenEditDialogRequest?.Invoke(producto);
    }

    [RelayCommand]
    private void Add()
    {
        OpenAddDialogRequest?.Invoke();
    }
}