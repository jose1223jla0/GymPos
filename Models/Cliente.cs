using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymPos.Models;

public class Cliente
{
    [Key]
    public int IdCliente { get; set; }
    public required string Dni { get; set; } 
    public required string Nombres { get; set; } 
    public required string Apellidos { get; set; } 
    //relaciones
    public List<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();

    public static implicit operator int(Cliente v)
    {
        throw new NotImplementedException();
    }
}
