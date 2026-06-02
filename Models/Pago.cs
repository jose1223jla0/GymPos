using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymPos.Models;

public class Pago
{
    [Key]
    public int IdPago { get; private set; }
    public int IdSuscripcion { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime FechaPago { get; private set; }
    public EstadoPago Estado { get; private set; }
    [ForeignKey(nameof(IdSuscripcion))]
    //propiedad de navegación
    public Suscripcion? Suscripcion { get; set; }

    // Para EF Core
    private Pago() { }
    public Pago(int idSuscripcion, decimal monto)
    {
        if (idSuscripcion <= 0)
        {
            throw new InvalidOperationException("Suscripción inválida.");
        }
        if (monto <= 0)
        {
            throw new InvalidOperationException("El monto debe ser mayor a cero.");
        }
        IdSuscripcion = idSuscripcion;
        Monto = monto;
        FechaPago = DateTime.UtcNow;
        Estado = EstadoPago.Pendiente;
    }

    // ─── Transiciones de estado ─────────────────────────────────────────────── 
    public void Confirmar()
    {
        if (Estado != EstadoPago.Pendiente)
        {
            throw new InvalidOperationException("Solo se puede confirmar un pago pendiente.");
        }
        Estado = EstadoPago.Confirmado;
    }
    public void Anular()
    {
        if (Estado == EstadoPago.Anulado)
        {
            throw new InvalidOperationException("El pago ya está anulado.");
        }
        Estado = EstadoPago.Anulado;
    }


    // ─── Consultas de estado ───────────────────────────────────────────────────
    public bool EstaConfirmado() => Estado == EstadoPago.Confirmado;
    public bool EstaAnulado() => Estado == EstadoPago.Anulado;
    public bool EstaPendiente() => Estado == EstadoPago.Pendiente;
    public override string ToString() => $"Pago #{IdPago} — {Monto:C} [{Estado}] ({FechaPago:dd/MM/yyyy})";
}
public enum EstadoPago
{
    Pendiente = 1,
    Confirmado = 2,
    Anulado = 3,
}