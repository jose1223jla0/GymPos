using GymPos.Models;
using GymPos.Services;
using GymPos.ViewModels.MembresiasVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
namespace GymPos.Views.MembresiaPage;

public sealed partial class ListMembresiaPage : Page
{
    public ListMembresiaViewModel ViewModel { get; }
    public EditMembresiaViewModel EditViewModel { get; }
    public CreateMembresiaViewModel CreateViewModel { get; }
    private readonly INotificationService _notificationService;
    private DispatcherTimer? _notificationTimer;


    public ListMembresiaPage()
    {
        // Obtener instancias antes de inicializar el componente XAML
        ViewModel = App.Services!.GetRequiredService<ListMembresiaViewModel>();
        EditViewModel = App.Services!.GetRequiredService<EditMembresiaViewModel>();
        CreateViewModel = App.Services!.GetRequiredService<CreateMembresiaViewModel>();
        _notificationService = App.Services!.GetRequiredService<INotificationService>();

        InitializeComponent();

        // Suscripciones a eventos
        ViewModel.OpenEditDialogRequest += OnOpenEditDialog;
        ViewModel.OpenAddDialogRequest += OnOpenAddDialog;
        _notificationService.NotificationRequested += OnNotificationRequested;
        EditViewModel.MembresiaGuardada += OnMembresiaGuardada;
        CreateViewModel.MembresiaCreada += OnMembresiaCreada;
        Loaded += async (s, e) => await ViewModel.InitAsync();
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ViewModel.OpenEditDialogRequest -= OnOpenEditDialog;
        ViewModel.OpenAddDialogRequest -= OnOpenAddDialog;
        _notificationService.NotificationRequested -= OnNotificationRequested;

        EditViewModel.MembresiaGuardada -= OnMembresiaGuardada;
        CreateViewModel.MembresiaCreada -= OnMembresiaCreada;
    }

    // =========================
    // EDITAR
    // =========================
    private async void OnOpenEditDialog(Membresia membresia)
    {
        EditViewModel.CargarParaEditar(membresia);
        EditMembresiaDialog.XamlRoot = XamlRoot;
       await EditMembresiaDialog.ShowAsync();
    }

    // =========================
    // AGREGAR
    // =========================
    private async void OnOpenAddDialog()
    {
        CreateViewModel.CargarParaAgregar();
        CreateMembresiaDialog.XamlRoot = XamlRoot;
        await CreateMembresiaDialog.ShowAsync();
    }

    // =========================
    // BOTÓN PRIMARY DIALOG
    // =========================

    private async void CreateMembresiaDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var def = args.GetDeferral();
        try
        {
            await CreateViewModel.CrearMembresiaCommand.ExecuteAsync(null);
        }
        catch
        {
            args.Cancel = true;
        }
        finally
        {
            def.Complete();
        }
    }

    private async void EditMembresiaDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var def = args.GetDeferral();
        try
        {
            await EditViewModel.GuardarMembresiaCommand.ExecuteAsync(null);
        }
        catch
        {
            args.Cancel = true;
        }
        finally
        {
            def.Complete();
        }
    }

    // =========================
    // EVENTOS DE GUARDADO
    // =========================
    private async void OnMembresiaGuardada()
    {
        // Ocultar el diálogo de editar y recargar lista
        EditMembresiaDialog.Hide();
        await ViewModel.LoadMembresias();
    }

    private async void OnMembresiaCreada()
    {
        // Ocultar el diálogo de crear y recargar lista
        CreateMembresiaDialog.Hide();
        await ViewModel.LoadMembresias();
    }

    // =========================
    // NOTIFICACIONES
    // =========================
    private void OnNotificationRequested(object? sender, Notification notification)
    {
        _notificationTimer?.Stop();
        TitleTextBlock.Text = notification.Title;
        MessageTextBlock.Text = notification.Message;
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

    private void OnEditMembresiaClick(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is Membresia m)
        {
            ViewModel.EditCommand.Execute(m);
        }
    }
}