using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.MembresiasVM;

public partial class ListMembresiaViewModel : ObservableObject
{
    private readonly IRepositoryMembresia _repositoryMembresia;
    private readonly INotificationService _notificationService;

    public ObservableCollection<Membresia> MembresiaList { get; } = new();

    public event Action<Membresia>? OpenEditDialogRequest;
    public event Action? OpenAddDialogRequest;

    [ObservableProperty]
    private bool isLoading = false;

    public ListMembresiaViewModel(
        IRepositoryMembresia repositoryMembresia,
        INotificationService notificationService)
    {
        _repositoryMembresia = repositoryMembresia;
        _notificationService = notificationService;
    }

    public async Task InitAsync()
    {
        await LoadMembresias();
    }

    public async Task LoadMembresias()
    {
        try
        {
            IsLoading = true;
            var lista = await _repositoryMembresia.GetAllAsync();
            MembresiaList.Clear();
            foreach (var item in lista)
                MembresiaList.Add(item);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError("Error", $"No se pudieron cargar las membresías: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Edit(Membresia membresia)
    {
        OpenEditDialogRequest?.Invoke(membresia);
    }

    [RelayCommand]
    private void Add()
    {
        OpenAddDialogRequest?.Invoke();
    }
}