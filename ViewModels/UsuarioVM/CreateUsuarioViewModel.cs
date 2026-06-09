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
    // Validaciones en tiempo real
    [ObservableProperty] private bool nombreValido = false;
    [ObservableProperty] private string nombreMensaje = string.Empty;
    [ObservableProperty] private bool apellidosValido = false;
    [ObservableProperty] private string apellidosMensaje = string.Empty;
    [ObservableProperty] private bool usernameDniValido = false;
    [ObservableProperty] private string usernameDniMensaje = string.Empty;
    [ObservableProperty] private bool passwordValido = false;
    [ObservableProperty] private string passwordMensaje = string.Empty;
    [ObservableProperty] private bool puedeGuardar = false;
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

    partial void OnNombreUsuarioChanged(string value)
    {
        ValidateNombre(value);
    }

    partial void OnApellidosUsuarioChanged(string value)
    {
        ValidateApellidos(value);
    }

    partial  void OnUsernameDniChanged(string value)
    {
        // lanzar validación asíncrona para chequear duplicados
        _ = ValidateUsernameDniAsync(value);
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

        // validación flexible: mínimo 4 caracteres
        if (string.IsNullOrEmpty(Password))
        {
            PasswordValido = false;
            PasswordMensaje = "La contraseña es obligatoria.";
        }
        else if (Password.Length < 4)
        {
            PasswordValido = false;
            PasswordMensaje = "La contraseña debe tener al menos 4 caracteres.";
        }
        else
        {
            PasswordValido = true;
            PasswordMensaje = string.Empty;
        }

        UpdateCanSave();
    }

    /// <summary>
    /// Valida los datos ingresados, muestra mensajes de error si es necesario, y si todo es correcto, crea un nuevo usuario en la base de datos.
    /// </summary>

    [RelayCommand]
    public async Task GuardarAsync()
    {
        HasError = false;
        // Validaciones finales (usar las reglas en tiempo real)
        if (!NombreValido || !ApellidosValido || !UsernameDniValido || !PasswordValido)
        {
            MostrarError("Revisa los campos del formulario. Hay validaciones pendientes o erróneas.");
            return;
        }

        if (EstadoCoincidenciaPassword != "ok")
        {
            MostrarError("Las contraseñas no coinciden.");
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

    // Validaciones en tiempo real
    private void ValidateNombre(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 4)
        {
            NombreValido = false;
            NombreMensaje = "El nombre debe tener al menos 4 caracteres.";
        }
        else
        {
            NombreValido = true;
            NombreMensaje = string.Empty;
        }
    }

    private void ValidateApellidos(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 4)
        {
            ApellidosValido = false;
            ApellidosMensaje = "Los apellidos deben tener al menos 4 caracteres.";
        }
        else
        {
            ApellidosValido = true;
            ApellidosMensaje = string.Empty;
        }
    }

    private async Task ValidateUsernameDniAsync(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length < 4)
        {
            UsernameDniValido = false;
            UsernameDniMensaje = "El DNI/Usuario debe tener al menos 4 caracteres.";
            return;
        }

        try
        {
            var existente = await _repositoryUsuario.GetUsuarioByUsernameDniAsync(value.Trim());
            if (existente != null)
            {
                UsernameDniValido = false;
                UsernameDniMensaje = "El DNI/Usuario ya está registrado.";
            }
            else
            {
                UsernameDniValido = true;
                UsernameDniMensaje = string.Empty;
            }
        }
        catch
        {
            UsernameDniValido = false;
            UsernameDniMensaje = "Error al validar el DNI/Usuario.";
        }
        UpdateCanSave();
    }

    private void UpdateCanSave()
    {
        PuedeGuardar = NombreValido && ApellidosValido && UsernameDniValido && PasswordValido && EstadoCoincidenciaPassword == "ok";
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
