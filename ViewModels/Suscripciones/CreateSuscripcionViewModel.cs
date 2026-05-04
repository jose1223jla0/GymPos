using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Suscripciones;

public partial class CreateSuscripcionViewModel : ObservableObject
{
    private readonly IServiceSuscripcion _service;
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly IRepositoryMembresia _repositoryMembresia;
    public ObservableCollection<Cliente> ClienteList = new();
    public ObservableCollection<Membresia> MembresiaList = new();
    private CancellationTokenSource _cts;
    public CreateSuscripcionViewModel(IServiceSuscripcion service, IRepositoryCliente repositoryCliente, IRepositoryMembresia repositoryMembresia)
    {
        _service = service;
        _repositoryCliente = repositoryCliente;
        _repositoryMembresia = repositoryMembresia;
    }

    [ObservableProperty]
    private Cliente? clienteSeleccionado;

    [ObservableProperty]
    private Membresia? membresiaSeleccionada;

    [ObservableProperty]
    private string? dni;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private bool isLoading=false;

    [ObservableProperty]
    private ObservableCollection<Cliente> clientesSugeridos = new();

    [RelayCommand]
    private async Task CrearSuscripcion()
    {
        if (ClienteSeleccionado == null || MembresiaSeleccionada == null)
        {
            return;
        }

        await _service.CrearSuscripcion(ClienteSeleccionado.IdCliente, MembresiaSeleccionada.IdMembresia);
    }

    partial void OnDniChanged(string value)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        if (string.IsNullOrWhiteSpace(value))
        {
            LimpiarCliente();
            return;
        }

        if (value.Length < 3)
        {
            LimpiarCliente();
            return;
        }

        _ = BuscarConDebounce(value, _cts.Token);
    }

    private void LimpiarCliente()
    {
        ClienteSeleccionado = null!;
        ClientesSugeridos.Clear();
        ErrorMessage = string.Empty;
    }

    private async Task BuscarConDebounce(string value, CancellationToken token)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
            {
                ClientesSugeridos.Clear();
                return;
            }

            await Task.Delay(300, token);

            var resultados = await _repositoryCliente.BuscarPorDniParcial(value);

            ClientesSugeridos.Clear();

            foreach (var item in resultados)
            {
                ClientesSugeridos.Add(item);
            }
        }
        catch (TaskCanceledException)
        {
            // ignorado
        }
    }
    public async Task SeleccionarClientePorDni(string dni)
    {
        var cliente = await _repositoryCliente.GetClienteByDni(dni);
        ClienteSeleccionado = cliente!;
    }

    public async Task LoadClientes()
    {
        var clientes = await _repositoryCliente.GetAll();
        ClienteList.Clear();
        foreach (var item in clientes)
        {
            ClienteList.Add(item);
        }
    }
}
