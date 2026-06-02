using GymPos.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters;

public class NotificationTypeToInfoBarSeverityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Success,
                NotificationType.Error => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Error,
                NotificationType.Warning => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Warning,
                NotificationType.Informational => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational,
                _ => Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational,
            };
        }
        return Microsoft.UI.Xaml.Controls.InfoBarSeverity.Informational;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
