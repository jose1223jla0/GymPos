using CommunityToolkit.Mvvm.ComponentModel;
using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.Membresias;

public partial class MembresiaViewModel : ObservableObject
{
    private readonly IRepositoryMembresia _repositoryMembresia;
    public ObservableCollection<Membresia> MembresiaList { get; } = new();
    public MembresiaViewModel(IRepositoryMembresia repositoryMembresia)
    {
        _repositoryMembresia = repositoryMembresia;
    }

    public async Task InitAsync()
    {
        await LoadMembresias();
    }

    private async Task LoadMembresias()
    {
        var lista = await _repositoryMembresia.GetAllAsync();
        MembresiaList.Clear();
        foreach (var item in lista)
        {
            MembresiaList.Add(item);
        }
    }
}
