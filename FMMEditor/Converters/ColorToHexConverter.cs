using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:X2}{1:X2}{2:X2}", color.R, color.G, color.B);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex)
            {
                var trimmed = hex.Trim();
                if (trimmed.StartsWith("#", StringComparison.Ordinal))
                {
                    trimmed = trimmed[1..];
                }

                if (trimmed.Length == 6 && int.TryParse(trimmed, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
                {
                    var r = (byte)((rgb >> 16) & 0xFF);
                    var g = (byte)((rgb >> 8) & 0xFF);
                    var b = (byte)(rgb & 0xFF);
                    return Color.FromArgb(r, g, b);
                }
            }

            return Binding.DoNothing;
        }
    }
}
