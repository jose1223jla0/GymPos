using GymPos.ViewModels.CajaVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.UI;
using System.Threading.Tasks;

namespace GymPos.Views.CajaPage;

public sealed partial class ResumenCajaPage : Page
{
    public ResumenCajaViewModel ViewModel { get; }
    private readonly Services.ICajaEventService? _cajaEventService;

    public ResumenCajaPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ResumenCajaViewModel>();
        _cajaEventService = App.Services!.GetService(typeof(Services.ICajaEventService)) as Services.ICajaEventService;
    }

    // ────────── NAVEGACIÓN ──────────

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
        if (_cajaEventService != null)
        {
            _cajaEventService.CajaAperturada += OnCajaAperturada;
            _cajaEventService.CajaCerrada += OnCajaCerrada;
        }
        await ViewModel.CargarCajaActivaAsync();
        ActualizarBadge();
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        ViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        if (_cajaEventService != null)
        {
            _cajaEventService.CajaAperturada -= OnCajaAperturada;
            _cajaEventService.CajaCerrada -= OnCajaCerrada;
        }
    }

    // ────────── SUSCRIPCIÓN A CAMBIOS ──────────

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.CajaAbierta))
        {
            ActualizarBadge();
        }
    }

    // ────────── BADGE DE ESTADO ──────────
    private void ActualizarBadge()
    {
        bool abierta = ViewModel.CajaAbierta;
        StatusText.Text = abierta ? "● Abierta" : "● Cerrada";
        StatusText.Foreground = new SolidColorBrush(abierta ? Color.FromArgb(255, 16, 124, 16) : Color.FromArgb(255, 197, 15, 31));
        StatusBadge.Background = new SolidColorBrush(abierta ? Color.FromArgb(20, 16, 124, 16) : Color.FromArgb(20, 197, 15, 31));
    }

    // ────────── ACCIONES ──────────
    private async void OnAbrirCajaClick(object sender, RoutedEventArgs e)
    {
        var fechaApertura = DateTime.Now;
        var montoBox = new NumberBox
        {
            PlaceholderText = "0.00",
            Minimum = 0,
            SpinButtonPlacementMode = NumberBoxSpinButtonPlacementMode.Hidden,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var fechaText = new TextBlock
        {
            Text = fechaApertura.ToString("dddd, d 'de' MMMM yyyy", new System.Globalization.CultureInfo("es-PE")),
            FontSize = 13,
            FontWeight = FontWeights.SemiBold
        };

        var horaText = new TextBlock
        {
            Text = fechaApertura.ToString("hh:mm tt"),
            FontSize = 13,
            FontWeight = FontWeights.SemiBold
        };

        var contenido = new StackPanel { Spacing = 16 };

        var grid = new Grid { ColumnSpacing = 10 };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var cardFecha = CrearCard("Fecha de apertura", fechaText);
        var cardHora = CrearCard("Hora de apertura", horaText);

        Grid.SetColumn(cardFecha, 0);
        Grid.SetColumn(cardHora, 1);

        grid.Children.Add(cardFecha);
        grid.Children.Add(cardHora);

        contenido.Children.Add(grid);

        contenido.Children.Add(new StackPanel
        {
            Spacing = 4,
            Children =
        {
            new TextBlock
            {
                Text = "Monto inicial",
                FontSize = 12,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            },
            montoBox,
            new TextBlock
            {
                Text = "Dinero físico en caja al inicio del turno",
                FontSize = 11,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            }
        }
        });

        var dialog = new ContentDialog
        {
            Title = "Aperturar caja",
            Content = contenido,
            PrimaryButtonText = "Aperturar",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            decimal monto = double.IsNaN(montoBox.Value) ? 0 : (decimal)montoBox.Value;
            await ViewModel.AbrirCajaAsync(monto);
            ActualizarBadge();
        }
    }

    private async void OnCajaAperturada()
    {
        // Recargar estado real del resumen
        await ViewModel.CargarCajaActivaAsync();
        this.DispatcherQueue.TryEnqueue(() => ActualizarBadge());

        this.DispatcherQueue.TryEnqueue(() =>
        {
            NotificationInfoBar.Title = "Caja aperturada";
            NotificationInfoBar.Message = "Se ha aperturado la caja de manera exitosa.";
            NotificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success;
            NotificationInfoBar.IsOpen = true;
        });

        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            this.DispatcherQueue.TryEnqueue(() => NotificationInfoBar.IsOpen = false);
        });
    }

    private async void OnCajaCerrada()
    {
        // Al cerrar la caja recargar el resumen (queda sin datos)
        await ViewModel.CargarCajaActivaAsync();
        this.DispatcherQueue.TryEnqueue(() => ActualizarBadge());

        this.DispatcherQueue.TryEnqueue(() =>
        {
            NotificationInfoBar.Title = "Caja cerrada";
            NotificationInfoBar.Message = "La caja se ha cerrado correctamente.";
            NotificationInfoBar.Severity = Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
            NotificationInfoBar.IsOpen = true;
        });

        _ = Task.Run(async () =>
        {
            await Task.Delay(2000);
            this.DispatcherQueue.TryEnqueue(() => NotificationInfoBar.IsOpen = false);
        });
    }

    // Helper para las tarjetas de fecha/hora
    private static Border CrearCard(string label, UIElement valor)
    {
        return new Border
        {
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(12, 10, 12, 10),
            Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
            Child = new StackPanel
            {
                Spacing = 2,
                Children =
                {
                    new TextBlock
                    {
                        Text = label,
                        FontSize = 11,
                        Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
                    },
                    valor
                }
            }
        };
    }

    private async void OnCerrarCajaClick(object sender, RoutedEventArgs e)
    {
        // Confirmar cierre de caja
        var dialog = new ContentDialog
        {
            Title = "Cerrar caja",
            Content = $"¿Confirmas el cierre de la caja?\nSaldo actual: {ViewModel.SaldoActualStr}",
            PrimaryButtonText = "Cerrar caja",
            CloseButtonText = "Cancelar",
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = this.XamlRoot
        };

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.CerrarCajaAsync();
            ActualizarBadge();
        }
    }
}