using GymPos.Models;
using GymPos.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface IProductoService
{
    Task<IEnumerable<Producto>> ObtenerProductosAsync(int? idCategoria = null);
}
public class ProductoService: IProductoService
{
    private readonly IRepositoryProducto _repo;

    public ProductoService(IRepositoryProducto repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Producto>> ObtenerProductosAsync(int? idCategoria = null)
        => idCategoria.HasValue
            ? _repo.ObtenerPorCategoriaAsync(idCategoria.Value)
            : _repo.ObtenerActivosAsync();

}
