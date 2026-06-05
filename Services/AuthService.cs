using GymPos.Models;
using GymPos.Repository;
using System.Linq;
using System.Threading.Tasks;
using static GymPos.Services.AuthService;

namespace GymPos.Services;

public interface IAuthService
{
    Usuario? UsuarioActual { get; }
    bool EstaAutenticado { get; }
    bool EsSuperAdmin { get; }
    bool EsAdministrador { get; }
    bool EsRecepcionista { get; }
    Task<LoginResult> LoginAsync(string usernameDni, string password);
    void Logout();
    bool TieneRol(params Rol[] roles);

}
public class AuthService : IAuthService
{
    private readonly IRepositoryUsuario _repositoryUsuario;
    public Usuario? UsuarioActual { get; private set; }
    public bool EstaAutenticado => UsuarioActual != null;
    public bool EsSuperAdmin => UsuarioActual?.Rol == Rol.SuperAdmin;
    public bool EsAdministrador => UsuarioActual?.Rol == Rol.SuperAdmin ||  UsuarioActual?.Rol == Rol.Administrador;
    public bool EsRecepcionista =>  UsuarioActual?.Rol == Rol.Recepcionista;
    public AuthService(IRepositoryUsuario repositoryUsuario)
    {
        _repositoryUsuario = repositoryUsuario;
    }
    public async Task<LoginResult> LoginAsync(string usernameDni, string password)
    {
        var usuario = await _repositoryUsuario.GetUsuarioByUsernameDniAsync(usernameDni);
        if (usuario == null)
        {
            return LoginResult.UserNotFound;
        }
        if (!usuario.EstadoUsuario)
        {
            return LoginResult.Inactive;
        }
        bool passwordValida = BCrypt.Net.BCrypt.Verify(password, usuario.Password);
        if (!passwordValida)
        {
            return LoginResult.InvalidPassword;
        }
        UsuarioActual = usuario;
        return LoginResult.Success;
    }
    public void Logout()
    {
        UsuarioActual = null;
    }

public enum LoginResult
{
    Success,
    UserNotFound,
    Inactive,
    InvalidPassword
}
    public bool TieneRol(params Rol[] roles)
    {
        return UsuarioActual != null && roles.Contains(UsuarioActual.Rol);
    }
}
