using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class IntLessThanTotalPagesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int current && parameter is string paramStr && int.TryParse(paramStr, out int total))
        {
            return current < total;
        }
        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
