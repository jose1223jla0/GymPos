using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace GymPos.Converters;

public class StockToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int stock)
        {
            // rojo si stock <= 5, amarillo si entre 1 y 5, verde si >5
            if (stock <= 5) return new SolidColorBrush(Colors.Red);
            return new SolidColorBrush(Colors.Green);
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
