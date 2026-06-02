using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace GymPos.Views.VentasPage
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateVentaPage : Page
    {
        public CreateVentaPage()
        {
            InitializeComponent();
        // Si existe un ViewModel con evento VentaCreada, suscribirse para mostrar notificación
        if (App.Services?.GetService(typeof(ViewModels.VentaVM.CreateVentaViewModel)) is ViewModels.VentaVM.CreateVentaViewModel vm)
        {
            vm.VentaCreada += OnVentaCreada;
        }
        }

    private void OnVentaCreada()
    {
        this.DispatcherQueue.TryEnqueue(() =>
        {
            NotificationInfoBar.Title = "Venta registrada";
            NotificationInfoBar.Message = "La venta se registró correctamente.";
            NotificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
            NotificationInfoBar.IsOpen = true;
        });
        _ = Task.Run(async () => { await Task.Delay(3000); this.DispatcherQueue.TryEnqueue(() => NotificationInfoBar.IsOpen = false); });
    }
    }
}
