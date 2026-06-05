using GymPos.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
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

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel?.GetType();
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

    private async void OnRegistrarVentaClick(object sender, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            Title = "Confirmar venta",
            Content = $"¿Estás seguro de registrar la venta?\nTotal: S/ {ViewModel.TotalFormateado}",
            PrimaryButtonText = "Registrar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.RegistrarVentaAsync();

            if (ViewModel.VentaExitosa)
            {
                this.DispatcherQueue.TryEnqueue(() =>
                {
                    InfoBarNotificacion.Title = "Venta registrada";
                    InfoBarNotificacion.Message = "La venta se registró correctamente.";
                    InfoBarNotificacion.Severity = InfoBarSeverity.Success;
                    InfoBarNotificacion.IsOpen = true;
                });

                _ = System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(3000);
                    this.DispatcherQueue.TryEnqueue(() => InfoBarNotificacion.IsOpen = false);
                });
            }
        }
    }
}
