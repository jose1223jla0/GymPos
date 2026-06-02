using GymPos.Models;
using GymPos.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Services;

public interface ICategoriaService
{
    Task<IEnumerable<Categoria>> ObtenerCategoriasAsync();
}
public class CategoriaService : ICategoriaService
{
    private readonly IRepositoryCategoria _repo;

    public CategoriaService(IRepositoryCategoria repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Categoria>> ObtenerCategoriasAsync()
        => _repo.ObtenerActivasAsync();

}
