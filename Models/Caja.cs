using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GymPos.Models;

public class Caja
{
    [Key]
    public int IdCaja { get; set; }
    public int IdUsuario { get; set; }
    public DateTime FechaApertura { get; private set; } = DateTime.Now;
    public DateTime? FechaCierre { get; private set; }
    public decimal MontoInicial { get; private set; }
    public decimal? MontoFinal { get; private set; }
    public bool Abierta { get; private set; }
    // Usuario que abrió la caja
    [ForeignKey(nameof(IdUsuario))]
    public Usuario? Usuario { get; private set; }
    // navegación
    public List<MovimientoCaja> Movimientos { get; set; } = new();
    private Caja() { }// Constructor privado para EF Core
    public Caja(decimal montoInicial)
    {
        if (montoInicial < 0)
        {
            throw new InvalidOperationException("Monto inicial inválido");
        }
        MontoInicial = montoInicial;
        FechaApertura = DateTime.Now;
        Abierta = true;
    }

    public void Abrir(decimal montoInicial)
    {
        if (Abierta)
        {
            throw new InvalidOperationException("La caja ya está abierta.");
        }
        if (montoInicial < 0)
        {
            throw new InvalidOperationException("Monto inicial inválido.");
        }
        MontoInicial = montoInicial;
        FechaApertura = DateTime.Now;
        Abierta = true;
    }


    public void AgregarMovimiento(MovimientoCaja movimiento)
    {
        if (!Abierta)
        {
            throw new InvalidOperationException("Caja cerrada.");
        }
        Movimientos.Add(movimiento);
    }

    public decimal CalcularSaldo() => MontoInicial + Movimientos.Sum(x => x.MontoEfectivo);
    public void Cerrar()
    {
        if (!Abierta)
        {
            throw new InvalidOperationException("Ya está cerrada.");
        }
        MontoFinal = CalcularSaldo();
        FechaCierre = DateTime.Now;
        Abierta = false;
    }
}
public class ResumenCaja
{
    public DateTime FechaApertura { get; set; }
    public decimal MontoInicial { get; set; }
    public decimal TotalIngresos { get; set; }
    public decimal TotalEgresos { get; set; }
    public decimal SaldoActual { get; set; }
    public int CantMovimientos { get; set; }
}