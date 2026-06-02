using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.ClienteVM;

public partial class CreateClienteViewModel : INotifyPropertyChanged
{
    // =======================
    // CAMPOS
    // =======================
    private int _idCliente;
    private string _dniCliente = string.Empty;
    private string _nombreCliente = string.Empty;
    private string _apellidoCliente = string.Empty;

    private string _dniError = string.Empty;
    private string _nombreError = string.Empty;
    private string _apellidoError = string.Empty;

    private string _errorMessage = string.Empty;
    private bool _hasError;
    private bool _isLoading;

    public int IdCliente
    {
        get => _idCliente;
        set
        {
            if (_idCliente != value)
            {
                _idCliente = value;
                OnPropertyChanged(nameof(IdCliente));
            }
        }
    }
    public string DniCliente
    {
        get => _dniCliente;
        set
        {
            if (_dniCliente != value)
            {
                _dniCliente = value;
                OnPropertyChanged(nameof(DniCliente));
                ValidateDni();
            }
        }
    }
    public string NombreCliente
    {
        get => _nombreCliente;
        set
        {
            if (_nombreCliente != value)
            {
                _nombreCliente = value;
                OnPropertyChanged(nameof(NombreCliente));
                ValidateNombre();
            }
        }
    }
    public string ApellidoCliente
    {
        get => _apellidoCliente;
        set
        {
            if (_apellidoCliente != value)
            {
                _apellidoCliente = value;
                OnPropertyChanged(nameof(ApellidoCliente));
                ValidateApellido();
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                HasError = !string.IsNullOrWhiteSpace(value);
                OnPropertyChanged(nameof(HasError));
            }
        }
    }
    public bool HasError
    {
        get => _hasError;
        set { _hasError = value; OnPropertyChanged(); }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }


    public string DniError
    {
        get => _dniError;
        set { _dniError = value; OnPropertyChanged(); }
    }

    public string NombreError
    {
        get => _nombreError;
        set { _nombreError = value; OnPropertyChanged(); }
    }

    public string ApellidoError
    {
        get => _apellidoError;
        set { _apellidoError = value; OnPropertyChanged(); }
    }

    private readonly IRepositoryCliente _repositoryCliente;
    public Action<Cliente>? OnClienteCreado;

    public CreateClienteViewModel(IRepositoryCliente repositoryCliente)
    {
        _repositoryCliente = repositoryCliente;
    }

    [RelayCommand]
    private async Task SaveCliente()
    {
        if (string.IsNullOrWhiteSpace(DniCliente) ||
           string.IsNullOrWhiteSpace(NombreCliente) ||
           string.IsNullOrWhiteSpace(ApellidoCliente))
        {
            ErrorMessage = "Complete todos los campos";
            return;
        }
        try
        {
            IsLoading = true;
            var cliente = new Cliente
            {
                Dni = DniCliente,
                Nombres = NombreCliente,
                Apellidos = ApellidoCliente
            };
            await _repositoryCliente.AddCliente(cliente);
            var clienteCreado = await _repositoryCliente.GetClienteByDni(DniCliente);
            if (clienteCreado != null)
            {
                OnClienteCreado?.Invoke(clienteCreado);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    //validaciones
    private void ValidateDni()
    {
        DniError = string.Empty;

        if (string.IsNullOrWhiteSpace(DniCliente))
        {
            DniError = "El DNI es obligatorio";
            return;
        }

        if (!DniCliente.All(char.IsDigit))
        {
            DniError = "Solo números";
            return;
        }

        if (DniCliente.Length > 8)
        {
            DniError = "Máximo 8 dígitos";
        }
    }

    private void ValidateNombre()
    {
        NombreError = string.Empty;

        if (string.IsNullOrWhiteSpace(NombreCliente))
        {
            NombreError = "El nombre es obligatorio";
            return;
        }

        if (!NombreCliente.All(c => char.IsLetter(c) || c == ' '))
        {
            NombreError = "Solo letras";
        }
    }

    private void ValidateApellido()
    {
        ApellidoError = string.Empty;
        if (string.IsNullOrWhiteSpace(ApellidoCliente))
        {
            ApellidoError = "El apellido es obligatorio";
            return;
        }

        if (!ApellidoCliente.All(c => char.IsLetter(c) || c == ' '))
        {
            ApellidoError = "Solo letras";
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
