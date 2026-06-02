using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.MembresiasVM;

public partial class EditMembresiaViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Membresia> ListMembresias { get; } = new();
    private readonly IRepositoryMembresia _repositoryMembresia;
    private int _idMembresia;
    private string _nombre = string.Empty;
    private int _totalSesiones;
    private decimal _precio;
    public int IdMembresia
    {
        get => _idMembresia;
        set
        {
            if (_idMembresia != value)
            {
                _idMembresia = value;
                OnPropertyChanged(nameof(IdMembresia));
            }
        }
    }
    public string Nombre
    {
        get => _nombre;
        set
        {
            if (_nombre != value)
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }
    }
    public int TotalSesiones
    {
        get => _totalSesiones;
        set
        {
            if (_totalSesiones != value)
            {
                _totalSesiones = value;
                OnPropertyChanged(nameof(TotalSesiones));
            }
        }
    }
    public decimal Precio
    {
        get => _precio;
        set
        {
            if (_precio != value)
            {
                _precio = value;
                OnPropertyChanged(nameof(Precio));
            }
        }
    }
    public EditMembresiaViewModel(IRepositoryMembresia repositoryMembresia)
    {
        _repositoryMembresia = repositoryMembresia;
    }

    [RelayCommand]
    public async Task SaveChangesMembresia()
    {
        try
        {
            var membresia = new Membresia
            {
                IdMembresia = IdMembresia,
                Nombre = Nombre,
                Sesiones = TotalSesiones,
                Precio = Precio
            };
            await _repositoryMembresia.UpdateAsync(membresia);

        }
        catch (Exception ex)
        {
            throw new Exception($"Error al actualizar la membresía: {ex.Message}");
        }
    }
    /// <summary>
    /// 
    /// </summary>

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
