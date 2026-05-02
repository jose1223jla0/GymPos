using GymPos.Models;
using GymPos.ViewModels.Clientes;
using GymPos.Views.ClientesPage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
namespace GymPos.Views;


public sealed partial class ClientePage : Page
{
    public ClienteViewModel ViewModel { get; }
    public ClientePage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<ClienteViewModel>();
        DataContext = ViewModel;
        ViewModel.OpenDialogRequested += OpenAddDialog;
        Loaded += ClientePage_Loaded;

    }

    private async void ClientePage_Loaded(object sender, RoutedEventArgs e)
    {
        await ViewModel.InitAsync();
    }

    private async void OpenAddDialog(Cliente? cliente)
    {
        var formVm = App.Services!.GetRequiredService<AddClienteViewModel>();
        if (cliente != null)
        {
            formVm.LoadCliente(cliente);
        }
        else
        {
            formVm.Clear();
        }

        var form = new AddClientePage
        {
            DataContext = formVm
        };

        var dialog = new ContentDialog
        {
            Title = cliente == null ? "Agregar Nuevo Cliente" : "Editar Cliente",
            Content = form,
            PrimaryButtonText = "Guardar",
            CloseButtonText = "Cancelar",
            CornerRadius = new CornerRadius(16),
            XamlRoot = this.XamlRoot
        };


        dialog.PrimaryButtonClick += async (s, e) =>
        {
            e.Cancel = true;

            await formVm.SaveCommand.ExecuteAsync(null);

            dialog.Hide();

            await ViewModel.InitAsync();
        };

        await dialog.ShowAsync();
    }
}
