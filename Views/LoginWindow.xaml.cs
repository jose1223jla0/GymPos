using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GymPos.Views
{
    public sealed partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            // Crear la página de login en tiempo de ejecución para evitar depender del inicializador generado por XAML
            var loginPage = new LoginPage();
            loginPage.LoginSucceeded += LoginPageControl_LoginSucceeded;
            this.Content = loginPage;
            this.Title = "Login";
        }

        private void LoginPageControl_LoginSucceeded(object? sender, EventArgs e)
        {
            // Abrir la ventana principal y cerrar la ventana de login
            var main = new MainWindow();
            main.Activate();
            this.Close();
        }
    }
}
