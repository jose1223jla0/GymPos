using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class EstadoProductoConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool estado)
        {
            return estado ? "Activo" : "Inactivo";
        }

        return "Inactivo";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
