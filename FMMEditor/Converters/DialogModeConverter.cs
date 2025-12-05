using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter to determine dialog mode (Add vs Edit) based on SelectedId.
    /// SelectedId == -1 means Add mode, otherwise Edit mode.
    /// </summary>
    public class DialogModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter?.ToString();
            bool isAddMode = value is int id && id == -1;

            return param switch
            {
                "Title" => isAddMode ? "ADD NAMES" : "EDIT NAME",
                "Height" => isAddMode ? 460 : 380,
                "AddVisibility" => isAddMode ? Visibility.Visible : Visibility.Collapsed,
                "EditVisibility" => isAddMode ? Visibility.Collapsed : Visibility.Visible,
                _ => value
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
