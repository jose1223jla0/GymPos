using System;

namespace GymPos.Services;
public interface ICajaEventService
{
    event Action? MovimientoRegistrado;
    event Action? CajaCerrada;
    event Action? CajaAperturada;
    void NotificarMovimiento();
    void NotificarCajaCerrada();
    void NotificarCajaAperturada();
}
public class CajaEventService: ICajaEventService
{
    public event Action? MovimientoRegistrado;
    public event Action? CajaCerrada;
    public event Action? CajaAperturada;
    public void NotificarMovimiento() => MovimientoRegistrado?.Invoke();
    public void NotificarCajaCerrada() => CajaCerrada?.Invoke();
    public void NotificarCajaAperturada() => CajaAperturada?.Invoke();
}
