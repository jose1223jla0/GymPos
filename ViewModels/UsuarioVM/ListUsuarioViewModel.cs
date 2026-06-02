using GymPos.Models;
using GymPos.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.UsuarioVM;

public class ListUsuarioViewModel : INotifyPropertyChanged
{

    private readonly IRepositoryUsuario _repositoryUsuario;
    public ObservableCollection<Usuario> ListUsuario { get; } = new();
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

    // ── INotifyPropertyChanged ────────────────────────────────────────────────
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
