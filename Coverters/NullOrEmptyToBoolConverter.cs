using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class NullOrEmptyToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string s)
            return !string.IsNullOrEmpty(s);
        return value is not null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
