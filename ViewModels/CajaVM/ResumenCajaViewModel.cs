using GymPos.Models;
using GymPos.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GymPos.ViewModels.CajaVM;

public partial class ResumenCajaViewModel : INotifyPropertyChanged
{
    private readonly IServiceCaja _cajaService;
    private readonly ICajaEventService _cajaEventService;
    private Caja? _cajaActual;

    public ResumenCajaViewModel(IServiceCaja cajaService, ICajaEventService cajaEventService)
    {
        _cajaService = cajaService;
        _cajaEventService = cajaEventService;
        _cajaEventService.MovimientoRegistrado += OnMovimientoRegistrado;
    }

    private async void OnMovimientoRegistrado()
    {
        await CargarCajaActivaAsync();
    }

    public Caja? CajaActual
    {
        get => _cajaActual;
        set
        {
            _cajaActual = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(MontoInicial));
            OnPropertyChanged(nameof(TotalIngresos));
            OnPropertyChanged(nameof(TotalEgresos));
            OnPropertyChanged(nameof(SaldoActual));
            OnPropertyChanged(nameof(MontoInicialStr));
            OnPropertyChanged(nameof(TotalIngresosStr));
            OnPropertyChanged(nameof(TotalEgresosStr));
            OnPropertyChanged(nameof(SaldoActualStr));
            OnPropertyChanged(nameof(Movimientos));
            OnPropertyChanged(nameof(CajaAbierta));
            OnPropertyChanged(nameof(CajaCerrada));
            OnPropertyChanged(nameof(FechaApertura));
            OnPropertyChanged(nameof(FechaAperturaStr));
        }
    }

    // ────────── DERIVADOS (decimal) ──────────
    public decimal MontoInicial => CajaActual?.MontoInicial ?? 0;
    public decimal TotalIngresos => CajaActual?.Movimientos.Where(m => m.EsIngreso()).Sum(m => m.Monto) ?? 0;
    public decimal TotalEgresos => CajaActual?.Movimientos.Where(m => m.EsEgreso()).Sum(m => m.Monto) ?? 0;
    public decimal SaldoActual => CajaActual?.CalcularSaldo() ?? 0;
    public DateTime? FechaApertura => CajaActual?.FechaApertura;
    // ────────── DERIVADOS FORMATEADOS (para binding XAML) ──────────
    public string MontoInicialStr => MontoInicial.ToString("C2");
    public string TotalIngresosStr => TotalIngresos.ToString("C2");
    public string TotalEgresosStr => TotalEgresos.ToString("C2");
    public string SaldoActualStr => SaldoActual.ToString("C2");
    public string FechaAperturaStr => FechaApertura?.ToString("dddd d 'de' MMMM HH:mm") ?? "";
    // ────────── ESTADO ──────────
    public bool CajaAbierta => CajaActual?.Abierta == true;
    public bool CajaCerrada => !CajaAbierta;
    // ────────── MOVIMIENTOS ──────────
    public IEnumerable<MovimientoCaja> Movimientos =>
        CajaActual?.Movimientos ?? Enumerable.Empty<MovimientoCaja>();

    // ────────── OPERACIONES ──────────
    public async Task CargarCajaActivaAsync()
    {
        try
        {
            CajaActual = await _cajaService.ObtenerCajaAbiertaAsync();
        }
        catch (InvalidOperationException)
        {
            CajaActual = null;
        }
    }

    public async Task AbrirCajaAsync(decimal monto)
    {
        CajaActual = await _cajaService.AbrirCajaAsync(monto);
    }

    public async Task CerrarCajaAsync()
    {
        if (CajaActual == null)
        {
            return;
        }

        try
        {
            // Cerrar la caja en el servicio; una vez cerrada no hay caja activa
            await _cajaService.CerrarCajaAsync();
            CajaActual = null;
        }
        catch (InvalidOperationException)
        {
            CajaActual = null;
        }
    }

    // ────────── INotifyPropertyChanged ──────────
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}