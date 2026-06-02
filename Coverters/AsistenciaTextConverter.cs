using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class AsistenciaTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool asistioHoy)
        {
            return asistioHoy ? "Registrada" : "Registrar";
        }
        return "Registrar Asistencia";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
