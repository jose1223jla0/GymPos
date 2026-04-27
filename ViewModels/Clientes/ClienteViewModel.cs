using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Clientes;

public partial class ClienteViewModel : ObservableObject
{
    
    public ObservableCollection<Cliente> clientes { get; set; } = new();
    private readonly IRepositoryCliente _repositoryCliente;


    public ClienteViewModel(IRepositoryCliente repositoryCliente)
    {
        _repositoryCliente = repositoryCliente;
    }


    [RelayCommand]
    public async Task LoadClientes()
    {
        var listaClientes = await _repositoryCliente.GetAll();
        clientes.Clear();
        foreach (var item in listaClientes)
        {
            clientes.Add(item);
        }
    }


    [RelayCommand]
    private void Edit(Cliente cliente)
    {
    }

    [RelayCommand]
    private void Delete(Cliente cliente)
    {
        clientes.Remove(cliente);
    }
}
