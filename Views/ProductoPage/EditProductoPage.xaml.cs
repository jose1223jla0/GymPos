using System;
using GymPos.ViewModels.ProductoVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
namespace GymPos.Views.ProductoPage;

public sealed partial class EditProductoPage : UserControl
{
    public EditProductoViewModel ViewModel { get; }
    public EditProductoPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<EditProductoViewModel>();
    }
}
