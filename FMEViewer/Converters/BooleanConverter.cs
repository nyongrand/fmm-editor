using System;
using System.Globalization;
using System.Windows.Data;

namespace FMEViewer.Converters
{
    internal class BooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (parameter as string) switch
            {
                null => (bool?)value == true,
                "Invert" => (bool?)value is null || (bool?)value == false,
                "Object" => value is null,
                "ObjectInvert" => value is not null,
                "String" => string.IsNullOrEmpty(value as string),
                "StringInvert" => !string.IsNullOrEmpty(value as string),
                "Int" => (int?)value is not null && (int?)value != 0,
                "IntInvert" => (int?)value is null || (int?)value == 0,
                _ => throw new NotImplementedException()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
