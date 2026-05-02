using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Clientes;

public partial class AddClienteViewModel: ObservableObject
{
    private readonly IRepositoryCliente _repositoryCliente;
    [ObservableProperty] private int idCliente;
    [ObservableProperty] private string nombres;
    [ObservableProperty] private string apellidos;
    [ObservableProperty] private string dni;

    public bool IsEditMode => IdCliente > 0;

    public void LoadCliente(Cliente cliente)
    {
        IdCliente = cliente.IdCliente;
        Nombres = cliente.Nombres;
        Apellidos = cliente.Apellidos;
        Dni = cliente.Dni;
    }

    public AddClienteViewModel(IRepositoryCliente repositoryCliente)
    {
        _repositoryCliente = repositoryCliente;
    }

    //private async Task AddCliente()
    //{
    //    var newCliente = new Cliente
    //    {
    //        Nombres = Nombres,
    //        Apellidos = Apellidos,
    //        Dni = Dni
    //    };
    //    await _repositoryCliente.AddCliente(newCliente);
    //}
    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsEditMode)
        {
            var cliente = new Cliente
            {
                IdCliente = IdCliente,
                Nombres = Nombres,
                Apellidos = Apellidos,
                Dni = Dni
            };

            await _repositoryCliente.UpdateCliente(cliente);
        }
        else
        {
            var cliente = new Cliente
            {
                Nombres = Nombres,
                Apellidos = Apellidos,
                Dni = Dni
            };

            await _repositoryCliente.AddCliente(cliente);
        }
    }

    public void Clear()
    {
        IdCliente = 0;
        Nombres = string.Empty;
        Apellidos = string.Empty;
        Dni = string.Empty;
    }
}
