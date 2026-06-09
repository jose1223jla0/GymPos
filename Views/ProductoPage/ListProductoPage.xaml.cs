using GymPos.Models;
using GymPos.Services;
using GymPos.ViewModels.ProductoVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;

namespace GymPos.Views.ProductoPage;

public sealed partial class ListProductoPage : Page
{
    // ── ViewModels ────────────────────────────────────────────────────────────
    public ListProductoViewModel ViewModel { get; }
    public EditProductoViewModel EditViewModel { get; }
    public CreateProductoViewModel CreateViewModel { get; }

    // ── Servicios ─────────────────────────────────────────────────────────────
    private readonly INotificationService _notificationService;
    private DispatcherTimer? _notificationTimer;

    // ── Constructor ───────────────────────────────────────────────────────────
    public ListProductoPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ListProductoViewModel>();
        EditViewModel = App.Services!.GetRequiredService<EditProductoViewModel>();
        CreateViewModel = App.Services!.GetRequiredService<CreateProductoViewModel>();
        _notificationService = App.Services!.GetRequiredService<INotificationService>();
        DataContext = ViewModel;
        ViewModel.OpenEditDialogRequest += OnOpenEditDialog;
        ViewModel.OpenAddDialogRequest += OnOpenAddDialog;
        _notificationService.NotificationRequested += OnNotificationRequested;
        CreateViewModel.ProductoCreado += OnProductoCreado;
        Loaded += async (_, _) => await ViewModel.InitAsync();
    }

    // ── Diálogo EDITAR ────────────────────────────────────────────────────────
    private async void OnOpenEditDialog(Producto producto)
    {
        await EditViewModel.CargarParaEditar(producto);

        if (FindName("EditProductoEditor") is EditProductoPage editor)
            editor.SetupForEdit(EditViewModel);

        EditViewModel.ProductoActualizado -= OnProductoActualizado;
        EditViewModel.ProductoActualizado += OnProductoActualizado;

        if (FindName("EditProductoDialog") is ContentDialog dialog)
        {
            dialog.XamlRoot = XamlRoot;
            await dialog.ShowAsync();
        }
    }

    private async void OnProductoActualizado(object? sender, EventArgs e)
    {
        if (FindName("EditProductoDialog") is ContentDialog dialog)
            dialog.Hide();

        EditViewModel.ProductoActualizado -= OnProductoActualizado;
        await ViewModel.LoadProductos();
    }

    // ── Diálogo AGREGAR ───────────────────────────────────────────────────────
    private async void OnOpenAddDialog()
    {
        await CreateViewModel.CargarParaAgregar();

        if (FindName("CreateProductoEditor") is CreateProductoPage editor)
            editor.DataContext = CreateViewModel;

        if (FindName("CreateProductoDialog") is ContentDialog dialog)
        {
            dialog.XamlRoot = XamlRoot;
            await dialog.ShowAsync();
        }
    }

    private async void OnProductoCreado(object? sender, EventArgs e)
    {
        if (FindName("CreateProductoDialog") is ContentDialog dialog)
            dialog.Hide();

        await ViewModel.LoadProductos();
    }

    // ── Guardar desde el diálogo de edición ───────────────────────────────────
    private async void EditProductoDialog_PrimaryButtonClick(
        ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        try
        {
            await EditViewModel.GuardarProductoCommand.ExecuteAsync(null);
            await ViewModel.LoadProductos();
        }
        catch
        {
            args.Cancel = true;
        }
        finally
        {
            deferral.Complete();
        }
    }

    // ── Guardar desde el diálogo de creación ──────────────────────────────────
    private async void CreateProductoDialog_PrimaryButtonClick(
        ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();
        try
        {
            await CreateViewModel.GuardarProductoCommand.ExecuteAsync(null);
        }
        catch
        {
            args.Cancel = true;
        }
        finally
        {
            deferral.Complete();
        }
    }

    // ── Toast de notificaciones ───────────────────────────────────────────────
    private void OnNotificationRequested(object? sender, Notification notification)
    {
        _notificationTimer?.Stop();
        _notificationTimer = null;
        TitleTextBlock.Text = notification.Title;
        MessageTextBlock.Text = notification.Message;
        (IconContainer.Background, NotificationIcon.Glyph) = notification.Type switch
        {
            NotificationType.Success => (Brush(Colors.SeaGreen), "\uE73E"),
            NotificationType.Error => (Brush(Colors.Crimson), "\uEA39"),
            NotificationType.Warning => (Brush(Colors.DarkOrange), "\uE7BA"),
            _ => (Brush(ColorHelper.FromArgb(255, 0, 120, 212)), "\uE946")
        };
        NotificationToastBorder.Visibility = Visibility.Visible;
        _notificationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(notification.DurationSeconds)
        };
        _notificationTimer.Tick += (_, _) =>
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

    // ── Helpers ───────────────────────────────────────────────────────────────
    private static SolidColorBrush Brush(Windows.UI.Color color) => new(color);
}