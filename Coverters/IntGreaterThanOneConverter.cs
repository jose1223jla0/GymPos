using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class IntGreaterThanOneConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int i) return i > 1;
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
