using System.ComponentModel.DataAnnotations;

namespace GymPos.Models;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string ApellidosUsuario { get; set; } = null!;
    public string UsernameDni { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool EstadoUsuario { get; set; }
    public Rol Rol { get; set; }
}


public enum Rol
{
    Administrador = 1,
    Recepcionista = 2
}