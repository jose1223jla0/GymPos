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
namespace GymPos;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer == null)
        {
            return;
        }

        string? tag = args.SelectedItemContainer.Tag.ToString();

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
        }
    }
}
