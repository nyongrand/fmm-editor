using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace FMMEditor.Converters
{
    public class GenderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isWoman = false;
            if (value is byte gender)
                isWoman = gender == 1;

            if (value is bool woman)
                isWoman = woman;

            var param = parameter?.ToString();
            if (param == "Symbol")
            {
                // Using full-width symbols for better compatibility
                return isWoman ? "\u2640" : "\u2642";
            }
            else if (param == "Color")
            {
                // Blue for Male, Pink for Female
                return isWoman
                    ? new SolidColorBrush(Color.FromRgb(233, 30, 99))   // Pink
                    : new SolidColorBrush(Color.FromRgb(33, 150, 243));  // Blue
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
