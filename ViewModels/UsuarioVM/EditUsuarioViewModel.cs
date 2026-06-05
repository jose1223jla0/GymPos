using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.ViewModels.UsuarioVM;

public partial class EditUsuarioViewModel : ObservableObject
{
    private readonly IRepositoryUsuario _repositoryUsuario;
    private Usuario? _usuarioOriginal;

    [ObservableProperty] private int idUsuario;
    [ObservableProperty] private string nombreUsuario = string.Empty;
    [ObservableProperty] private string apellidosUsuario = string.Empty;
    [ObservableProperty] private string usernameDni = string.Empty;
    [ObservableProperty] private string nuevaPassword = string.Empty;
    [ObservableProperty] private string confirmarPassword = string.Empty;
    [ObservableProperty] private string estadoCoincidenciaPassword = "none";
    [ObservableProperty] private bool estadoUsuario = true;
    [ObservableProperty] private Rol rolSeleccionado = Rol.Recepcionista;
    [ObservableProperty] private bool isLoading;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool hasError;
    [ObservableProperty] private bool cambiarPassword;

    public List<Rol> Roles { get; } = new()
    {
        Rol.Administrador,
        Rol.Recepcionista
    };

    public event EventHandler? UsuarioActualizado;

    public EditUsuarioViewModel(IRepositoryUsuario repositoryUsuario)
    {
        _repositoryUsuario = repositoryUsuario;
    }

    partial void OnNuevaPasswordChanged(string value)
    {
        UpdatePasswordMatch();
    }

    partial void OnConfirmarPasswordChanged(string value)
    {
        UpdatePasswordMatch();
    }

    private void UpdatePasswordMatch()
    {
        if (string.IsNullOrEmpty(NuevaPassword) && string.IsNullOrEmpty(ConfirmarPassword))
        {
            EstadoCoincidenciaPassword = "none";
            return;
        }

        EstadoCoincidenciaPassword = NuevaPassword == ConfirmarPassword ? "ok" : "error";
    }

    public void CargarUsuario(Usuario usuario)
    {
        _usuarioOriginal = usuario;

        IdUsuario = usuario.IdUsuario;
        NombreUsuario = usuario.NombreUsuario;
        ApellidosUsuario = usuario.ApellidosUsuario;
        UsernameDni = usuario.UsernameDni;
        EstadoUsuario = usuario.EstadoUsuario;
        RolSeleccionado = usuario.Rol;

        NuevaPassword = string.Empty;
        ConfirmarPassword = string.Empty;
        CambiarPassword = false;
        HasError = false;
        MensajeError = string.Empty;
    }

    [RelayCommand]
    public async Task GuardarAsync()
    {
        HasError = false;

        if (_usuarioOriginal != null &&
            _usuarioOriginal.Rol == Rol.SuperAdmin)
        {
            MostrarError("No se puede editar el Super Administrador.");
            return;
        }

        if (string.IsNullOrWhiteSpace(NombreUsuario) ||
            string.IsNullOrWhiteSpace(ApellidosUsuario) ||
            string.IsNullOrWhiteSpace(UsernameDni))
        {
            MostrarError("Los campos nombre, apellidos y DNI son obligatorios.");
            return;
        }

        if (CambiarPassword)
        {
            if (string.IsNullOrWhiteSpace(NuevaPassword))
            {
                MostrarError("Ingresa la nueva contraseña.");
                return;
            }

            if (NuevaPassword.Length < 6)
            {
                MostrarError("La contraseña debe tener al menos 6 caracteres.");
                return;
            }

            if (NuevaPassword != ConfirmarPassword)
            {
                MostrarError("Las contraseñas no coinciden.");
                return;
            }
        }

        IsLoading = true;

        try
        {
            string passwordFinal = _usuarioOriginal!.Password;

            if (CambiarPassword)
            {
                passwordFinal = BCrypt.Net.BCrypt.HashPassword(NuevaPassword);
            }

            var usuarioActualizado = new Usuario
            {
                IdUsuario = IdUsuario,
                NombreUsuario = NombreUsuario.Trim(),
                ApellidosUsuario = ApellidosUsuario.Trim(),
                UsernameDni = UsernameDni.Trim(),
                Password = passwordFinal,
                EstadoUsuario = EstadoUsuario,
                Rol = RolSeleccionado
            };

            await _repositoryUsuario.UpdateUsuarioAsync(usuarioActualizado);

            UsuarioActualizado?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            MostrarError($"Error al actualizar: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void MostrarError(string mensaje)
    {
        MensajeError = mensaje;
        HasError = true;
    }
}