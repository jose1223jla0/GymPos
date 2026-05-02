using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymPos.Models;

public class Membresia
{
    [Key]
    public int IdMembresia { get; set; }
    public required string Nombre { get; set; }
    public int Sesiones { get; set; }
    public decimal Precio { get; set; }
    //propiedades de navegación
    public List<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}
