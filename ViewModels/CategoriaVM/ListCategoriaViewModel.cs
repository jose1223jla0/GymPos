using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.CategoriaVM;

public partial class ListCategoriaViewModel : INotifyPropertyChanged
{

    public ObservableCollection<Categoria> ListCategoria { get; } = new();
    private readonly IRepositoryCategoria _repositorioCategoria;
    public ListCategoriaViewModel(IRepositoryCategoria repositoryCategoria)
    {
        _repositorioCategoria = repositoryCategoria;
    }
    public async Task InitializeAsync()
    {
        await LoadVentas();
    }
    /// <summary>
    /// Carga las categorías desde el repositorio y las agrega a la colección observable ListCategoria.
    /// </summary>
    private async Task LoadVentas()
    {
        var categorias = await _repositorioCategoria.ObtenerActivasAsync();
        ListCategoria.Clear();
        foreach (var categoria in categorias)
        {
            ListCategoria.Add(categoria);
        }
    }

    // ── INotifyPropertyChanged ────────────────────────────────────────────────
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
