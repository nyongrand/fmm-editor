using System;
using System.Globalization;
using System.Windows.Data;

namespace FMEViewer.Converters
{
    internal class BytesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
                return BitConverter.ToString(bytes);

            return value?.ToString() ?? "NULL";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
