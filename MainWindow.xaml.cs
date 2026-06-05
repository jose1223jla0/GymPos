using GymPos.Models;
using GymPos.Views;
using GymPos.Views.AsistenciaPage;
using GymPos.Views.CajaPage;
using GymPos.Views.ClientesPage;
using GymPos.Views.MembresiaPage;
using GymPos.Views.ProductoPage;
using GymPos.Views.SuscripcionPage;
using GymPos.Views.UsuarioPage;
using GymPos.Views.VentasPage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;

namespace GymPos;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CargarUsuarioHeader();
        AplicarRestriccionesPorRol();
        // La lógica de notificación de cierre de caja se maneja en ResumenCajaPage.
        // No suscribimos globalmente aquí para evitar cerrar sesión automáticamente al cerrar la caja.
    }

    private async void OnCajaCerrada()
    {
        this.DispatcherQueue.TryEnqueue(() => contentFrame.Navigate(typeof(Views.PerfilPage.PerfilUserPage)));
        await Task.Delay(1200);
        var auth = App.Services!.GetService(typeof(Services.IAuthService)) as Services.IAuthService;
        auth?.Logout();
        var login = new Views.LoginWindow();
        login.Activate();
        this.Close();
    }

    private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer == null)
        {
            return;
        }
        string? tag = args.SelectedItemContainer.Tag?.ToString();
        if (string.IsNullOrEmpty(tag))
        {
            return;
        }

        switch (tag)
        {
            case "SamplePage1":
                contentFrame.Navigate(typeof(DashboardPage));
                break;

            case "SamplePage2":
                contentFrame.Navigate(typeof(ListClientePage));
                break;

            case "SamplePage3":
                contentFrame.Navigate(typeof(ListSuscripcionPage));
                break;

            case "SamplePage4":
                contentFrame.Navigate(typeof(MembresiaPage));
                break;

            case "SamplePage5":
                contentFrame.Navigate(typeof(ListAsistenciaPage));
                break;

            case "SamplePage6":
                contentFrame.Navigate(typeof(ResumenCajaPage));
                break;

            case "SamplePage7":
                contentFrame.Navigate(typeof(ListVentasPage));
                break;

            case "SamplePage8":
                contentFrame.Navigate(typeof(ListProductoPage));
                break;

            case "SamplePage9":
                contentFrame.Navigate(typeof(ListUsuarioPage));
                break;

            case "SamplePage10":
                contentFrame.Navigate(typeof(ListMembresiaPage));
                break;
        }
    }

    private void AplicarRestriccionesPorRol()
    {
        var auth = App.Services!.GetService(typeof(Services.IAuthService)) as Services.IAuthService;

        if (auth == null)
        {
            return;
        }

        if (auth.EsSuperAdmin)
        {
            nvAsistencia.Visibility = Visibility.Collapsed;
            nvCaja.Visibility = Visibility.Collapsed;
            nvMembresias.Visibility = Visibility.Collapsed;
            nvVentas.Visibility = Visibility.Collapsed;
        }

        if(auth.EsAdministrador)
        {
            nvUsuarios.Visibility = Visibility.Visible;
            nvDashboard.Visibility = Visibility.Visible;
        }

        // Recepcionista solo puede acceder a: Caja, Ventas, Subscripcion, Asistencia
        if (auth.EsRecepcionista)
        {
            nvGestionarMembresia.Visibility = Visibility.Collapsed;
            nvClientes.Visibility = Visibility.Collapsed;
            nvProductos.Visibility = Visibility.Collapsed;
            nvUsuarios.Visibility = Visibility.Collapsed;
        }
    }

    private void CargarUsuarioHeader()
    {
        var auth = App.Services!.GetService(typeof(Services.IAuthService)) as Services.IAuthService;
        var u = auth?.UsuarioActual;
        if (u != null)
        {
            HeaderUserName.Text = $"{u.NombreUsuario} {u.ApellidosUsuario}";

            HeaderUserRole.Text = u.Rol switch
            {
                Rol.SuperAdmin => "Super Administrador",
                Rol.Administrador => "Administrador",
                Rol.Recepcionista => "Recepcionista",
                _ => "Sin rol"
            };
        }
    }

    public void BtnPerfil_Click(object sender, RoutedEventArgs e)
    {
        contentFrame.Navigate(typeof(Views.PerfilPage.PerfilUserPage));
    }

    public void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        var auth = App.Services!.GetService(typeof(Services.IAuthService)) as Services.IAuthService;
        auth?.Logout();
        var login = new LoginWindow();
        login.Activate();
        Close();
    }
}