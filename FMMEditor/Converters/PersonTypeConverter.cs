using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter for person type byte to human-readable string.
    /// </summary>
    public class PersonTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte type)
            {
                return type switch
                {
                    0 => "Player",
                    1 => "Staff",
                    _ => $"Other ({type})"
                };
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str switch
                {
                    "Player" => (byte)0,
                    "Staff" => (byte)1,
                    _ => (byte)2
                };
            }
            return (byte)0;
        }
    }
}
