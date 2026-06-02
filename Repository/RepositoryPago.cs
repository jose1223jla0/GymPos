using GymPos.Data.DbData;
using GymPos.Models;
using System.Threading.Tasks;

namespace GymPos.Repository;
public interface IRepositoryPago
{
    Task<Pago> CrearAsync(Pago pago);
    Task GuardarCambiosAsync();
}

public class RepositoryPago : IRepositoryPago
{
    private readonly GymPosContext _context;
    public RepositoryPago(GymPosContext context)
    {
        _context = context;
    }
    public async Task<Pago> CrearAsync(Pago pago)
    {
        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();
        return pago;
    }

    public async Task GuardarCambiosAsync()
    {
        await _context.SaveChangesAsync();
    }
}
