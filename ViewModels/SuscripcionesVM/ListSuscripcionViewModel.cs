using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.SuscripcionesVM;

public partial class ListSuscripcionViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Suscripcion> ListSuscripciones { get; } = new();
    private readonly IRepositorySuscripcion _repositorySuscripcion;
    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading != value)
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
    }
    public ListSuscripcionViewModel(IRepositorySuscripcion repositorySuscripcion)
    {
        _repositorySuscripcion = repositorySuscripcion;
    }

    public async Task InitAsync()
    {
        await LoadSuscripciones();
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
            var suscripciones = await _repositorySuscripcion.GetAllSuscripcion();
            ListSuscripciones.Clear();
            foreach (var item in suscripciones)
            {
                ListSuscripciones.Add(item);
            }

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

    // ── INotifyPropertyChanged ────────────────────────────────────────────────
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
