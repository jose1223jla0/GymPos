using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Threading.Tasks;
using System.Globalization;

public partial class CreateMembresiaViewModel : ObservableObject
{
    private readonly IRepositoryMembresia _repository;
    private readonly INotificationService _notification;

    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private int sesiones;
    [ObservableProperty] private decimal precio;

    public string PrecioDouble
    {
        get => Precio.ToString("F2", CultureInfo.CurrentCulture);
        set
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out var parsed))
            {
                Precio = parsed;
            }
            else
            {
                Precio = 0;
            }
            OnPropertyChanged(nameof(PrecioDouble));
        }
    }

    partial void OnPrecioChanged(decimal value)
    {
        OnPropertyChanged(nameof(PrecioDouble));
    }

    public event Action? MembresiaCreada;

    public CreateMembresiaViewModel(IRepositoryMembresia repository, INotificationService notification)
    {
        _repository = repository;
        _notification = notification;
    }

    public void CargarParaAgregar()
    {
        Nombre = "";
        Sesiones = 0;
        Precio = 0;
    }

    private bool Validar()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            _notification.ShowWarning("Validación", "Nombre requerido");
            return false;
        }

        if (Sesiones <= 0)
        {
            _notification.ShowWarning("Validación", "Sesiones inválidas");
            return false;
        }

        if (Precio <= 0)
        {
            _notification.ShowWarning("Validación", "Precio inválido");
            return false;
        }

        return true;
    }

    [RelayCommand]
    public async Task CrearMembresia()
    {
        if (!Validar())
            return;

        var m = new Membresia
        {
            Nombre = Nombre.Trim(),
            Sesiones = Sesiones,
            Precio = Precio
        };

        await _repository.CrearAsync(m);

        _notification.ShowSuccess("Creado", "Membresía creada");

        MembresiaCreada?.Invoke();
    }
}