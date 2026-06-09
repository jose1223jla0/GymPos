using GymPos.Models;
using GymPos.ViewModels.ProductoVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
namespace GymPos.Views.ProductoPage;

public sealed partial class CreateProductoPage : UserControl
{
    public CreateProductoViewModel createVM;

    public CreateProductoPage()
    {
        InitializeComponent();
        createVM = App.Services!.GetRequiredService<CreateProductoViewModel>();
        DataContext = createVM;
    }

    private void TxtNombre_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (DataContext is CreateProductoViewModel vm)
        {
            vm.NombreProducto = TxtNombre.Text;
        }
    }

    private void CmbCategoria_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is CreateProductoViewModel vm)
        {
            vm.CategoriaSeleccionada = CmbCategoria.SelectedItem as Categoria;
        }
    }

    private void NbPrecio_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
    {
        if (DataContext is CreateProductoViewModel vm)
        {
            vm.PrecioProductoDouble = e.NewValue;
        }
    }

    private void NbStock_ValueChanged(object sender, NumberBoxValueChangedEventArgs e)
    {
        if (DataContext is CreateProductoViewModel vm)
        {
            vm.StockProducto = (int)e.NewValue;
        }
    }
}
