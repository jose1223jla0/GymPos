using GymPos.ViewModels.Asistencias;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GymPos.Views;
public sealed partial class AsistenciaPage : Page
{
    public AsistenciaPage()
    {
        InitializeComponent();
        DataContext = new AsistenciaViewModel();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        //FormDialog.XamlRoot = Content.XamlRoot;

        //var result = await FormDialog.ShowAsync();

        //if (result == ContentDialogResult.Primary)
        //{
        //    string nombre = NombreBox.Text;
        //    string email = EmailBox.Text;
        //    string pass = PasswordBox.Password;
        //}
    }
}
