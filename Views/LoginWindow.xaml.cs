using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GymPos.Views
{
    public sealed partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            var loginPage = new LoginPage();

            loginPage.LoginSucceeded += LoginPageControl_LoginSucceeded;

            Content = loginPage;

            Title = "Login";
        }

        private void LoginPageControl_LoginSucceeded(object? sender, EventArgs e)
        {
            var mainWindow = new MainWindow();

            App.MainAppWindow = mainWindow;

            mainWindow.Activate();

            Close();
        }
    }
}
