using GymPos.Models;
using GymPos.Repository;
using System;
using System.Threading.Tasks;
namespace GymPos.Services;

public interface IServiceSuscripcion
{
    Task CrearSuscripcion(int clienteId, int membresiaId);
}
public class ServiceSuscripcion : IServiceSuscripcion
{
    private readonly IRepositorySuscripcion _repositorySuscripcion;
    private readonly IRepositoryPago _repositoryPago;
    private readonly IRepositoryCaja _repositoryCaja;
    private readonly IRepositoryMovimientoCaja _repositoryMovimiento;
    private readonly IRepositoryMembresia _repositoryMembresia;
    private readonly INotificationService _notificationService;
    private readonly ICajaEventService _cajaEventService;
    public ServiceSuscripcion(
        IRepositorySuscripcion repositorySuscripcion,
        IRepositoryPago repositoryPago,
        IRepositoryCaja repositoryCaja,
        IRepositoryMovimientoCaja repositoryMovimiento,
        IRepositoryMembresia repositoryMembresia,
        INotificationService notificationService,
        ICajaEventService cajaEventService)
    {
        _repositorySuscripcion = repositorySuscripcion;
        _repositoryPago = repositoryPago;
        _repositoryCaja = repositoryCaja;
        _repositoryMovimiento = repositoryMovimiento;
        _repositoryMembresia = repositoryMembresia;
        _notificationService = notificationService;
        _cajaEventService = cajaEventService;
    }
    /// <summary>
    /// Crea una suscripción para un cliente, registra el pago asociado y añade el movimiento correspondiente a la caja.
    /// </summary>
    /// <remarks>Valida la membresía, crea la suscripción y el pago, confirma el pago y registra el movimiento
    /// en la caja abierta.</remarks>
    /// <param name="clienteId">Identificador del cliente asociado a la suscripción.</param>
    /// <param name="membresiaId">Identificador de la membresía que se asigna a la suscripción.</param>
    /// <returns>Una tarea que representa la operación asincrónica.</returns>
    /// <exception cref="Exception">Se lanza si la membresía indicada no se encuentra.</exception>
    public async Task CrearSuscripcion(int clienteId, int membresiaId)
    {
        ///========== buscar membresia=======///
        var membresia = await _repositoryMembresia.GetById(membresiaId);
        if (membresia == null)
        {
            throw new Exception("Membresía no encontrada");
        }
        ///========== buscar caja abierta=======///
        var caja = await _repositoryCaja.ObtenerCajaAbiertaAsync();
        /// Crear suscripción
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var suscripcion = new Suscripcion
        {
            IdCliente = clienteId,
            IdMembresia = membresiaId,
            FechaInicio = hoy,
            FechaFin = hoy.AddDays(membresia.Sesiones),
            Cancelada = false
        };
        await _repositorySuscripcion.AddSuscripcionAsync(suscripcion);
        ///========= Crear pago asociado============//
        var pago = new Pago(suscripcion.IdSuscripcion, membresia.Precio);
        await _repositoryPago.CrearAsync(pago);
        pago.Confirmar();
        await _repositoryPago.GuardarCambiosAsync();
        // ========== Registrar movimiento en caja ======/
        var movimiento = MovimientoCaja.CrearIngreso(
            idCaja: caja.IdCaja,
            monto: membresia.Precio,
            concepto: $"Suscripción {membresia.Nombre} — cliente #{clienteId}",
            idPago: pago.IdPago
        );
        caja.AgregarMovimiento(movimiento);
        await _repositoryMovimiento.AddAsync(movimiento);
        await _repositoryMovimiento.SaveChangesAsync();
        _cajaEventService.NotificarMovimiento();
        _notificationService?.ShowInfo("Movimiento","Se registró un nuevo movimiento de caja.");
    }

    //public async Task CrearSuscripcion(int clienteId, int membresiaId)
    //{
    //    var membresia = await _repositoryMembresia.GetById(membresiaId);
    //    if (membresia == null)
    //    {
    //        throw new Exception("Membresía no encontrada");
    //    }
    //    var fechaInicio = DateOnly.FromDateTime(DateTime.Now);
    //    var fechaFin = fechaInicio.AddDays(membresia.Sesiones);
    //    var nueva = new Suscripcion
    //    {
    //        IdCliente = clienteId,
    //        IdMembresia = membresiaId,
    //        FechaInicio = fechaInicio,
    //        FechaFin = fechaFin,
    //        Cancelada = false,
    //        EstadoSuscripcion = EstadoSuscripcion.Activa
    //    };
    //    await _repositorySuscripcion.AddSuscripcionAsync(nueva);
    //}
}
