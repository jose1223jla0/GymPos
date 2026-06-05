using GymPos.Services;
using GymPos.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
namespace GymPos.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LoginPage : Page
{
    private readonly LoginViewModel _viewModel;
    public event EventHandler? LoginSucceeded;

    public LoginPage()
    {
        InitializeComponent();
        var authService = App.Services!.GetService(typeof(IAuthService)) as IAuthService;
        _viewModel = new LoginViewModel(authService!);
        _viewModel.LoginExitoso += (s, e) => LoginSucceeded?.Invoke(this, EventArgs.Empty);
    }

    private async void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        await EjecutarLogin();
    }

    private async void PasswordBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            await EjecutarLogin();
    }

    private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
            PasswordBox.Focus(FocusState.Programmatic);
    }

    private async System.Threading.Tasks.Task EjecutarLogin()
    {
        ErrorBar.IsOpen = false;
        BtnLogin.IsEnabled = false;
        LoadingBar.Visibility = Visibility.Visible;

        _viewModel.UsernameDni = UserTextBox.Text;
        await _viewModel.LoginCommand.ExecuteAsync(PasswordBox.Password);

        LoadingBar.Visibility = Visibility.Collapsed;
        BtnLogin.IsEnabled = true;

        if (_viewModel.HasError)
        {
            ErrorBar.Message = _viewModel.ErrorMessage;
            ErrorBar.IsOpen = true;
            PasswordBox.Password = string.Empty;
            PasswordBox.Focus(FocusState.Programmatic);
        }
    }
}