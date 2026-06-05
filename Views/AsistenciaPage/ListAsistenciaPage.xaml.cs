using GymPos.Models;
using GymPos.ViewModels.Asistencias;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
namespace GymPos.Views.AsistenciaPage;

public sealed partial class ListAsistenciaPage : Page
{
    public AsistenciaViewModel ViewModel { get; }
    public ListAsistenciaPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<AsistenciaViewModel>();
        DataContext = ViewModel;
        Loaded+= AsistenciaListPage_Loaded;
    }

    public async void AsistenciaListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }

    private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        // El binding a ViewModel.SearchText ya disparará el filtrado via OnSearchTextChanged
    }

    private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // Forzar actualización por si el binding no se actualizó aún
        ViewModel.SearchText = sender.Text;
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var suscripcionVM = button!.DataContext as SuscripcionAsistenciaVM;

        if (suscripcionVM != null)
        {
            var dialog = new ContentDialog
            {
                Title = "Confirmar asistencia",
                Content = "¿Está seguro de marcar la asistencia?",
                PrimaryButtonText = "Aceptar",
                CloseButtonText = "Cancelar",
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                try
                {
                    await ViewModel.RegistrarAsistenciaCommand.ExecuteAsync(suscripcionVM.Suscripcion.IdSuscripcion);
                    // mostrar info bar de éxito
                    NotificationInfoBar.Title = "Asistencia registrada";
                    NotificationInfoBar.Message = "Se marcó la asistencia correctamente.";
                    NotificationInfoBar.Severity = InfoBarSeverity.Success;
                    NotificationInfoBar.IsOpen = true;
                }
                catch (Exception ex)
                {
                    NotificationInfoBar.Title = "Error";
                    NotificationInfoBar.Message = ex.Message;
                    NotificationInfoBar.Severity = InfoBarSeverity.Error;
                    NotificationInfoBar.IsOpen = true;
                }
                // ocultar automáticamente después de 3 segundos
                _ = Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    this.DispatcherQueue.TryEnqueue(() => NotificationInfoBar.IsOpen = false);
                });
            }
        }
    }
}
