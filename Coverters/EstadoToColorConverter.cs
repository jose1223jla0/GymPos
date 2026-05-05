using GymPos.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace GymPos.Converters;

public class EstadoToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is EstadoSuscripcion estado)
        {
            return estado switch
            {
                EstadoSuscripcion.Activa => new SolidColorBrush(Colors.Green),
                EstadoSuscripcion.Vencida => new SolidColorBrush(Colors.Orange),
                EstadoSuscripcion.Cancelada => new SolidColorBrush(Colors.Red),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}