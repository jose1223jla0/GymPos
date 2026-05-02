using System;
using System.ComponentModel.DataAnnotations;

namespace GymPos.Models;

public class Asistencia
{
    [Key]
    public int IdAsistencia { get; set; }
    public int IdSuscripcion { get; set; }
    public DateTime Fecha { get; set; }
    //relaciones
    public Suscripcion? Suscripcion { get; set; }
}
