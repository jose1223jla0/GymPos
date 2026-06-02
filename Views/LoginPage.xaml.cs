using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GymPos.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }

    //// Evento que notifica que el login fue exitoso
    public event EventHandler? LoginSucceeded;


    //private async void Button_Click(object sender, RoutedEventArgs e)
    //{
    //    var user = UserTextBox.Text;
    //    var pass = PasswordBox.Password;

    //    if (user == "admin" && pass == "1234")
    //    {
    //        ShowInfo("Login correcto", "Bienvenido al sistema", InfoBarSeverity.Success);
    //        await Task.Delay(1000);

    //        // Notificar al contenedor (ventana de login) que el login fue correcto
    //        AlertBar.IsOpen = false;
    //        LoginSucceeded?.Invoke(this, EventArgs.Empty);
    //    }
    //    else
    //    {
    //        ShowInfo("Error", "Credenciales incorrectas", InfoBarSeverity.Error);

    //        await Task.Delay(1000);
    //        AlertBar.IsOpen = false;
    //    }
    //}

    //private void ShowInfo(string title, string message, InfoBarSeverity severity)
    //{
    //    AlertBar.Title = title;
    //    AlertBar.Message = message;
    //    AlertBar.Severity = severity;
    //    AlertBar.IsOpen = true;
    //}

    //private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    //{

    //}
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        ValidarLogin();
    }

    private void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            ValidarLogin();
    }

    private void UserTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            ValidarLogin();
    }

    private void ValidarLogin()
    {
        string usuario = UserTextBox.Text.Trim();
        string password = PasswordBox.Password;

        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(password))
        {
            AlertBar.Title = "Campos requeridos";
            AlertBar.Message = "Por favor ingresa tu usuario y contraseña.";
            AlertBar.Severity = InfoBarSeverity.Warning;
            AlertBar.IsOpen = true;
            return;
        }

        // ─────────────────────────────────────────────
        // TODO: Reemplaza con tu lógica real de autenticación
        // ─────────────────────────────────────────────
        bool credencialesValidas = usuario == "admin" && password == "1234";

        if (credencialesValidas)
        {
            AlertBar.IsOpen = false;
            // Notificar al contenedor (ventana de login) que el login fue correcto
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            AlertBar.Title = "Acceso denegado";
            AlertBar.Message = "Usuario o contraseña incorrectos. Intenta nuevamente.";
            AlertBar.Severity = InfoBarSeverity.Error;
            AlertBar.IsOpen = true;
            PasswordBox.Password = string.Empty;
            PasswordBox.Focus(FocusState.Programmatic);
        }
    }

    private async void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Recuperar contraseña",
            Content = "Contacta al administrador del sistema para restablecer tu contraseña.",
            CloseButtonText = "Entendido",
            XamlRoot = XamlRoot
        };
        await dialog.ShowAsync();
    }

}