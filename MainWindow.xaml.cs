using GymPos.Views;
using GymPos.Views.AsistenciaPage;
using GymPos.Views.MembresiaPage;
using GymPos.Views.SuscripcionPage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
namespace GymPos;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItemContainer == null)
        {
            return;
        }

        string? tag = args.SelectedItemContainer.Tag.ToString();

        switch (tag)
        {
            case "SamplePage1":
                contentFrame.Navigate(typeof(DashboardPage));
                break;

            case "SamplePage2":
                contentFrame.Navigate(typeof(ClientePage));
                break;

            case "SamplePage3":
                contentFrame.Navigate(typeof(ListSuscripcionPage));
                break;

            case "SamplePage4":
                contentFrame.Navigate(typeof(MembresiaListPage));
                break;

            case "SamplePage5":
                contentFrame.Navigate(typeof(ListAsistenciaPage));
                break;
        }
    }
}
