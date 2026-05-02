using GymPos.Models;
using GymPos.ViewModels.Membresias;
using GymPos.Views.SuscripcionPage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Windows.Forms;
namespace GymPos.Views.MembresiaPage;

public sealed partial class MembresiaListPage : Page
{
    public MembresiaViewModel ViewModel;
    public MembresiaListPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<MembresiaViewModel>();
        DataContext = ViewModel;
        Loaded += MembresiaListPage_Loaded;
    }

    public async void MembresiaListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        var membresiaSeleccionada = (Membresia)e.ClickedItem;
        Frame.Navigate(typeof(CreateSuscripcionPage), membresiaSeleccionada);
    }
}