using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Services;
using System;
using System.Threading.Tasks;
using static GymPos.Services.AuthService;

namespace GymPos.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string usernameDni = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError = false;

    public event EventHandler? LoginExitoso;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoginAsync(string password)
    {
        if (string.IsNullOrWhiteSpace(UsernameDni) || string.IsNullOrWhiteSpace(password))
        {
            MostrarError("Por favor ingresa tu usuario y contraseña.");
            return;
        }

        IsLoading = true;
        HasError = false;

        try
        {
            var resultado = await _authService.LoginAsync(UsernameDni.Trim(), password);
            switch (resultado)
            {
                case LoginResult.Success:
                    LoginExitoso?.Invoke(this, EventArgs.Empty);
                    break;
                case LoginResult.Inactive:
                    MostrarError("La cuenta está desactivada. Contacta al administrador.");
                    break;
                case LoginResult.UserNotFound:
                case LoginResult.InvalidPassword:
                default:
                    MostrarError("Usuario o contraseña incorrectos.");
                    break;
            }
        }
        catch (Exception)
        {
            MostrarError("Error al conectar con la base de datos. Verifica la conexión.");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void MostrarError(string mensaje)
    {
        ErrorMessage = mensaje;
        HasError = true;
    }
}
