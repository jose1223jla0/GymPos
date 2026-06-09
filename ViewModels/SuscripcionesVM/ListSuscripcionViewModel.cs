using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GymPos.ViewModels.SuscripcionesVM;

public partial class ListSuscripcionViewModel : ObservableObject
{
    // Colección mostrada
    public ObservableCollection<Suscripcion> ListSuscripciones { get; } = new();

    // Paginación
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

    // Buscador
    [ObservableProperty]
    private string? searchTerm;

    private CancellationTokenSource? _searchCts;

    // Estado de carga
    [ObservableProperty]
    private bool isLoading;

    // Dependencias
    private readonly IRepositorySuscripcion _repositorySuscripcion;

    // Comandos paginación
    public IAsyncRelayCommand NextPageCommand { get; }
    public IAsyncRelayCommand PrevPageCommand { get; }
    public IAsyncRelayCommand<int> GoToPageCommand { get; }

    public ListSuscripcionViewModel(IRepositorySuscripcion repositorySuscripcion)
    {
        _repositorySuscripcion = repositorySuscripcion;

        NextPageCommand = new AsyncRelayCommand(
            async () =>
            {
                if (CurrentPage < TotalPages)
                {
                    CurrentPage++;
                    await LoadSuscripciones();
                }
            },
            () => CurrentPage < TotalPages);

        PrevPageCommand = new AsyncRelayCommand(
            async () =>
            {
                if (CurrentPage > 1)
                {
                    CurrentPage--;
                    await LoadSuscripciones();
                }
            },
            () => CurrentPage > 1);

        GoToPageCommand = new AsyncRelayCommand<int>(async page =>
        {
            CurrentPage = Math.Clamp(page, 1, TotalPages == 0 ? 1 : TotalPages);
            await LoadSuscripciones();
        });
    }

    public async Task InitAsync()
    {
        await LoadSuscripciones();
    }
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
        _ = Task.Delay(400, ct)
            .ContinueWith(
                async _ =>
                {
                    if (ct.IsCancellationRequested) return;
                    CurrentPage = 1;
                    await LoadSuscripciones();
                },
                ct,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                uiScheduler);
    }
    /// <summary>
    /// Listar usuarios, se llama cada vez que se navega a esta vista para mostrar los cambios realizados en otras vistas (crear, editar, eliminar)
    /// </summary>
    /// <returns></returns>

    private async Task LoadSuscripciones()
    {
        try
        {
            IsLoading = true;

            var term = SearchTerm?.Trim();

            var all = (await _repositorySuscripcion.GetAllSuscripcion()).ToList();

            IEnumerable<Suscripcion> filtered = all;

            if (!string.IsNullOrWhiteSpace(term))
            {
                var t = term.ToLower();
                filtered = filtered.Where(s =>
                    ((s.Cliente != null) && ($"{s.Cliente.Nombres} {s.Cliente.Apellidos}".ToLower().Contains(t) || (s.Cliente.Dni ?? string.Empty).ToLower().Contains(t)))
                    || ((s.Membresia != null) && (s.Membresia.Nombre ?? string.Empty).ToLower().Contains(t))
                    || s.IdSuscripcion.ToString().Contains(t)
                );
            }

            TotalItems = filtered.Count();

            var page = Math.Max(1, CurrentPage);
            var items = filtered.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ListSuscripciones.Clear();
            foreach (var item in items)
                ListSuscripciones.Add(item);

            // Si la búsqueda está activa y no hay paginación en servidor, aseguremos estar en la primera página
            if (!string.IsNullOrWhiteSpace(term))
            {
                CurrentPage = 1;
            }

            NextPageCommand.NotifyCanExecuteChanged();
            PrevPageCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al cargar las suscripciones: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task Edit()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task Delete()
    {

    }

    // ObservableObject provides RaisePropertyChanged
}
