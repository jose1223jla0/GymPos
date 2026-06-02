using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymPos.Models;

public class Suscripcion
{
    [Key]
    public int IdSuscripcion { get; set; }
    public int IdCliente { get; set; }
    public int IdMembresia { get; set; }
    public DateOnly FechaInicio { get; set; }
    public DateOnly FechaFin { get; set; }
    public bool Cancelada { get; set; }
    public EstadoSuscripcion EstadoSuscripcion { get; set; } = EstadoSuscripcion.Activa;
    //propiwdades de navegación
    public List<Pago> Pagos { get; set; } = new();
    public List<Asistencia> Asistencias { get; set; } = new();
    [ForeignKey(nameof(IdCliente))]
    public Cliente? Cliente { get; set; }
    [ForeignKey(nameof(IdMembresia))]
    public Membresia? Membresia { get; set; }


    [NotMapped]
    public int SesionesUsadas => Asistencias.Count;

    [NotMapped]
    public int SesionesRestantes => Membresia != null ? Membresia.Sesiones - SesionesUsadas : 0;

    [NotMapped]
    public EstadoSuscripcion Estado
    {
        get
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            if (Cancelada)
            {
                return EstadoSuscripcion.Cancelada;
            }
            if (FechaFin < hoy)
            {
                return EstadoSuscripcion.Vencida;
            }
            if (Membresia != null && SesionesRestantes <= 0)
            {
                return EstadoSuscripcion.Vencida;
            }
            return EstadoSuscripcion.Activa;
        }
    }
}
public enum EstadoSuscripcion
{
    Activa = 1,
    Vencida = 2,
    Cancelada = 3
}
