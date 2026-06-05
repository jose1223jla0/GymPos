using GymPos.Models;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace GymPos.Converters;

public class RolToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Rol rol)
        {
            return rol switch
            {
                Rol.Administrador => new SolidColorBrush(Colors.MediumPurple),
                Rol.Recepcionista => new SolidColorBrush(Colors.MediumSeaGreen),
                Rol.SuperAdmin => new SolidColorBrush(Colors.Gold),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
