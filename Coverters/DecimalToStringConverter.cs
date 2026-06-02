using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class DecimalToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        value is decimal d ? $"S/ {d:F2}" : "S/ 0.00";

    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        throw new NotImplementedException();
}
