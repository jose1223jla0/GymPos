using GymPos.Models;
using GymPos.ViewModels.UsuarioVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GymPos.Views.UsuarioPage;

public sealed partial class CreateEditUsuarioPage : UserControl
{
    private CreateUsuarioViewModel? _createVM;
    private EditUsuarioViewModel? _editVM;
    private bool _esEdicion;

    public CreateEditUsuarioPage()
    {
        InitializeComponent();
        _createVM = App.Services!.GetRequiredService<CreateUsuarioViewModel>();
        DataContext = _createVM;
        _esEdicion = false;
        ChkCambiarPassword.Visibility = Visibility.Collapsed;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Asegurar DataContext correcto para los bindings de Password
        if (DataContext == null && _createVM != null)
        {
            DataContext = _createVM;
        }
    }

    public void SetupForEdit(EditUsuarioViewModel editVM)
    {
        _esEdicion = true;
        _editVM = editVM;
        DataContext = _editVM;
        TxtNombre.Text = editVM.NombreUsuario;
        TxtApellidos.Text = editVM.ApellidosUsuario;
        TxtDni.Text = editVM.UsernameDni;
        TglEstado.IsOn = editVM.EstadoUsuario;
        CmbRol.SelectedIndex = editVM.RolSeleccionado == Rol.Administrador ? 0 : 1;
        ChkCambiarPassword.Visibility = Visibility.Visible;
        PanelPassword.Visibility = Visibility.Collapsed;
        PanelConfirmar.Visibility = Visibility.Collapsed;
    }

    public object? CurrentViewModel => DataContext;

    private void ChkCambiarPassword_Changed(object sender, RoutedEventArgs e)
    {
        if (ChkCambiarPassword.IsChecked == true)
        {
            PanelPassword.Visibility = Visibility.Visible;
            PanelConfirmar.Visibility = Visibility.Visible;
        }
        else
        {
            PanelPassword.Visibility = Visibility.Collapsed;
            PanelConfirmar.Visibility = Visibility.Collapsed;
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is CreateUsuarioViewModel cvm)
        {
            cvm.Password = TxtPassword.Password;
            cvm.ConfirmarPassword = TxtConfirmar.Password;
            UpdatePasswordMatchVisuals(cvm.EstadoCoincidenciaPassword);
        }
        else if (DataContext is EditUsuarioViewModel evm)
        {
            evm.NuevaPassword = TxtPassword.Password;
            evm.ConfirmarPassword = TxtConfirmar.Password;
            UpdatePasswordMatchVisuals(evm.EstadoCoincidenciaPassword);
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
