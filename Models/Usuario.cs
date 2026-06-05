using System.Collections.Generic;
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
    //propiedad  de navegacion
    public ICollection<Caja> Cajas { get; set; } = new List<Caja>();

    // Propiedad calculada para mostrar iniciales en el avatar
    public string Initiales
    {
        get
        {
            var parts = new[] { NombreUsuario, ApellidosUsuario };
            string initials = string.Empty;
            foreach (var p in parts)
            {
                if (!string.IsNullOrWhiteSpace(p))
                    initials += p.Trim()[0];
            }
            return initials.Length > 2 ? initials.Substring(0, 2).ToUpper() : initials.ToUpper();
        }
    }

    public string EstadoTexto => EstadoUsuario ? "Activo" : "Inactivo";

    public string RolTexto => Rol switch
    {
        Rol.SuperAdmin => "Super Administrador",
        Rol.Administrador => "Administrador",
        Rol.Recepcionista => "Recepcionista",
        _ => "Sin rol"
    };
}


public enum Rol
{
    SuperAdmin = 1,
    Administrador = 2,
    Recepcionista = 3
}