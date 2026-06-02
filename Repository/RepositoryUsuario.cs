using GymPos.Data.DbData;
using GymPos.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymPos.Repository;

public interface IRepositoryUsuario
{
    Task<Usuario?> GetUsuarioByUsernameDniAsync(string usernameDni);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task AddUsuarioAsync(Usuario usuario);
    Task UpdateUsuarioAsync(Usuario usuario);
    Task DeleteUsuarioAsync(int idUsuario);
}

public class RepositoryUsuario : IRepositoryUsuario
{
    private readonly GymPosContext _context;
    public RepositoryUsuario(GymPosContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync()
    {
        return await _context.Usuarios.AsNoTracking().ToListAsync();
    }
    public async Task AddUsuarioAsync(Usuario usuario)
    {
        var nuevoUsuario = new Usuario
        {
            UsernameDni = usuario.UsernameDni,
            NombreUsuario = usuario.NombreUsuario,
            ApellidosUsuario = usuario.ApellidosUsuario,
            Password = usuario.Password,
            EstadoUsuario = usuario.EstadoUsuario,
            Rol = usuario.Rol
        };
        await _context.Usuarios.AddAsync(nuevoUsuario);
        await _context.SaveChangesAsync();

    }

    public Task DeleteUsuarioAsync(int idUsuario)
    {
        throw new System.NotImplementedException();
    }

    public async Task<Usuario?> GetUsuarioByUsernameDniAsync(string usernameDni)
    {
        var usuario = await _context.Usuarios.AsNoTracking()
                             .FirstOrDefaultAsync(u => u.UsernameDni == usernameDni);
        return usuario;
    }

    public async Task UpdateUsuarioAsync(Usuario usuario)
    {
        var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
        if (usuarioExistente == null)
        {
            throw new Exception("Usuario no encontrado");
        }
        usuarioExistente.UsernameDni = usuario.UsernameDni;
        usuarioExistente.NombreUsuario = usuario.NombreUsuario;
        usuarioExistente.ApellidosUsuario = usuario.ApellidosUsuario;
        usuarioExistente.Password = usuario.Password;
        usuarioExistente.EstadoUsuario = usuario.EstadoUsuario;
        usuarioExistente.Rol = usuario.Rol;
        await _context.SaveChangesAsync();
    }
}
