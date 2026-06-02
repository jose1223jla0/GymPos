using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class InvertedBoolToVisConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility v)
            return v == Visibility.Visible ? false : true;
        throw new NotImplementedException();
    }
}
