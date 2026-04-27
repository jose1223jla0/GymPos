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


    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var user = UserTextBox.Text;
        var pass = PasswordBox.Password;

        if (user == "admin" && pass == "1234")
        {
            ShowInfo("Login correcto", "Bienvenido al sistema", InfoBarSeverity.Success);
            await Task.Delay(1000);

            Frame.Navigate(typeof(MainWindow));
            AlertBar.IsOpen = false;
        }
        else
        {
            ShowInfo("Error", "Credenciales incorrectas", InfoBarSeverity.Error);

            await Task.Delay(1000);
            AlertBar.IsOpen = false;
        }
    }

    private void ShowInfo(string title, string message, InfoBarSeverity severity)
    {
        AlertBar.Title = title;
        AlertBar.Message = message;
        AlertBar.Severity = severity;
        AlertBar.IsOpen = true;
    }

    private void Hyperlink_Click(Hyperlink sender, HyperlinkClickEventArgs args)
    {

    }
}