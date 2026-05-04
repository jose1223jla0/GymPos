using GymPos.Models;
using GymPos.ViewModels.Suscripciones;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace GymPos.Views.SuscripcionPage;

public sealed partial class CreateSuscripcionPage : Page
{
    public CreateSuscripcionViewModel ViewModel { get; }
    public CreateSuscripcionPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<CreateSuscripcionViewModel>();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Membresia membresia)
        {
            ViewModel.MembresiaSeleccionada = membresia;
        }
    }
    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender,AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            ViewModel.Dni = sender.Text;

            if (string.IsNullOrWhiteSpace(sender.Text))
            {
                ViewModel.ClienteSeleccionado = null!;
            }
        }
    }

    private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender,AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        var cliente = args.SelectedItem as Cliente;

        if (cliente != null)
        {
            ViewModel.ClienteSeleccionado = cliente;
            sender.Text = cliente.Dni;
        }
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        await ViewModel.CrearSuscripcionCommand.ExecuteAsync(null);
    }
}
