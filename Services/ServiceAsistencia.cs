using GymPos.Models;
using GymPos.Repository;
using GymPos.ViewModels.Asistencias;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IServiceAsistencia
{
    Task RegistrarAsistencia(int idSuscripcion);
    Task<List<SuscripcionAsistenciaVM>> GetSuscripcionesConEstadoAsync();
}
public class ServiceAsistencia : IServiceAsistencia
{
    private readonly IRepositoryAsistencia _repositoryAsistencia;
    private readonly IRepositorySuscripcion _repositorySuscripcion;

    public ServiceAsistencia(IRepositoryAsistencia repositoryAsistencia, IRepositorySuscripcion repositorySuscripcion)
    {
        _repositoryAsistencia = repositoryAsistencia;
        _repositorySuscripcion = repositorySuscripcion;
    }

    public async Task<List<SuscripcionAsistenciaVM>> GetSuscripcionesConEstadoAsync()
    {
        var suscripciones = await _repositorySuscripcion.GetSuscripcionesActivas();
        var resultado = new List<SuscripcionAsistenciaVM>();
        foreach (var suscripcion in suscripciones)
        {
            bool asistioHoy = await _repositoryAsistencia.ExisteAsistenciaHoyAsync(suscripcion.IdSuscripcion);
            int totalSesiones = await _repositoryAsistencia.ContarAsistenciasAsync(suscripcion.IdSuscripcion);
            resultado.Add(new SuscripcionAsistenciaVM
            {
                Suscripcion = suscripcion,
                AsistioHoy = asistioHoy,
                TotalSesiones = totalSesiones
            });
        }
        return resultado;
    }

    public async Task RegistrarAsistencia(int idSuscripcion)
    {
        var suscripcion = await _repositorySuscripcion.GetByIdAsync(idSuscripcion);
        if (suscripcion == null)
        {
            throw new Exception("Suscripción no encontrada");
        }
        if (suscripcion.Cancelada)
        {
            throw new Exception("Suscripción cancelada");
        }
        bool asistioHoy = await _repositoryAsistencia .ExisteAsistenciaHoyAsync(idSuscripcion);
        if (asistioHoy)
        {
            throw new Exception("Ya registró asistencia hoy");
        }
        int sesionesUsadas =await _repositoryAsistencia.ContarAsistenciasAsync(idSuscripcion);
        int sesionesDisponibles = suscripcion.Membresia!.Sesiones;
        if (sesionesUsadas >= sesionesDisponibles)
        {
            throw new Exception("No tiene sesiones disponibles");
        }

        var asistencia = new Asistencia
        {
            IdSuscripcion = idSuscripcion,
            Fecha = DateOnly.FromDateTime(DateTime.Now)
        };
        await _repositoryAsistencia.AddAsistenciaAsync(asistencia);
    }
}

