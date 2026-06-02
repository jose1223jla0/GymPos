using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymPos.Models;

public class Producto
{
    [Key]
    public int IdProducto { get; set; }
    public int IdCategoria { get; set; }
    public required string NombreProducto { get; set; }
    public int  StockProducto { get; set; }
    public decimal PrecioProducto { get; set; }
    public bool EstadoProducto { get; set; }
    //propiedades de navegacion
    [ForeignKey(nameof(IdCategoria))]
    public Categoria? Categoria { get; set; }
    public ICollection<DetalleVenta> DetallesVenta { get; set; }= new List<DetalleVenta>();
}
