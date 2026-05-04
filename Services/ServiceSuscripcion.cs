using GymPos.Models;
using GymPos.Repository;
using System;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IServiceSuscripcion
{
    Task CrearSuscripcion(int clienteId, int membresiaId);
}
public class ServiceSuscripcion: IServiceSuscripcion
{
    private readonly IRepositorySuscripcion _repositorySuscripcion;
    private readonly IRepositoryMembresia _repositoryMembresia;
    public ServiceSuscripcion(IRepositorySuscripcion repositorySuscripcion, IRepositoryMembresia repositoryMembresia)
    {
        _repositorySuscripcion = repositorySuscripcion;
        _repositoryMembresia = repositoryMembresia;
    }

    public async Task CrearSuscripcion(int clienteId, int membresiaId)
    {

        var activas = await _repositorySuscripcion.GetActivasByCliente(clienteId);

        foreach (var s in activas)
        {
            if (s.FechaFin < DateOnly.FromDateTime(DateTime.Now))
            {
                s.EstadoSuscripcion = EstadoSuscripcion.Vencida;
            }
        }

        var membresia = await _repositoryMembresia.GetById(membresiaId);

        if (membresia == null)
        {
            throw new Exception("Membresía no encontrada");
        }

        var fechaInicio = DateOnly.FromDateTime(DateTime.Now);

        var nueva = new Suscripcion
        {
            IdCliente = clienteId,
            IdMembresia = membresiaId,
            FechaInicio = fechaInicio,
            FechaFin = fechaInicio.AddDays(membresia.Sesiones),
            EstadoSuscripcion = EstadoSuscripcion.Activa
        };

        await _repositorySuscripcion.AddSuscripcionAsync(nueva);
    }
}
