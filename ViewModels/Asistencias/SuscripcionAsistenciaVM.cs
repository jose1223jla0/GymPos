using GymPos.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GymPos.ViewModels.Asistencias;

public partial class SuscripcionAsistenciaVM : INotifyPropertyChanged
{
    private bool _asistioHoy;
    private int _totalSesiones;
    public Suscripcion Suscripcion { get;  set; } = null!;

    public bool AsistioHoy
    {
        get => _asistioHoy;
        set
        {
            if (_asistioHoy != value)
            {
                _asistioHoy = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
