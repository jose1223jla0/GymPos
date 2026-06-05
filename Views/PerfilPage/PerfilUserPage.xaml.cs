using GymPos.Models;
using GymPos.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
namespace GymPos.Views.PerfilPage;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PerfilUserPage : Page
{
    public event EventHandler? SesionCerrada;
    public PerfilUserPage()
    {
        InitializeComponent();
        CargarPerfil();
    }

    private void CargarPerfil()
    {
        var authService = App.Services!.GetService(typeof(IAuthService)) as IAuthService;
        var usuario = authService?.UsuarioActual;

        if (usuario == null) return;

        TxtNombreCompleto.Text = $"{usuario.NombreUsuario} {usuario.ApellidosUsuario}";
        TxtRol.Text = usuario.Rol switch
        {
            Rol.SuperAdmin => "Super Administrador",
            Rol.Administrador => "Administrador",
            Rol.Recepcionista => "Recepcionista",
            _ => "Sin rol"
        };
        TxtDni.Text = usuario.UsernameDni;
        TxtNombre.Text = usuario.NombreUsuario;
        TxtApellidos.Text = usuario.ApellidosUsuario;
        TxtEstado.Text = usuario.EstadoUsuario ? "Activo" : "Inactivo";
        // Color del indicador de estado
        EstadoDot.Fill = usuario.EstadoUsuario
            ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.SeaGreen)
            : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.IndianRed);

        TxtEstado.Foreground = usuario.EstadoUsuario
            ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.SeaGreen)
            : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.IndianRed);
    }

    private async void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Cerrar sesión",
            Content = "¿Deseas cerrar tu sesión actual?",
            PrimaryButtonText = "Cerrar sesión",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = XamlRoot
        };
        var result = await dialog.ShowAsync();

        if (result != ContentDialogResult.Primary)
        {
            return;
        }
        var authService = App.Services!.GetService(typeof(IAuthService)) as IAuthService;
        authService?.Logout();
        var loginWindow = new LoginWindow();
        loginWindow.Activate();
        App.MainAppWindow?.Close();
        App.MainAppWindow = null;
    }
}
