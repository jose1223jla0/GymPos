using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class PrecioConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is decimal precio)
        {
            return $"S/ {precio:N2}";
        }

        return "S/ 0.00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
