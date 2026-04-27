using Microsoft.UI.Xaml.Controls;

namespace GymPos.Services;

public interface INavigationService
{
    void Initialize(Frame frame);
    void Navigate<T>() where T : Page;
    void GoBack();
}

public class NavegationService : INavigationService
{
    private Frame? _frame;
    public void GoBack()
    {
        if (_frame!.CanGoBack)
        {
            _frame.GoBack();
        }
    }

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public void Navigate<T>() where T : Page
    {
        _frame!.Navigate(typeof(T));
    }
}
