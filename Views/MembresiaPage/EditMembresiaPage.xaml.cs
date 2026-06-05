using GymPos.ViewModels.MembresiasVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GymPos.Views.MembresiaPage;
public sealed partial class EditMembresiaPage : UserControl
{
    public EditMembresiaViewModel ViewModel { get; }
    public EditMembresiaPage()
    {
        InitializeComponent();
        ViewModel = App.Services!.GetRequiredService<EditMembresiaViewModel>();
        DataContext = ViewModel;
    }
}
