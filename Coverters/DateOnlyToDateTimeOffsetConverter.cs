using Microsoft.UI.Xaml.Data;
using System;

namespace GymPos.Converters
{
    public class DateOnlyToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateOnly d)
            {
                // CalendarDatePicker uses Nullable<DateTimeOffset>
                return new DateTimeOffset(d.ToDateTime(TimeOnly.MinValue));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dto)
            {
                return DateOnly.FromDateTime(dto.DateTime);
            }
            if (value is DateTimeOffset?)
            {
                var nd = (DateTimeOffset?)value;
                if (nd.HasValue) return DateOnly.FromDateTime(nd.Value.DateTime);
            }
            return DateOnly.FromDateTime(DateTime.Today);
        }
    }
}
