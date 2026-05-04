using CommunityToolkit.Mvvm.ComponentModel;
using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Suscripciones;

public class ListSuscripcionViewModel : ObservableObject
{
    public ObservableCollection<Suscripcion> ListSuscripciones { get; } = new();
    private readonly IRepositorySuscripcion _repositorySuscripcion;
    public ListSuscripcionViewModel(IRepositorySuscripcion repositorySuscripcion)
    {
        _repositorySuscripcion = repositorySuscripcion;
    }

    public async Task InitAsync()
    {
        await LoadSuscripciones();
    }

    private async Task LoadSuscripciones()
    {
        var suscripciones = await _repositorySuscripcion.GetAllSuscripcion();
        ListSuscripciones.Clear();
        foreach(var item in suscripciones)
        {
            ListSuscripciones.Add(item);
        }
    }
}
