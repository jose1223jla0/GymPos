using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GymPos.ViewModels.UsuarioVM;

public partial class ListUsuarioViewModel : ObservableObject
{
    private readonly IRepositoryUsuario _repositoryUsuario;

    public ObservableCollection<Usuario> ListUsuario { get; } = new();

    public event Action<Usuario>? OpenEditDialogRequest;
    public event Action? OpenAddDialogRequest;

    public ListUsuarioViewModel(IRepositoryUsuario repositoryUsuario)
    {
        _repositoryUsuario = repositoryUsuario;
    }

    public async Task InitializeAsync()
    {
        await LoadUsuario();
    }

    /// <summary>
    /// Carga los usuarios desde el repositorio y actualiza ListUsuario.
    /// </summary>
    private async Task LoadUsuario()
    {
        var usuarios = await _repositoryUsuario.GetAllAsync();
        ListUsuario.Clear();
        foreach (var item in usuarios)
        {
            ListUsuario.Add(item);
        }
    }

    [RelayCommand]
    private void Edit(Usuario usuario)
    {
        OpenEditDialogRequest?.Invoke(usuario);
    }

    [RelayCommand]
    private void Add()
    {
        OpenAddDialogRequest?.Invoke();
    }
}
