using GymPos.Repository;
using GymPos.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Linq;
using Microsoft.UI.Dispatching;

namespace GymPos.ViewModels;

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly IServiceCaja _cajaService;
    private readonly IRepositoryCliente _repositoryCliente;
    private readonly IRepositoryAsistencia _repositoryAsistencia;
    private readonly IRepositoryMembresia _repositoryMembresia;
    private readonly ICajaEventService _cajaEventService;
    private readonly IRepositoryVenta _repositoryVenta;
    private DispatcherQueue? _dispatcher;
    private Timer? _timer;

    public ObservableCollection<string> Actividades { get; } = new();

    public DashboardViewModel(
        IServiceCaja cajaService,
        IRepositoryAsistencia repositoryAsistencia,
        IRepositoryMembresia repositoryMembresia,
        IRepositoryCliente repositoryCliente,
        ICajaEventService cajaEventService,
        IRepositoryVenta repositoryVenta)
    {
        _cajaService = cajaService;
        _repositoryAsistencia = repositoryAsistencia;
        _repositoryMembresia = repositoryMembresia;
        _cajaEventService = cajaEventService;
        _repositoryCliente = repositoryCliente;
        _repositoryVenta = repositoryVenta;
        _cajaEventService.MovimientoRegistrado += OnCajaMovimiento;
    }

    private async void OnCajaMovimiento()
    {
        await CargarIngresosAsync();
    }


    private int _clientesCount;
    public int ClientesCount { get => _clientesCount; set { _clientesCount = value; OnPropertyChanged(); } }

    private decimal _ingresosHoy;
    public decimal IngresosHoy { get => _ingresosHoy; set { _ingresosHoy = value; OnPropertyChanged(); OnPropertyChanged(nameof(IngresosHoyStr)); } }
    public string IngresosHoyStr => IngresosHoy.ToString("C2");

    private int _asistenciasHoy;
    public int AsistenciasHoy { get => _asistenciasHoy; set { _asistenciasHoy = value; OnPropertyChanged(); } }

    private int _membresiasCount;
    public int MembresiasCount { get => _membresiasCount; set { _membresiasCount = value; OnPropertyChanged(); } }

    public async Task InitAsync()
    {
        _dispatcher = DispatcherQueue.GetForCurrentThread();
        await CargarDatosAsync();
        _timer = new Timer(async _ => await RefreshActivitiesAsyncWrapper(), null, 1000, 10000);
    }

    private async Task CargarDatosAsync()
    {
        await CargarIngresosAsync();
        await CargarClientesAsync();
        await CargarAsistenciasAsync();
        await CargarMembresiasAsync();
    }
    private async Task CargarIngresosAsync()
    {
        try
        {
            var resumen = await _cajaService.ObtenerResumenAsync();
            IngresosHoy = resumen.TotalIngresos;
        }
        catch
        {
            IngresosHoy = 0;
        }
    }
    private async Task CargarClientesAsync()
    {
        try
        {
            var totalClientes = await _repositoryCliente.CountClientes();
            ClientesCount = totalClientes;
        }
        catch
        {
            ClientesCount = 0;
        }
    }
    private async Task CargarAsistenciasAsync()
    {
        try
        {
            var totalAsistencia = await _repositoryAsistencia.ContarAsistenciasHoyAsync() ;
            AsistenciasHoy= totalAsistencia;
        }
        catch
        {
            AsistenciasHoy = 0;
        }
    }
    private async Task CargarMembresiasAsync()
    {
        try
        {
            var totalMembresias = await _repositoryMembresia.CountMembresiaAsync();
            MembresiasCount = totalMembresias;
        }
        catch
        {
            MembresiasCount = 0;
        }
    }
    private async Task RefreshActivitiesAsyncWrapper()
    {
        try
        {
            await RefreshActivitiesAsync();
        }
        catch
        {
            // ignore errors in periodic refresh
        }
    }

    private async Task RefreshActivitiesAsync()
    {
        var items = new List<string>();
        try
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var ventas = (await _repositoryVenta.ObtenerTodasAsync())
                        .Where(v => v.FechaVenta == hoy)
                        .OrderByDescending(v => v.FechaVenta)
                        .ThenByDescending(v => v.IdVenta)
                        .Take(2)
                        .ToList();
            foreach (var v in ventas)
            {
                var productos = v.VentaDetalles != null ? string.Join(", ", v.VentaDetalles.Select(d => d.Producto?.NombreProducto ?? $"#{d.IdProducto}")) : "";
                items.Add($"Venta: {productos} — {v.Total:C2}");
            }

            var asistencias = (await _repositoryAsistencia.GetAllAsistenciaAsync())
                                .OrderByDescending(a => a.Fecha)
                                .ThenByDescending(a => a.IdAsistencia)
                                .ToList();
            var ultima = asistencias.FirstOrDefault();
            if (ultima != null)
            {
                string nombreCliente = "Cliente desconocido";
                if (ultima.Suscripcion?.Cliente != null)
                {
                    nombreCliente = $"{ultima.Suscripcion.Cliente.Nombres} {ultima.Suscripcion.Cliente.Apellidos}";
                }
                else if (ultima.Suscripcion != null)
                {
                    var cli = await _repositoryCliente.GetById(ultima.Suscripcion.IdCliente);
                    if (cli != null) nombreCliente = $"{cli.Nombres} {cli.Apellidos}";
                }
                items.Add($"Asistencia: {nombreCliente} — {ultima.Fecha:dd/MM/yyyy}");
            }

            // Último cliente registrado (por Id mayor)
            var clientes = await _repositoryCliente.GetAll();
            var ultimoCliente = clientes.OrderByDescending(c => c.IdCliente).FirstOrDefault();
            if (ultimoCliente != null)
            {
                items.Add($"Cliente nuevo: {ultimoCliente.Nombres} {ultimoCliente.Apellidos}");
            }
        }
        catch
        {
            // ignore individual errors
        }
        if (_dispatcher != null)
        {
            _dispatcher.TryEnqueue(() =>
            {
                Actividades.Clear();
                foreach (var it in items)
                {
                    Actividades.Add(it);
                }
            });
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
