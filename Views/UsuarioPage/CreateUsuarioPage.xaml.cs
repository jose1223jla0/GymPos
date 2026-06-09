using GymPos.ViewModels.UsuarioVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Text.RegularExpressions;

namespace GymPos.Views.UsuarioPage;

public sealed partial class CreateUsuarioPage : UserControl
{
    private CreateUsuarioViewModel? _createVM;

    public CreateUsuarioPage()
    {
        InitializeComponent();
        _createVM = App.Services!.GetRequiredService<CreateUsuarioViewModel>();
        DataContext = _createVM;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is CreateUsuarioViewModel cvm)
        {
            var pwd = TxtPassword.Password;
            var conf = TxtConfirmar.Password;
            cvm.Password = pwd;
            cvm.ConfirmarPassword = conf;
            UpdatePasswordMatchVisuals(cvm.EstadoCoincidenciaPassword);
        }
    }

    private void TxtNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is CreateUsuarioViewModel cvm)
        {
            cvm.NombreUsuario = TxtNombre.Text;
        }
    }

    private void TxtApellidos_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is CreateUsuarioViewModel cvm)
        {
            cvm.ApellidosUsuario = TxtApellidos.Text;
        }
    }

    private void TxtDni_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
    {
        // Solo números
        if (!Regex.IsMatch(args.NewText, @"^\d*$"))
        {
            args.Cancel = true;
        }

        // Máximo 8 dígitos
        if (args.NewText.Length > 8)
        {
            args.Cancel = true;
        }
    }

    private void UpdatePasswordMatchVisuals(string estado)
    {
        switch (estado)
        {
            case "ok":
                TxtCoincide.Visibility = Visibility.Visible;
                TxtNoCoincide.Visibility = Visibility.Collapsed;
                break;
            case "error":
                TxtCoincide.Visibility = Visibility.Collapsed;
                TxtNoCoincide.Visibility = Visibility.Visible;
                break;
            default:
                TxtCoincide.Visibility = Visibility.Collapsed;
                TxtNoCoincide.Visibility = Visibility.Collapsed;
                break;
        }
    }
}


