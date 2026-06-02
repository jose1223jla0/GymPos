using GymPos.ViewModels.ProductoVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace GymPos.Views.ProductoPage;

public sealed partial class ListProductoPage : Page
{
    public ListProductoViewModel ViewModel { get; }
    public ListProductoPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ListProductoViewModel>();
        Loaded += ProductosListPage_Loaded;
    }

    private async void ProductosListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }

    private async void ProductoDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        
    }
}
