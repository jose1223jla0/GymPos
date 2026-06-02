using GymPos.Models;
using GymPos.Repository;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.VentaVM;

public partial class ListVentaViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Venta> ListVentas { get; private set; } = new ObservableCollection<Venta>();
    private readonly IRepositoryVenta _repositoryVenta;
    private int _idVenta;
    private DateOnly _fechaVenta;
    private decimal _totalVenta;
    public int IdVenta
    {
        get => _idVenta;
        set { if (_idVenta != value) { _idVenta = value; OnPropertyChanged(nameof(IdVenta)); } }
    }

    public DateOnly FechaVenta
    {
        get => _fechaVenta;
        set { if (_fechaVenta != value) { _fechaVenta = value; OnPropertyChanged(nameof(FechaVenta)); } }
    }

    public decimal TotalVenta
    {
        get => _totalVenta;
        set { if (_totalVenta != value) { _totalVenta = value; OnPropertyChanged(nameof(TotalVenta)); } }
    }
    public ListVentaViewModel(IRepositoryVenta repositoryVenta)
    {
        _repositoryVenta = repositoryVenta;
    }


    public async Task InitAsync()
    {
        await LoadVentas();
    }
    /*========================================================================================
     * Carga las ventas desde el repositorio y las agrega a la colección observable ListVentas.
     =========================================================================================*/
    private async Task LoadVentas()
    {
        var ventas = await _repositoryVenta.ObtenerTodasAsync();
        ListVentas.Clear();
        foreach (var venta in ventas)
        {
            ListVentas.Add(venta);
        }
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
