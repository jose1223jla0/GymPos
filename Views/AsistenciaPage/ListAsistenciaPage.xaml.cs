using GymPos.ViewModels.Asistencias;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace GymPos.Views.AsistenciaPage;

public sealed partial class ListAsistenciaPage : Page
{
    public AsistenciaViewModel ViewModel { get; }
    public ListAsistenciaPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<AsistenciaViewModel>();
        DataContext = ViewModel;
        Loaded+= AsistenciaListPage_Loaded;
    }


    public async void AsistenciaListPage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }
}
