using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace GymPos.ViewModels.CarritoVM;

public partial class CarritoViewModel : ObservableObject
{
    public ObservableCollection<CarritoItemViewModel> Items { get; } = new();

    public decimal Total => Items.Sum(i => i.Subtotal);
    public string TotalFormateado => Total.ToString("F2");

    [RelayCommand]
    public void AddProducto(Producto producto)
    {
        var existing = Items.FirstOrDefault(i => i.IdProducto == producto.IdProducto);
        if (existing != null)
        {
            if (existing.Cantidad < existing.StockDisponible)
                existing.Cantidad++;
            return;
        }

        Items.Add(new CarritoItemViewModel(producto));
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalFormateado));
    }

    [RelayCommand]
    public void RemoveItem(CarritoItemViewModel item)
    {
        if (Items.Contains(item))
            Items.Remove(item);
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalFormateado));
    }

    [RelayCommand]
    public void Increase(CarritoItemViewModel item)
    {
        if (item.Cantidad < item.StockDisponible)
            item.Cantidad++;
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalFormateado));
    }

    [RelayCommand]
    public void Decrease(CarritoItemViewModel item)
    {
        if (item.Cantidad > 1)
            item.Cantidad--;
        else
            RemoveItem(item);
        OnPropertyChanged(nameof(Total));
        OnPropertyChanged(nameof(TotalFormateado));
    }
}
