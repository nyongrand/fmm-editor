using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    internal class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                byte b => $"{b:X2}",
                short s => $"{s:X4}",
                ushort us => $"{us:X4}",
                int i => $"{i:X8}",
                uint ui => $"{ui:X8}",
                long l => $"{l:X16}",
                ulong ul => $"{ul:X16}",
                _ => value?.ToString() ?? ""
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
