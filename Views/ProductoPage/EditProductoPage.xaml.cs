using System;
using GymPos.ViewModels.ProductoVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
namespace GymPos.Views.ProductoPage;

public sealed partial class EditProductoPage : UserControl
{
    private EditProductoViewModel? _editVM;

    public EditProductoPage()
    {
        InitializeComponent();
    }

    public void SetupForEdit(EditProductoViewModel vm)
    {
        _editVM = vm;
        DataContext = vm;
    }

    private void TxtNombre_TextChanged(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs e)
    {
        if (DataContext is EditProductoViewModel vm && sender is Microsoft.UI.Xaml.Controls.TextBox tb)
        {
            vm.NombreProducto = tb.Text;
        }
    }

    private void CmbCategoria_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
    {
        if (DataContext is EditProductoViewModel vm && sender is Microsoft.UI.Xaml.Controls.ComboBox cb)
        {
            vm.CategoriaSeleccionada = cb.SelectedItem as GymPos.Models.Categoria;
        }
    }

    private void NbPrecio_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs e)
    {
        if (DataContext is EditProductoViewModel vm)
        {
            vm.PrecioProductoDouble = e.NewValue;
        }
    }

    private void NbStock_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs e)
    {
        if (DataContext is EditProductoViewModel vm)
        {
            vm.StockProducto = (int)e.NewValue;
        }
    }
}
