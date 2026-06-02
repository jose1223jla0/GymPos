using GymPos.ViewModels.ClienteVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;

namespace GymPos.Views.ClientesPage;
public sealed partial class EditClientePage : UserControl
{
    public EditClienteViewModel ViewModel { get; }
    public EditClientePage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<EditClienteViewModel>();
        DataContext = ViewModel;
    }
}
