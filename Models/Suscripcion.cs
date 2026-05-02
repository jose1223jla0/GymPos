using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymPos.Models;

public class Suscripcion
{
    [Key]
    public int IdSuscripcion { get; set; }
    public int IdCliente { get; set; }
    public int IdMembresia { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public EstadoSuscripcion Estado { get; set; } = EstadoSuscripcion.Activa;
    //propiedades de navegación
    public List<Asistencia> Asistencias { get; set; } = new();
    public Cliente? Cliente { get; set; }
    public Membresia? Membresia { get; set; }
}

public enum EstadoSuscripcion
{
    Activa = 1,
    Vencida = 2,
    Cancelada = 3
}
