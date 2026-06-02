using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class ZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            int i => i == 0 ? Visibility.Visible : Visibility.Collapsed,
            long l => l == 0 ? Visibility.Visible : Visibility.Collapsed,
            double d => Math.Abs(d) < double.Epsilon ? Visibility.Visible : Visibility.Collapsed,
            float f => Math.Abs(f) < float.Epsilon ? Visibility.Visible : Visibility.Collapsed,
            decimal m => m == 0m ? Visibility.Visible : Visibility.Collapsed,
            _ => Visibility.Collapsed
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
