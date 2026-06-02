using CommunityToolkit.Mvvm.ComponentModel;
using GymPos.Models;

namespace GymPos.ViewModels.CategoriaVM;

public partial class CategoriaViewModel : ObservableObject
{
    private readonly Categoria _model;

    public int IdCategoria => _model.IdCategoria;
    public string NombreCategoria => _model.NombreCategoria;

    [ObservableProperty]
    private bool _isSeleccionada;

    public CategoriaViewModel(Categoria model)
    {
        _model = model;
    }
}

