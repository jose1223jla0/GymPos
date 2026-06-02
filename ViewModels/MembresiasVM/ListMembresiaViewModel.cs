using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.MembresiasVM;

public partial class ListMembresiaViewModel : INotifyPropertyChanged
{
    private readonly IRepositoryMembresia _repositoryMembresia;
    public ObservableCollection<Membresia> MembresiaList { get; } = new();
    public ListMembresiaViewModel(IRepositoryMembresia repositoryMembresia)
    {
        _repositoryMembresia = repositoryMembresia;
    }

    public async Task InitAsync()
    {
        await LoadMembresias();
    }
    /// <summary>
    /// Carga las membresías desde el repositorio y actualiza MembresiaList.
    /// </summary>
    private async Task LoadMembresias()
    {
        var lista = await _repositoryMembresia.GetAllAsync();
        MembresiaList.Clear();
        foreach (var item in lista)
        {
            MembresiaList.Add(item);
        }
    }

    // ── INotifyPropertyChanged ────────────────────────────────────────────────

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
