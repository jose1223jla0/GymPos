using CommunityToolkit.Mvvm.Input;
using GymPos.Models;
using GymPos.Repository;
using GymPos.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.SuscripcionesVM;

public partial class CreateSuscripcionViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Membresia> MembresiaList { get; } = new();
    public ObservableCollection<Cliente> ClientesSugeridos { get; } = new();
    private readonly IServiceSuscripcion _serviceSuscripcion;
    private readonly IRepositoryCliente _repositoryCliente;
    private Cliente? _clienteSeleccionado;
    private Membresia? _membresiaSeleccionado;
    private DateOnly _fechaInicio = DateOnly.FromDateTime(DateTime.Today);
    private DateOnly _fechaFin = DateOnly.FromDateTime(DateTime.Today);
    private bool _isPaseDiario;
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private string _textoBusqueda = string.Empty;
    private bool _mostrarSugerencias;
    // ── Propiedades ──────────────────────────────────────────────────────────
    public Cliente? ClienteSeleccionado
    {
        get => _clienteSeleccionado;
        set
        {
            if (_clienteSeleccionado != value)
            {
                _clienteSeleccionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneClienteSeleccionado));
            }
        }
    }
    public bool TieneClienteSeleccionado => _clienteSeleccionado != null;

    public Membresia? MembresiaSeleccionado
    {
        get => _membresiaSeleccionado;
        set
        {
            if (_membresiaSeleccionado != value)
            {
                _membresiaSeleccionado = value;
                _isPaseDiario = _membresiaSeleccionado != null && _membresiaSeleccionado.Sesiones == 1;
                if (_membresiaSeleccionado != null)
                {
                    if (!_isPaseDiario)
                    {
                        FechaFin = FechaInicio.AddDays(_membresiaSeleccionado.Sesiones);
                    }
                    else
                    {
                        FechaFin = FechaInicio;
                    }
                }
                else
                {
                    FechaFin = DateOnly.FromDateTime(DateTime.Today);
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsPaseDiario));
                OnPropertyChanged(nameof(FechaPickersEnabled));
                OnPropertyChanged(nameof(FechaFinIsEditable));
            }
        }
    }
    public DateOnly FechaInicio
    {
        get => _fechaInicio;
        set
        {
            if (_fechaInicio != value)
            {
                _fechaInicio = value;
                OnPropertyChanged();
                if (MembresiaSeleccionado != null && !_isPaseDiario)
                {
                    FechaFin = _fechaInicio.AddDays(MembresiaSeleccionado.Sesiones);
                }
                else
                {
                    FechaFin = _fechaInicio;
                }
            }
        }
    }

    public DateOnly FechaFin
    {
        get => _fechaFin;
        set { if (_fechaFin != value) { _fechaFin = value; OnPropertyChanged(); } }
    }

    public bool IsPaseDiario => _isPaseDiario;
    public bool FechaPickersEnabled => MembresiaSeleccionado != null && !_isPaseDiario;
    public bool FechaFinIsEditable => false;

    public bool IsLoading
    {
        get => _isLoading;
        set { if (_isLoading != value) { _isLoading = value; OnPropertyChanged(); } }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
    }

    public string TextoBusqueda
    {
        get => _textoBusqueda;
        set { if (_textoBusqueda != value) { _textoBusqueda = value; OnPropertyChanged(); } }
    }

    public bool MostrarSugerencias
    {
        get => _mostrarSugerencias;
        set { if (_mostrarSugerencias != value) { _mostrarSugerencias = value; OnPropertyChanged(); } }
    }

    // ── Eventos ──────────────────────────────────────────────────────────────

    public event Action? OnAbrirFormulario;
    public event Action? OnSuscripcionCreada;

    // ── Constructor ──────────────────────────────────────────────────────────

    public CreateSuscripcionViewModel(
        IServiceSuscripcion serviceSuscripcion,
        IRepositoryCliente repositoryCliente)
    {
        _serviceSuscripcion = serviceSuscripcion;
        _repositoryCliente = repositoryCliente;
    }

    // ── Comandos ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Llamado desde el code-behind cuando el texto del AutoSuggestBox cambia
    /// (con debounce aplicado externamente).
    /// </summary>
    public async Task BuscarClientesAsync(string query)
    {
        ClientesSugeridos.Clear();

        if (ClienteSeleccionado != null && !string.IsNullOrWhiteSpace(query) && ClienteSeleccionado.Dni == query)
        {
            MostrarSugerencias = false;
            return;
        }

        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
        {
            MostrarSugerencias = false;
            return;
        }

        try
        {
            var resultados = await _repositoryCliente.BuscarClientes(query);
            foreach (var c in resultados)
                ClientesSugeridos.Add(c);

            MostrarSugerencias = ClientesSugeridos.Count > 0;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al buscar: {ex.Message}";
        }
    }

    /// <summary>
    /// Selecciona un cliente de la lista de sugerencias.
    /// </summary>
    public void SeleccionarCliente(Cliente cliente)
    {
        ClienteSeleccionado = cliente;
        TextoBusqueda = cliente.Dni;
        MostrarSugerencias = false;
        ClientesSugeridos.Clear();
    }

    /// <summary>
    /// Llamado cuando el modal de nuevo cliente termina con éxito.
    /// Selecciona automáticamente el cliente recién registrado.
    /// </summary>
    public void SeleccionarClienteNuevo(Cliente cliente)
    {
        ClienteSeleccionado = cliente;
        TextoBusqueda = cliente.Dni;
        ClientesSugeridos.Clear();
        MostrarSugerencias = false;
    }

    [RelayCommand]
    private async Task CreateSuscripcion()
    {
        if (ClienteSeleccionado == null)
        {
            ErrorMessage = "Selecciona un cliente antes de continuar.";
            return;
        }
        if (MembresiaSeleccionado == null)
        {
            ErrorMessage = "Selecciona una membresía.";
            return;
        }
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            await _serviceSuscripcion.CrearSuscripcion( ClienteSeleccionado.IdCliente,  MembresiaSeleccionado.IdMembresia);
            LimpiarFormulario();
            OnSuscripcionCreada?.Invoke();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void AbrirFormulario() => OnAbrirFormulario?.Invoke();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void LimpiarFormulario()
    {
        ClienteSeleccionado = null;
        MembresiaSeleccionado = null;
        TextoBusqueda = string.Empty;
        FechaInicio = DateOnly.FromDateTime(DateTime.Today);
        FechaFin = DateOnly.FromDateTime(DateTime.Today);
        ErrorMessage = string.Empty;
        ClientesSugeridos.Clear();
        MostrarSugerencias = false;
    }

    // ── INotifyPropertyChanged ────────────────────────────────────────────────

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}