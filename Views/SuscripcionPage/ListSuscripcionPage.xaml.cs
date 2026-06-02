using GymPos.ViewModels.SuscripcionesVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace GymPos.Views.SuscripcionPage;
public sealed partial class ListSuscripcionPage : Page
{
    public ListSuscripcionViewModel ViewModel;
    public ListSuscripcionPage()
    {
        InitializeComponent();
        ViewModel=App.Services!.GetRequiredService<ListSuscripcionViewModel>();
        DataContext = ViewModel;
        Loaded += SuscripcionListPage_Loaded;
    }

    /// <summary>
    /// Inicializa la vista cuando se carga la página y ejecuta de forma asíncrona ViewModel.InitAsync. 
    /// </summary>
    /// <remarks>Manejador de eventos asincrónico; async void debe usarse únicamente para controladores de
    /// eventos.</remarks>
    /// <param name="sender">Origen del evento, normalmente la página o el control que desencadena la carga.</param>
    /// <param name="e">Argumentos del evento de enrutamiento.</param>
    public async void SuscripcionListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is bool created && created)
        {
            NotificationInfoBar.Title = "Suscripción creada";
            NotificationInfoBar.Message = "La suscripción se creó correctamente.";
            NotificationInfoBar.Severity = InfoBarSeverity.Success;
            NotificationInfoBar.IsOpen = true;
            _ = Task.Run(async () => { await Task.Delay(3000); this.DispatcherQueue.TryEnqueue(() => NotificationInfoBar.IsOpen = false); });
        }
    }
}
