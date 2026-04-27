using GymPos.ViewModels.Clientes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
namespace GymPos.Views;

public sealed partial class ClientePage : Page
{
    public ClienteViewModel viewModel { get; }
    public ClientePage()
    {
        InitializeComponent();
        viewModel = App.Services!.GetRequiredService<ClienteViewModel>();
        DataContext = viewModel;
        
    }

    protected override async void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        await viewModel.LoadClientesCommand.ExecuteAsync(null);
    }
}
