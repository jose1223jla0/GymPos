using GymPos.Models;
using GymPos.Services;
using GymPos.ViewModels.UsuarioVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
namespace GymPos.Views.UsuarioPage;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ListUsuarioPage : Page
{
    public ListUsuarioViewModel ViewModel { get; }
    public CreateUsuarioViewModel CreateViewModel { get; }
    public EditUsuarioViewModel EditViewModel { get; }
    private readonly INotificationService _notificationService;
    private readonly IAuthService _authService;
    private Microsoft.UI.Dispatching.DispatcherQueueTimer? _notificationTimer;

    public ListUsuarioPage()
    {
        ViewModel = App.Services!.GetRequiredService<ListUsuarioViewModel>();
        CreateViewModel = App.Services!.GetRequiredService<CreateUsuarioViewModel>();
        EditViewModel = App.Services!.GetRequiredService<EditUsuarioViewModel>();
        _notificationService = App.Services!.GetRequiredService<INotificationService>();
        _authService = App.Services!.GetRequiredService<IAuthService>();
        InitializeComponent();
        DataContext = ViewModel;
        ViewModel.OpenAddDialogRequest += OnOpenAddDialogRequest;
        ViewModel.OpenEditDialogRequest += OnOpenEditDialogRequest;
        _notificationService.NotificationRequested += NotificationService_NotificationRequested;
        CreateViewModel.UsuarioCreado += OnUsuarioCreado;
        EditViewModel.UsuarioActualizado += OnUsuarioActualizado;

        Loaded += ListUsuarioPage_Loaded;
    }

    public Microsoft.UI.Xaml.Visibility AddButtonVisibility => _authService.EsRecepcionista ? Microsoft.UI.Xaml.Visibility.Collapsed : Microsoft.UI.Xaml.Visibility.Visible;
    private async void ListUsuarioPage_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.InitializeAsync();
    }
    private void EditButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Usuario usuario)
        {
            if (_authService.EsRecepcionista)
            {
                _notificationService.ShowWarning("Permisos", "No tiene permisos para editar usuarios.");
                return;
            }
            if (usuario.Rol == Rol.SuperAdmin)
            {
                _notificationService.ShowWarning("Usuario", "El Super Administrador no se puede editar.");
                return;
            }
            if (ViewModel.EditCommand.CanExecute(usuario))
            {
                ViewModel.EditCommand.Execute(usuario);
            }
        }
    }
    private async void OnOpenAddDialogRequest()
    {
        CreateUserEditor.DataContext = CreateViewModel;
        CreateUsuarioDialog.XamlRoot = this.XamlRoot;
        await CreateUsuarioDialog.ShowAsync();
    }
    private async void OnOpenEditDialogRequest(Usuario usuario)
    {
        EditViewModel.CargarUsuario(usuario);
        EditUserEditor.SetupForEdit(EditViewModel);
        EditUsuarioDialog.XamlRoot = this.XamlRoot;
        await EditUsuarioDialog.ShowAsync();
    }

    private async void CreateUsuarioDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var def = args.GetDeferral();
        try
        {
            await CreateViewModel.GuardarAsync();
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

    private async void EditUsuarioDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var def = args.GetDeferral();
        try
        {
            await EditViewModel.GuardarAsync();
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

    private async void OnUsuarioCreado(object? sender, EventArgs e)
    {
        CreateUsuarioDialog.Hide();
        await ViewModel.InitializeAsync();
        _notificationService.ShowSuccess("Usuario", "Usuario creado correctamente.");
    }

    private async void OnUsuarioActualizado(object? sender, EventArgs e)
    {
        EditUsuarioDialog.Hide();
        await ViewModel.InitializeAsync();
        _notificationService.ShowSuccess("Usuario", "Usuario actualizado correctamente.");
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
        NotificationToastBorder.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
        _notificationTimer = DispatcherQueue.CreateTimer();
        _notificationTimer.Interval = TimeSpan.FromSeconds(notification.DurationSeconds);
        _notificationTimer.Tick += (s, e) =>
        {
            NotificationToastBorder.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
            _notificationTimer?.Stop();
            _notificationTimer = null;
        };
        _notificationTimer.Start();
    }

    private void CloseButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        NotificationToastBorder.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
        _notificationTimer?.Stop();
        _notificationTimer = null;
    }
}
