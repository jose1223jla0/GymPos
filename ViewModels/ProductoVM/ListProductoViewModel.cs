using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ProductoVM;

public class ListProductoViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Producto> ListProductos { get; } = new();

    private readonly IRepositoryProducto _repositoryProducto;

    public ListProductoViewModel(IRepositoryProducto repositoryProducto)
    {
        _repositoryProducto = repositoryProducto;
    }

    public async Task InitAsync()
    {
        await LoadProductos();

    }

    private async Task LoadProductos()
    {
        var productos = await _repositoryProducto.ObtenerActivosAsync();
        ListProductos.Clear();
        foreach (var product in productos)
        {
            ListProductos.Add(product);
        }
    }

 
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
