using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Clientes;

public partial class ClienteViewModel : ObservableObject
{
    public ObservableCollection<Cliente> ClienteList { get; } = new();
    private readonly IRepositoryCliente _repositoryCliente;
    public event Action<Cliente?>? OpenDialogRequested;

    public ClienteViewModel(IRepositoryCliente repositoryCliente)
    {
        _repositoryCliente = repositoryCliente;
    }

    public async Task InitAsync()
    {
        await LoadClientes();
    }

    private async Task LoadClientes()
    {
        var lista = await _repositoryCliente.GetAll();
        ClienteList.Clear();
        foreach (var item in lista)
        {
            ClienteList.Add(item);
        }
    }

    [RelayCommand]
    private async Task AddCliente()
    {
        OpenDialogRequested?.Invoke(null);
    }

    [RelayCommand]
    private void Edit(Cliente cliente)
    {
        OpenDialogRequested?.Invoke(cliente);
    }
}
