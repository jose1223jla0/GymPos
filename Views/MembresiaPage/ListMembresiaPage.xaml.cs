using GymPos.Models;
using GymPos.ViewModels.MembresiasVM;
using GymPos.Views.SuscripcionPage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
namespace GymPos.Views.MembresiaPage;

public sealed partial class ListMembresiaPage : Page
{
    public ListMembresiaViewModel ViewModel;
    public ListMembresiaPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ListMembresiaViewModel>();
        DataContext = ViewModel;
        Loaded += ListMembresiaPage_Loaded;
    }

    public async void ListMembresiaPage_Loaded (object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        var membresiaSeleccionada = (Membresia)e.ClickedItem;
        Frame.Navigate(typeof(CreateSuscripcionPage), membresiaSeleccionada);
    }
}