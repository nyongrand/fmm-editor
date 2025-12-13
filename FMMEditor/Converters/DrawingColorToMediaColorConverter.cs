using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FMMEditor.Converters
{
    public class DrawingColorToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Drawing.Color drawingColor)
            {
                return Color.FromRgb(drawingColor.R, drawingColor.G, drawingColor.B);
            }
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color mediaColor)
            {
                return System.Drawing.Color.FromArgb(mediaColor.R, mediaColor.G, mediaColor.B);
            }
            return System.Drawing.Color.Black;
        }
    }
}
