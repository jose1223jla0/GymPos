using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace GymPos.Models;

public class MovimientoCaja
{
    [Key]
    public int IdMovimientoCaja { get; set; }
    public int IdCaja { get; set; }
    public int? IdVenta { get; set; }
    public int? IdPago { get; set; }
    public TipoMovimiento TipoMovimiento { get; set; }
    public decimal Monto { get; set; }
    public DateOnly Fecha { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public string Concepto { get; set; } = string.Empty;

    [ForeignKey(nameof(IdCaja))]
    public Caja? Caja { get; set; }

    [ForeignKey(nameof(IdPago))]
    public Pago? Pago { get; set; }
    [ForeignKey(nameof(IdVenta))]
    public Venta? Venta { get; set; }

    [NotMapped]
    public string MontoEfectivoStr
    {
        get
        {
            var cultura = new CultureInfo("es-PE");
            var signo = MontoEfectivo >= 0 ? "+" : "-";
            return $"{signo}{Math.Abs(MontoEfectivo).ToString("C2", cultura)}";
        }
    }
    private MovimientoCaja() { } // Para EF Core
    public MovimientoCaja(int idCaja, TipoMovimiento tipo, decimal monto, string concepto,
                      int? idPago = null, int? idVenta = null)
    {
        if (monto <= 0)
        {
            throw new InvalidOperationException("El monto debe ser mayor a cero.");
        }
        if (string.IsNullOrWhiteSpace(concepto))
        {
            throw new InvalidOperationException("El concepto es obligatorio.");
        }
        if (idPago.HasValue && idVenta.HasValue)
        {
            throw new InvalidOperationException("Un movimiento no puede tener pago y venta al mismo tiempo.");
        }

        IdCaja = idCaja;
        TipoMovimiento = tipo;
        Monto = monto;
        Concepto = concepto.Trim();
        Fecha = DateOnly.FromDateTime(DateTime.Now);
        IdPago = idPago;
        IdVenta = idVenta;
    }

    // ─── Factory methods ───────────────────────────────────────────────────────
    public static MovimientoCaja CrearIngreso(int idCaja, decimal monto, string concepto, int? idPago = null)
        => new(idCaja, TipoMovimiento.Ingreso, monto, concepto, idPago);

    public static MovimientoCaja CrearIngresoVenta(int idCaja, decimal monto, string concepto, int idVenta)
        => new(idCaja, TipoMovimiento.Ingreso, monto, concepto, idVenta: idVenta);   // ✅ nuevo factory

    public static MovimientoCaja CrearEgreso(int idCaja, decimal monto, string concepto)
        => new(idCaja, TipoMovimiento.Egreso, monto, concepto);

    // ─── Propiedades calculadas ────────────────────────────────────────────────

    /// <summary>
    /// Monto con signo: positivo para ingresos, negativo para egresos.
    /// Usado por Caja.CalcularSaldo().
    /// </summary>
    [NotMapped]
    public decimal MontoEfectivo
    {
        get
        {
            if (TipoMovimiento == TipoMovimiento.Ingreso)
            {
                return Monto;
            }
            else
            {
                return -Monto;
            }
        }
    }
    // ─── Consultas de estado ───────────────────────────────────────────────────
    /// <summary>
    /// Indica si este movimiento fue generado automáticamente por un pago.
    /// </summary>
    public bool EsAutomatico() => IdPago.HasValue || IdVenta.HasValue;
    public bool EsPorMembresia() => IdPago.HasValue;
    public bool EsPorVenta() => IdVenta.HasValue;
    public bool EsManual() => !IdPago.HasValue && !IdVenta.HasValue;
    public bool EsIngreso() => TipoMovimiento == TipoMovimiento.Ingreso;
    public bool EsEgreso() => TipoMovimiento == TipoMovimiento.Egreso;
    public override string ToString() => $"[{TipoMovimiento}] {Concepto} — {Monto:C} ({Fecha})";

}
/// <summary>
/// 
/// </summary>

public enum TipoMovimiento
{
    Ingreso = 1,
    Egreso = 2,
}
