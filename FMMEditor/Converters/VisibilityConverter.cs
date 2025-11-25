using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (parameter as string) switch
            {
                "Object" => value is null ? Visibility.Collapsed : Visibility.Visible,
                "Boolean" => (bool?)value == true ? Visibility.Visible : Visibility.Collapsed,
                "Inverter" => (bool?)value == true ? Visibility.Collapsed : Visibility.Visible,
                "Text" => string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible,
                "TextInvert" => string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed,
                "Margin" => ((Thickness)value).Top != 0 ? Visibility.Visible : Visibility.Collapsed,
                _ => (object)((bool?)value == true ? Visibility.Visible : Visibility.Collapsed),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
