using GymPos.Models;
using GymPos.Services;
using GymPos.ViewModels.ClienteVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GymPos.Views.ClientesPage;

public sealed partial class ListClientePage : Page
{
    public ListClienteViewModel ViewModel { get; }
    public EditClienteViewModel EditViewModel { get; }
    private readonly INotificationService _notificationService;
    private DispatcherTimer? _notificationTimer;

    public ListClientePage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ListClienteViewModel>();
        EditViewModel = App.Services!.GetRequiredService<EditClienteViewModel>();
        _notificationService = App.Services!.GetRequiredService<INotificationService>();
        ViewModel.OpenDialogRequest += ViewModel_OpenDialogRequest;
        Loaded += ClientesListPage_Loaded;
        _notificationService.NotificationRequested += NotificationService_NotificationRequested;
    }

    private async void ClientesListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.LoadClientes();
    }

    private async void ViewModel_OpenDialogRequest(Cliente cliente)
    {
        await EditViewModel.LoadClientes(cliente);
        ClienteDialog.XamlRoot = this.XamlRoot;

        await ClienteDialog.ShowAsync();
    }

    private async void ClienteDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        await EditViewModel.SaveChangesClienteCommand.ExecuteAsync(null);
        await ViewModel.LoadClientes();
    }

    private void NotificationService_NotificationRequested(object? sender, Notification notification)
    {
        if (_notificationTimer != null)
        {
            _notificationTimer.Stop();
            _notificationTimer = null;
        }
        TitleTextBlock.Text = notification.Title;
        MessageTextBlock.Text = notification.Message;
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

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        NotificationToastBorder.Visibility = Visibility.Collapsed;
        _notificationTimer?.Stop();
    }
}
