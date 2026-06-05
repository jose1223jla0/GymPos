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
    private readonly IRepositoryCliente _repositoryCliente;

    public ServiceSuscripcion(
        IRepositorySuscripcion repositorySuscripcion,
        IRepositoryPago repositoryPago,
        IRepositoryCaja repositoryCaja,
        IRepositoryMovimientoCaja repositoryMovimiento,
        IRepositoryMembresia repositoryMembresia,
        INotificationService notificationService,
        ICajaEventService cajaEventService,
        IRepositoryCliente repositoryCliente)
    {
        _repositorySuscripcion = repositorySuscripcion;
        _repositoryPago = repositoryPago;
        _repositoryCaja = repositoryCaja;
        _repositoryMovimiento = repositoryMovimiento;
        _repositoryMembresia = repositoryMembresia;
        _notificationService = notificationService;
        _cajaEventService = cajaEventService;
        _repositoryCliente = repositoryCliente;
    }

    public async Task CrearSuscripcion(int clienteId, int membresiaId)
    {
        var membresia = await _repositoryMembresia.GetById(membresiaId) ?? throw new Exception("Membresía no encontrada");
        var cliente = await _repositoryCliente.GetById(clienteId);
        var nombreCliente = cliente != null  ? $"{cliente.Nombres} {cliente.Apellidos}"
            : $"cliente #{clienteId}";

        var caja = await ObtenerCajaAbierta();
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        bool esPaseDiario = membresia.Sesiones == 1;
        var suscripcion = new Suscripcion
        {
            IdCliente = clienteId,
            IdMembresia = membresiaId,
            FechaInicio = hoy,
            FechaFin = esPaseDiario  ? hoy: hoy.AddDays(membresia.Sesiones - 1),
            Cancelada = false
        };
        suscripcion = await _repositorySuscripcion.AddSuscripcionAsync(suscripcion);
        var pago = await CrearPago(suscripcion.IdSuscripcion, membresia.Precio);
        var concepto = esPaseDiario ? $"{membresia.Nombre} — {nombreCliente}"  : $"Suscripción {membresia.Nombre} — {nombreCliente}";

        await RegistrarMovimientoCaja(
            caja.IdCaja,
            pago.IdPago,
            membresia.Precio,
            concepto
        );

        _cajaEventService.NotificarMovimiento();
        _notificationService?.ShowInfo("Movimiento", "Se registró la suscripción correctamente.");
    }

    // ================= HELPERS =================

    private async Task<Caja> ObtenerCajaAbierta()
    {
        try
        {
            return await _repositoryCaja.ObtenerCajaAbiertaAsync();
        }
        catch (InvalidOperationException)
        {
            _notificationService?.ShowError(
                "Caja cerrada",
                "No hay una caja abierta. Aperture la caja antes de continuar."
            );

            throw new InvalidOperationException("Caja cerrada.");
        }
    }

    private async Task<Pago> CrearPago(int suscripcionId, decimal monto)
    {
        var pago = new Pago(suscripcionId, monto);
        pago.Confirmar();

        var creado = await _repositoryPago.CrearAsync(pago);
        await _repositoryPago.GuardarCambiosAsync();

        if (creado.IdPago <= 0)
            throw new InvalidOperationException("No se pudo crear el pago.");

        return creado;
    }

    private async Task RegistrarMovimientoCaja( int cajaId,   int pagoId,   decimal monto,  string concepto)
    {
        var movimiento = MovimientoCaja.CrearIngreso(
            idCaja: cajaId,
            monto: monto,
            concepto: concepto,
            idPago: pagoId
        );
        await _repositoryMovimiento.AddAsync(movimiento);
        await _repositoryMovimiento.SaveChangesAsync();
    }
}