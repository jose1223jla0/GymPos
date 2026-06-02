using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Asistencias;

public partial class AsistenciaViewModel : ObservableObject
{
    public ObservableCollection<SuscripcionAsistenciaVM> SuscripcionesActivas { get; } = new();
    private readonly IServiceAsistencia _serviceAsistencia;
    public AsistenciaViewModel(IServiceAsistencia serviceAsistencia)
    {
        _serviceAsistencia = serviceAsistencia;
    }
    public async Task InitAsync()
    {
        await LoadSuscripcionesActivas();
    }

    private async Task LoadSuscripcionesActivas()
    {
        var data = await _serviceAsistencia.GetSuscripcionesConEstadoAsync();
        SuscripcionesActivas.Clear();
        foreach (var item in data)
        {
            SuscripcionesActivas.Add(item);
        }
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



