using GymPos.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
namespace GymPos.Views.VentasPage;

public sealed partial class ListVentasPage : Page
{
    public VentaViewModel ViewModel { get; }
    public ListVentasPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<VentaViewModel>();
        DataContext = this; 
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is int idCaja)
        {
            ViewModel.IdCajaActiva = idCaja;
        }

        await ViewModel.CargarDatosAsync();
    }
}
