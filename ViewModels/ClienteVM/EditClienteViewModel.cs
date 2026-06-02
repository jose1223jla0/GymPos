using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ClienteVM;
public partial class EditClienteViewModel:ObservableObject
{
    public ObservableCollection<Cliente> Clientes { get; } = new ();
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly INotificationService _notificationService;

    [ObservableProperty] private int idCliente;
    [ObservableProperty] private string? nombres;
    [ObservableProperty] private string? apellidos;
    [ObservableProperty] private string? dni;

    public EditClienteViewModel(IRepositoryCliente repositoryCliente, INotificationService notificationService)
    {
        _repositoryCliente = repositoryCliente;
        _notificationService = notificationService;
    }

    public async Task LoadClientes(Cliente cliente)
    {
        IdCliente = cliente.IdCliente;
        Nombres = cliente.Nombres;
        Apellidos = cliente.Apellidos;
        Dni = cliente.Dni;
    }

    [RelayCommand]
    public async Task SaveChangesCliente()
    {
        try
        {
            var cliente = new Cliente
            {
                IdCliente = IdCliente,
                Nombres = Nombres!,
                Apellidos = Apellidos!,
                Dni = Dni!
            };
            await _repositoryCliente.UpdateCliente(cliente);
            _notificationService.ShowSuccess("Cliente Actualizado", $"El cliente {Nombres} {Apellidos} ha sido actualizado correctamente.");
        }
        catch (System.Exception ex)
        {
            _notificationService.ShowError("Error al actualizar", $"Error: {ex.Message}");
        }
    }
}
