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
            if (value is byte gender)
            {
                var param = parameter?.ToString();
                
                if (param == "Symbol")
                {
                    // 0 = Male, 1 = Female
                    // Using full-width symbols for better compatibility
                    return gender == 0 ? "\u2642" : "\u2640";
                }
                else if (param == "Color")
                {
                    // Blue for Male, Pink for Female
                    return gender == 0 
                        ? new SolidColorBrush(Color.FromRgb(33, 150, 243))  // Blue
                        : new SolidColorBrush(Color.FromRgb(233, 30, 99));   // Pink
                }
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
