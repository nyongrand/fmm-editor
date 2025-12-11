using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    public class BooleanToShortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short shortValue)
            {
                return shortValue == 1;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return (short)(boolValue ? 1 : 0);
            }
            return (short)0;
        }
    }
}
