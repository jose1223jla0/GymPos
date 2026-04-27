using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryCliente
{
    Task<List<Cliente>> GetAll();
    Task AddCliente(Cliente cliente);
}

public class RepositoryCliente : IRepositoryCliente
{
    private readonly GymPosContext _context;
    public RepositoryCliente(GymPosContext context)
    {
        _context = context;
    }

    public async Task<List<Cliente>> GetAll()
    {
        var listaClientes = await _context.Clientes.ToListAsync();
        return listaClientes;
    }

    public async Task AddCliente(Cliente cliente)
    {
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();
    }
}
