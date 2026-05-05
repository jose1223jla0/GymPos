using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymPos.Models;

public class Asistencia
{
    [Key]
    public int IdAsistencia { get; set; }
    public int IdSuscripcion { get; set; }
    public DateOnly Fecha { get; set; }
    //relaciones
    [ForeignKey(nameof(IdSuscripcion))]
    public Suscripcion? Suscripcion { get; set; }
}
