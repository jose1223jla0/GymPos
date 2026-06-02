using System;

namespace GymPos.Services;
public interface ICajaEventService
{
    event Action? MovimientoRegistrado;
    void NotificarMovimiento();
}
public class CajaEventService: ICajaEventService
{
    public event Action? MovimientoRegistrado;
    public void NotificarMovimiento() => MovimientoRegistrado?.Invoke();
}
