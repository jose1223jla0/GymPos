using GymPos.Models;
using GymPos.Repository;
using System;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IServiceCliente
{
    Task RegistrarClienteConSuscripcion(Cliente cliente, int idMembresia);
}
public class ServiceCliente : IServiceCliente
{
    private readonly IRepositoryCliente _repoCliente;
    private readonly IRepositoryMembresia _repoMembresia;
    private readonly IRepositorySuscripcion _repoSuscripcion;
    public ServiceCliente(IRepositoryCliente repositoryCliente, 
                        IRepositoryMembresia repositoryMembresia, 
                        IRepositorySuscripcion repositorySuscripcion)
    {
        _repoCliente = repositoryCliente;
        _repoMembresia = repositoryMembresia;
        _repoSuscripcion = repositorySuscripcion;
    }
    public async Task RegistrarClienteConSuscripcion(Cliente cliente, int idMembresia)
    {
        await _repoCliente.AddCliente(cliente);
        var membresia = await _repoMembresia.GetById(idMembresia);
        if (membresia == null)
        {
            throw new Exception("Membresia no econtrada");
        }
        var fechaInicio = DateOnly.FromDateTime(DateTime.Now);
        var suscripcion = new Suscripcion
        {
            IdCliente = cliente.IdCliente,
            IdMembresia = idMembresia,
            FechaInicio = fechaInicio,
            FechaFin = fechaInicio.AddDays(membresia.Sesiones),
            Estado = EstadoSuscripcion.Activa
        };
        await _repoSuscripcion.AddSuscripcionAsync(suscripcion);
    }
}
