using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    internal class HexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = value switch
            {
                byte b => BitConverter.ToString([b]),
                short s => BitConverter.ToString(BitConverter.GetBytes(s)),
                ushort us => BitConverter.ToString(BitConverter.GetBytes(us)),
                int i => BitConverter.ToString(BitConverter.GetBytes(i)),
                uint ui => BitConverter.ToString(BitConverter.GetBytes(ui)),
                long l => BitConverter.ToString(BitConverter.GetBytes(l)),
                ulong ul => BitConverter.ToString(BitConverter.GetBytes(ul)),
                _ => value?.ToString() ?? ""
            };
            return hex.Replace("-", "");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
