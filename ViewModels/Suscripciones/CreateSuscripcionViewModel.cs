using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Services;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Suscripciones;

public partial class CreateSuscripcionViewModel : ObservableObject
{
    private readonly IServiceSuscripcion _service;

    public CreateSuscripcionViewModel(IServiceSuscripcion service)
    {
        _service = service;
    }

    [ObservableProperty]
    private Cliente clienteSeleccionado;

    [ObservableProperty]
    private Membresia membresiaSeleccionada;

    [RelayCommand]
    private async Task CrearSuscripcion()
    {
        if (ClienteSeleccionado == null || MembresiaSeleccionada == null)
        {
            return;
        }

        await _service.CrearSuscripcion(ClienteSeleccionado.IdCliente,MembresiaSeleccionada.IdMembresia);
    }
}
