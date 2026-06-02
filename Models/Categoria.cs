using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymPos.Models;
public class Categoria
{
    [Key]
    public int IdCategoria { get; set; }
    public required string NombreCategoria { get; set; }
    //propiedades de navegacion
    public List<Producto> Productos { get; set; } = new List<Producto>();
}
