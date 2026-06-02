using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;
public interface IRepositoryCategoria
{
    Task<IEnumerable<Categoria>> ObtenerActivasAsync();
}
public class RepositoryCategoria : IRepositoryCategoria
{
    private readonly GymPosContext _context;
    public RepositoryCategoria(GymPosContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Categoria>> ObtenerActivasAsync()
    {
        return await _context.Categorias.OrderBy(c => c.NombreCategoria).ToListAsync();
    }
}
