using GymPos.Models;
using GymPos.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerProductosAsync(int? idCategoria = null, string? terminoBusqueda = null);
}
public class ProductoService: IProductoService
{
    private readonly IRepositoryProducto _repo;

    public ProductoService(IRepositoryProducto repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Producto>> ObtenerProductosAsync(int? idCategoria = null, string? terminoBusqueda = null)
    {
        if (!string.IsNullOrWhiteSpace(terminoBusqueda))
        {
            return _repo.ObtenerPorNombreAsync(terminoBusqueda, idCategoria);
        }

        return idCategoria.HasValue
            ? _repo.ObtenerPorCategoriaAsync(idCategoria.Value)
            : _repo.ObtenerActivosAsync();
    }

}
