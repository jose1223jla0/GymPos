using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace GymPos.Models;

public class Venta
{
    [Key]
    public int IdVenta { get; set; }
    public DateOnly FechaVenta { get; set; }
    public decimal Total { get; set; }
    //propiedades de navegacion
    public MovimientoCaja? MovimientoCaja { get; set; }
    public ICollection<DetalleVenta> VentaDetalles { get; set; } = new List<DetalleVenta>();
}
