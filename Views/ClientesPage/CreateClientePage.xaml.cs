using GymPos.ViewModels.ClienteVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace GymPos.Views.ClientesPage;

public sealed partial class CreateClientePage : UserControl
{
    public CreateClienteViewModel ViewModel { get; }
    public CreateClientePage()
    {
        InitializeComponent();
        ViewModel= App.Services!.GetRequiredService<CreateClienteViewModel>();
        DataContext = ViewModel;
    }
}
