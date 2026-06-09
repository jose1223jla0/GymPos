using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ProductoVM;

public partial class ListProductoViewModel : ObservableObject
{
    // ── Colección de productos mostrados en la tabla ──────────────────────────
    public ObservableCollection<Producto> ListProductos { get; } = new();

    // ── Paginación ────────────────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPages))]
    private int currentPage = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPages))]
    private int pageSize = 20;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalPages))]
    private int totalItems;

    public int TotalPages => PageSize == 0 ? 0 : (TotalItems + PageSize - 1) / PageSize;

    // ── Búsqueda ──────────────────────────────────────────────────────────────
    [ObservableProperty]
    private string? searchTerm;

    private CancellationTokenSource? _searchCts;

    // ── Estado de carga ───────────────────────────────────────────────────────
    [ObservableProperty]
    private bool isLoading;

    // ── Dependencias ──────────────────────────────────────────────────────────
    private readonly IRepositoryProducto _repositoryProducto;
    private readonly INotificationService _notificationService;

    // ── Eventos para abrir diálogos ───────────────────────────────────────────
    public event Action<Producto>? OpenEditDialogRequest;
    public event Action? OpenAddDialogRequest;

    // ── Comandos de paginación ────────────────────────────────────────────────
    public IAsyncRelayCommand NextPageCommand { get; }
    public IAsyncRelayCommand PrevPageCommand { get; }
    public IAsyncRelayCommand<int> GoToPageCommand { get; }

    // ── Constructor ───────────────────────────────────────────────────────────
    public ListProductoViewModel(IRepositoryProducto repositoryProducto, INotificationService notificationService)
    {
        _repositoryProducto = repositoryProducto;
        _notificationService = notificationService;

        NextPageCommand = new AsyncRelayCommand(
            async () =>
            {
                if (CurrentPage < TotalPages)
                {
                    CurrentPage++;
                    await LoadProductos();
                }
            },
            () => CurrentPage < TotalPages);

        PrevPageCommand = new AsyncRelayCommand(
            async () =>
            {
                if (CurrentPage > 1)
                {
                    CurrentPage--;
                    await LoadProductos();
                }
            },
            () => CurrentPage > 1);

        GoToPageCommand = new AsyncRelayCommand<int>(async page =>
        {
            CurrentPage = Math.Clamp(page, 1, TotalPages == 0 ? 1 : TotalPages);
            await LoadProductos();
        });
    }

    // ── Inicialización ────────────────────────────────────────────────────────
    public async Task InitAsync() => await LoadProductos();

   
    partial void OnSearchTermChanged(string? value)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var ct = _searchCts.Token;
        var term = value?.Trim();
        if (!string.IsNullOrEmpty(term) && term.Length < 3)
        {
            return;
        }
        var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        // Esperamos 400 ms (debounce) y luego cargamos EN EL HILO DE UI
        _ = Task.Delay(400, ct)
            .ContinueWith(
                async _ =>
                {
                    if (ct.IsCancellationRequested) return;
                    CurrentPage = 1;
                    await LoadProductos();
                },
                ct,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                uiScheduler);  
    }

    // ── Carga / filtrado de productos ─────────────────────────────────────────
    public async Task LoadProductos()
    {
        try
        {
            IsLoading = true;

            var term = SearchTerm?.Trim();

            if (!string.IsNullOrWhiteSpace(term))
            {
                var encontrados = await _repositoryProducto.ObtenerPorNombreAsync(term);
                ListProductos.Clear();
                foreach (var p in encontrados)
                    ListProductos.Add(p);

                TotalItems = ListProductos.Count;
                CurrentPage = 1;
            }
            else
            {
                var resultado = await _repositoryProducto.ObtenerActivosPaginadoAsync(
                    CurrentPage, PageSize, null);
                ListProductos.Clear();
                foreach (var p in resultado.Items)
                    ListProductos.Add(p);
                TotalItems = resultado.TotalItems;
            }

            NextPageCommand.NotifyCanExecuteChanged();
            PrevPageCommand.NotifyCanExecuteChanged();
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

    // ── Comandos de diálogo ───────────────────────────────────────────────────
    [RelayCommand]
    private void Edit(Producto producto) => OpenEditDialogRequest?.Invoke(producto);

    [RelayCommand]
    private void Add() => OpenAddDialogRequest?.Invoke();
}