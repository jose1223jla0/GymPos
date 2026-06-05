using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.ViewModels.UsuarioVM;

public partial class CreateUsuarioViewModel : ObservableObject
{
    private readonly IRepositoryUsuario _repositoryUsuario;

    [ObservableProperty] private string nombreUsuario = string.Empty;
    [ObservableProperty] private string apellidosUsuario = string.Empty;
    [ObservableProperty] private string usernameDni = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmarPassword = string.Empty;
    [ObservableProperty] private string estadoCoincidenciaPassword = "none";
    [ObservableProperty] private bool estadoUsuario = true;
    [ObservableProperty] private Rol rolSeleccionado = Rol.Recepcionista;
    [ObservableProperty] private bool isLoading = false;
    [ObservableProperty] private string mensajeError = string.Empty;
    [ObservableProperty] private bool hasError = false;

    public List<Rol> Roles { get; } = new() { Rol.Administrador, Rol.Recepcionista };

    public event EventHandler? UsuarioCreado;

    public CreateUsuarioViewModel(IRepositoryUsuario repositoryUsuario)
    {
        _repositoryUsuario = repositoryUsuario;
    }

    partial void OnPasswordChanged(string value)
    {
        UpdatePasswordMatch();
    }

    partial void OnConfirmarPasswordChanged(string value)
    {
        UpdatePasswordMatch();
    }

    private void UpdatePasswordMatch()
    {
        if (string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(ConfirmarPassword))
        {
            EstadoCoincidenciaPassword = "none";
            return;
        }

        if (Password == ConfirmarPassword)
        {
            EstadoCoincidenciaPassword = "ok";
        }
        else
        {
            EstadoCoincidenciaPassword = "error";
        }
    }

    /// <summary>
    /// Valida los datos ingresados, muestra mensajes de error si es necesario, y si todo es correcto, crea un nuevo usuario en la base de datos.
    /// </summary>

    [RelayCommand]
    public async Task GuardarAsync()
    {
        HasError = false;

        if (string.IsNullOrWhiteSpace(NombreUsuario) || string.IsNullOrWhiteSpace(ApellidosUsuario) ||
            string.IsNullOrWhiteSpace(UsernameDni) || string.IsNullOrWhiteSpace(Password))
        {
            MostrarError("Todos los campos son obligatorios.");
            return;
        }

        if (Password != ConfirmarPassword)
        {
            MostrarError("Las contraseñas no coinciden.");
            return;
        }

        if (Password.Length < 6)
        {
            MostrarError("La contraseña debe tener al menos 6 caracteres.");
            return;
        }

        var existente = await _repositoryUsuario.GetUsuarioByUsernameDniAsync(UsernameDni.Trim());
        if (existente != null)
        {
            MostrarError("El DNI/Usuario ya está registrado.");
            return;
        }

        IsLoading = true;
        try
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

            var usuario = new Usuario
            {
                NombreUsuario = NombreUsuario.Trim(),
                ApellidosUsuario = ApellidosUsuario.Trim(),
                UsernameDni = UsernameDni.Trim(),
                Password = passwordHash,
                EstadoUsuario = EstadoUsuario,
                Rol = RolSeleccionado
            };

            await _repositoryUsuario.AddUsuarioAsync(usuario);
            UsuarioCreado?.Invoke(this, EventArgs.Empty);
            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            MostrarError($"Error al guardar: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Muestra un mensaje de error en la interfaz y activa la bandera de error.
    /// </summary>
    private void MostrarError(string mensaje)
    {
        MensajeError = mensaje;
        HasError = true;
    }
    /// <summary>
    /// Limpia los campos del formulario y restablece el estado a los valores predeterminados.
    /// </summary>
    private void LimpiarFormulario()
    {
        NombreUsuario = string.Empty;
        ApellidosUsuario = string.Empty;
        UsernameDni = string.Empty;
        Password = string.Empty;
        ConfirmarPassword = string.Empty;
        EstadoUsuario = true;
        RolSeleccionado = Rol.Recepcionista;
        HasError = false;
    }
}
