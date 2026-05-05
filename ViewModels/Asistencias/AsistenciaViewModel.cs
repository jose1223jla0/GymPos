using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Asistencias;

public partial class AsistenciaViewModel : ObservableObject
{
    public ObservableCollection<Suscripcion> SuscripcionesActivas { get; set; } = new();

    private readonly IRepositoryAsistencia _repositoryAsistencia;
    private readonly IRepositorySuscripcion _repositorySuscripcion;
    [ObservableProperty]
    private int _totalAsistencias;
    [ObservableProperty]
    private int _asistenciaRestante;
    [ObservableProperty]
    private int _idSuscripcion;
    public AsistenciaViewModel(IRepositoryAsistencia repositoryAsistencia, IRepositorySuscripcion repositorySuscripcion)
    {
        _repositoryAsistencia = repositoryAsistencia;
        _repositorySuscripcion = repositorySuscripcion;
    }

    public async Task InitAsync()
    {
        await LoadSuscripcionesActivas();
    }

    private async Task LoadSuscripcionesActivas()
    {
        var suscripciones = await _repositorySuscripcion.GetSuscripcionesActivas();
        SuscripcionesActivas.Clear();
        foreach (var item in suscripciones)
        {
            SuscripcionesActivas.Add(item);
        }
    }

    [RelayCommand]
    private async Task RegistrarAsistencia(int idSuscripcion)
    {
        bool asistioHoy = await _repositoryAsistencia.ExisteAsistenciaHoyAsync(idSuscripcion);

        if (asistioHoy)
        {
            return;
        }
        var asistencia = new Asistencia
        {
            IdSuscripcion = idSuscripcion,
            Fecha = DateOnly.FromDateTime(DateTime.Now)
        };
        await _repositoryAsistencia.AddAsistenciaAsync(asistencia);
        await LoadSuscripcionesActivas();
    }
}



