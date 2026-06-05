using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Asistencias;

public partial class AsistenciaViewModel : ObservableObject
{
    public ObservableCollection<SuscripcionAsistenciaVM> SuscripcionesActivas { get; } = new();
    private readonly IServiceAsistencia _serviceAsistencia;
    private readonly List<SuscripcionAsistenciaVM> _allSuscripciones = new();
    public AsistenciaViewModel(IServiceAsistencia serviceAsistencia)
    {
        _serviceAsistencia = serviceAsistencia;
    }
    [ObservableProperty]
    private string searchText;

    partial void OnSearchTextChanged(string value)
    {
        ApplyFilter();
    }
    public async Task InitAsync()
    {
        await LoadSuscripcionesActivas();
    }

    private async Task LoadSuscripcionesActivas()
    {
        var data = await _serviceAsistencia.GetSuscripcionesConEstadoAsync();
        _allSuscripciones.Clear();
        _allSuscripciones.AddRange(data);
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        SuscripcionesActivas.Clear();
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var item in _allSuscripciones)
                SuscripcionesActivas.Add(item);
            return;
        }

        var q = SearchText.Trim();
        var filtered = _allSuscripciones.Where(s =>
            (!string.IsNullOrEmpty(s.Suscripcion?.Cliente?.Dni) && s.Suscripcion.Cliente.Dni.Contains(q, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrEmpty(s.Suscripcion?.Cliente?.Nombres) && s.Suscripcion.Cliente.Nombres.Contains(q, StringComparison.OrdinalIgnoreCase))
            || (!string.IsNullOrEmpty(s.Suscripcion?.Cliente?.Apellidos) && s.Suscripcion.Cliente.Apellidos.Contains(q, StringComparison.OrdinalIgnoreCase))
        );

        foreach (var item in filtered)
            SuscripcionesActivas.Add(item);
    }

    [RelayCommand]
    private async Task RegistrarAsistencia(int idSuscripcion)
    {
        try
        {
            await _serviceAsistencia.RegistrarAsistencia(idSuscripcion);
            await LoadSuscripcionesActivas();
        }
        catch (Exception ex)
        {
            throw new Exception($" no se pudo registrar la  asistencia {ex}");
        }
    }
}



