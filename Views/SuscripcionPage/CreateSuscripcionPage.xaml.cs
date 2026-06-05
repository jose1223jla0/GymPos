using GymPos.Models;
using GymPos.ViewModels.ClienteVM;
using GymPos.ViewModels.SuscripcionesVM;
using GymPos.Views.ClientesPage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Threading.Tasks;

namespace GymPos.Views.SuscripcionPage;

public sealed partial class CreateSuscripcionPage : Page
{
    public CreateSuscripcionViewModel ViewModel { get; }
    private DispatcherTimer? _debounceTimer;
    private const int DebounceDelayMs = 400;

    public CreateSuscripcionPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<CreateSuscripcionViewModel>();
        DataContext = ViewModel;
        ViewModel.OnAbrirFormulario += MostrarModalNuevoCliente;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Membresia membresia)
            ViewModel.MembresiaSeleccionado = membresia;
    }

    // ── AutoSuggestBox ────────────────────────────────────────────────────────

    private void BuscadorDni_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
        {
            return;
        }
        var texto = sender.Text;
        ViewModel.TextoBusqueda = texto;
        _debounceTimer?.Stop();
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(DebounceDelayMs)
        };
        _debounceTimer.Tick += async (s, e) =>
        {
            _debounceTimer.Stop();
            await ViewModel.BuscarClientesAsync(texto);

            sender.ItemsSource = ViewModel.ClientesSugeridos;
        };
        _debounceTimer.Start();
    }

    private void BuscadorDni_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        if (args.SelectedItem is not Cliente cliente)
        {
            return;
        }
        _debounceTimer?.Stop();
        ViewModel.SeleccionarCliente(cliente);

        sender.Text = cliente.Dni;
    }

    private void BuscadorDni_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is Cliente cliente)
        {
            _debounceTimer?.Stop();
            ViewModel.SeleccionarCliente(cliente);
            sender.Text = cliente.Dni;
        }
    }

    // ── Botón Crear Suscripción ───────────────────────────────────────────────

    private async void BtnCrearSuscripcion_Click(object sender, RoutedEventArgs e)
    {
        void OnCreada()
        {
            // No-op aquí; la página de lista puede manejar la notificación si lo requiere.
        }
        ViewModel.OnSuscripcionCreada += OnCreada;
        await ViewModel.CreateSuscripcionCommand.ExecuteAsync(null);
        ViewModel.OnSuscripcionCreada -= OnCreada;
        Frame.Navigate(typeof(ListSuscripcionPage), true);
    }

    // ── Quitar cliente seleccionado ───────────────────────────────────────────

    private void QuitarClienteSeleccionado_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ClienteSeleccionado = null;
        ViewModel.TextoBusqueda = string.Empty;
        BuscadorDni.Text = string.Empty;
    }

    // ── Modal Nuevo Cliente ───────────────────────────────────────────────────

    private async void MostrarModalNuevoCliente()
    {
        var viewModel = App.Services!.GetRequiredService<CreateClienteViewModel>();

        if (!string.IsNullOrWhiteSpace(ViewModel.TextoBusqueda))
        {
            viewModel.DniCliente = ViewModel.TextoBusqueda;
        }

        var vista = new CreateClientePage
        {
            DataContext = viewModel,
            Width = 420,
            Height = 320,
            CornerRadius = new CornerRadius(16)
        };

        var dialog = new ContentDialog
        {
            Content = vista,
            PrimaryButtonText = "Guardar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = XamlRoot
        };
        viewModel.OnClienteCreado += (clienteCreado) =>
        {
            ViewModel.SeleccionarClienteNuevo(clienteCreado);
            DispatcherQueue.TryEnqueue(() =>
            {
                BuscadorDni.Text = clienteCreado.Dni;
                dialog.Hide();
                NotificationInfoBar.Title = "Cliente creado";
                NotificationInfoBar.Message = "El cliente se creó correctamente.";
                NotificationInfoBar.Severity = InfoBarSeverity.Success;
                NotificationInfoBar.IsOpen = true;
                _ =Task.Run(async () => { await Task.Delay(3000); this.DispatcherQueue.TryEnqueue(() => NotificationInfoBar.IsOpen = false); });
            });
        };
        dialog.PrimaryButtonClick += async (s, e) =>
        {
            e.Cancel = true;
            await viewModel.SaveClienteCommand.ExecuteAsync(null);
        };
        await dialog.ShowAsync();
    }
}