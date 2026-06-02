using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymPos.Models;
public class DetalleVenta
{
    [Key]
    public int IdDetalleVenta { get; set; }
    public int IdVenta { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    //propiedades de navegacion
    [ForeignKey(nameof(IdVenta))]
    public Venta? Venta { get; set; }

    [ForeignKey(nameof(IdProducto))]
    public Producto? Producto { get; set; }
}