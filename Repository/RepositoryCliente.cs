using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryCliente
{
    Task<List<Cliente>> GetAll();
    Task AddCliente(Cliente cliente);
    Task UpdateCliente(Cliente cliente);
    Task<Cliente?> GetClienteByDni(string dni);
    Task<List<Cliente>> BuscarPorDniParcial(string dni);
    Task<List<Cliente>> GetPaged(int page, int pageSize);
    Task<int> CountClientes();
    Task<List<Cliente>> BuscarClientes(string query);

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

    public async Task UpdateCliente(Cliente cliente)
    {
        var existeCliente = await _context.Clientes.FindAsync(cliente.IdCliente);

        if (existeCliente == null)
        {
            throw new Exception("Cliente no encontrado");
        }
        existeCliente.Nombres = cliente.Nombres;
        existeCliente.Apellidos = cliente.Apellidos;
        existeCliente.Dni = cliente.Dni;
        await _context.SaveChangesAsync();
    }
    public async Task<Cliente?> GetClienteByDni(string dni)
    {
        return await _context.Clientes.FirstOrDefaultAsync(c => c.Dni == dni);
    }

    public async Task<List<Cliente>> BuscarPorDniParcial(string dni)
    {
        return await _context.Clientes.Where(c => c.Dni.Contains(dni)).OrderBy(c => c.Dni)
                                      .Take(10)
                                      .ToListAsync();
    }
    public async Task<List<Cliente>> GetPaged(int page, int pageSize)
    {
        return await _context.Clientes.AsNoTracking()
                    .Select(c => new Cliente
                    {
                        IdCliente = c.IdCliente,
                        Dni = c.Dni,
                        Nombres = c.Nombres,
                        Apellidos = c.Apellidos
                    }).OrderBy(c => c.IdCliente).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<int> CountClientes()
    {
        return await _context.Clientes.CountAsync();
    }

    public async Task<List<Cliente>> BuscarClientes(string query)
    {
        var q = query.Trim().ToLower();
        return await _context.Clientes
                    .Where(c => c.Dni.Contains(q) || c.Nombres.ToLower().Contains(q) || c.Apellidos.ToLower().Contains(q))
                    .OrderBy(c => c.Apellidos)
                    .Take(10)
                    .ToListAsync();
    }
}
