using GymPos.ViewModels.UsuarioVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GymPos.Views.UsuarioPage
{
    public sealed partial class EditUsuarioPage : UserControl
    {
        private EditUsuarioViewModel? _editVM;

        public EditUsuarioPage()
        {
            InitializeComponent();
            // Obtener VM desde el contenedor para mantener coherencia con otras vistas
            _editVM = App.Services?.GetService<EditUsuarioViewModel>();
            if (_editVM != null)
                DataContext = _editVM;
        }

        // Configura la vista con los datos del VM proporcionado por el controlador de listas
        public void SetupForEdit(EditUsuarioViewModel vm)
        {
            _editVM = vm;
            DataContext = vm;

            TxtNombre.Text = vm.NombreUsuario;
            TxtApellidos.Text = vm.ApellidosUsuario;
            TxtDni.Text = vm.UsernameDni;
            TxtPassword.Password = string.Empty;
            TxtConfirmar.Password = string.Empty;
            UpdatePasswordMatchVisuals("none");
        }

        // Handlers separados: cada uno actualiza solo su propiedad
        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditUsuarioViewModel vm)
            {
                vm.NuevaPassword = TxtPassword.Password;
                UpdatePasswordMatchVisuals(vm.EstadoCoincidenciaPassword);
            }
        }

        private void TxtConfirmar_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditUsuarioViewModel vm)
            {
                vm.ConfirmarPassword = TxtConfirmar.Password;
                UpdatePasswordMatchVisuals(vm.EstadoCoincidenciaPassword);
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
}
