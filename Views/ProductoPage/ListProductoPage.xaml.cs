using GymPos.Models;
using GymPos.Services;
using GymPos.ViewModels.ProductoVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GymPos.Views.ProductoPage;

public sealed partial class ListProductoPage : Page
{
    public ListProductoViewModel ViewModel { get; }
    public EditProductoViewModel EditViewModel { get; }
    private readonly INotificationService _notificationService;
    private DispatcherTimer? _notificationTimer;

    public ListProductoPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ListProductoViewModel>();
        EditViewModel = App.Services!.GetRequiredService<EditProductoViewModel>();
        _notificationService = App.Services!.GetRequiredService<INotificationService>();

        ViewModel.OpenEditDialogRequest += OnOpenEditDialog;
        ViewModel.OpenAddDialogRequest += OnOpenAddDialog;
        _notificationService.NotificationRequested += OnNotificationRequested;

        Loaded += async (s, e) => await ViewModel.InitAsync();
    }

    // ---- Diálogo EDITAR ----
    private async void OnOpenEditDialog(Producto producto)
    {
        await EditViewModel.CargarParaEditar(producto);
        DialogTitulo.Text = "Editar Producto";
        ProductoDialog.XamlRoot = this.XamlRoot;
        ProductoDialog.Title = string.Empty;
        await ProductoDialog.ShowAsync();
    }

    // ---- Diálogo AGREGAR ----
    private async void OnOpenAddDialog()
    {
        await EditViewModel.CargarParaAgregar();
        DialogTitulo.Text = "Agregar Producto";
        ProductoDialog.XamlRoot = this.XamlRoot;
        ProductoDialog.Title = string.Empty;
        await ProductoDialog.ShowAsync();
    }

    // ---- Guardar desde el diálogo ----
    private async void ProductoDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        // Defer para poder cancelar el cierre si hay error
        var deferral = args.GetDeferral();
        try
        {
            await EditViewModel.GuardarProductoCommand.ExecuteAsync(null);
            await ViewModel.LoadProductos();
        }
        catch
        {
            // El ViewModel ya mostró la notificación de error; no cerramos el diálogo
            args.Cancel = true;
        }
        finally
        {
            deferral.Complete();
        }
    }

    // ---- Toast de notificaciones ----
    private void OnNotificationRequested(object? sender, Notification notification)
    {
        _notificationTimer?.Stop();
        _notificationTimer = null;

        TitleTextBlock.Text = notification.Title;
        MessageTextBlock.Text = notification.Message;

        // Color del ícono según tipo
        IconContainer.Background = notification.Type switch
        {
            NotificationType.Success => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.SeaGreen),
            NotificationType.Error => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Crimson),
            NotificationType.Warning => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DarkOrange),
            _ => new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 0, 120, 212))
        };
        NotificationIcon.Glyph = notification.Type switch
        {
            NotificationType.Success => "\uE73E",
            NotificationType.Error => "\uEA39",
            NotificationType.Warning => "\uE7BA",
            _ => "\uE946"
        };

        NotificationToastBorder.Visibility = Visibility.Visible;

        _notificationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(notification.DurationSeconds)
        };
        _notificationTimer.Tick += (s, e) =>
        {
            NotificationToastBorder.Visibility = Visibility.Collapsed;
            _notificationTimer?.Stop();
        };
        _notificationTimer.Start();
    }

    private void CloseToast_Click(object sender, RoutedEventArgs e)
    {
        NotificationToastBorder.Visibility = Visibility.Collapsed;
        _notificationTimer?.Stop();
    }
}