using GymPos.ViewModels.Suscripciones;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GymPos.Views.SuscripcionPage;
public sealed partial class ListSuscripcionPage : Page
{
    public ListSuscripcionViewModel ViewModel;
    public ListSuscripcionPage()
    {
        InitializeComponent();
        ViewModel=App.Services!.GetRequiredService<ListSuscripcionViewModel>();
        DataContext = ViewModel;
        Loaded += ListSuscripcionPage_Loaded;
    }

    private void ListSuscripcionPage_Loaded(object sender, RoutedEventArgs e)
    {
       
    }

    public async void SuscripcionListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }
}
