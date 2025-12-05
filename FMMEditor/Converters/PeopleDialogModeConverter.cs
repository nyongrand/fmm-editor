using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter to determine dialog mode (Add vs Edit) based on SelectedUid.
    /// SelectedUid == -1 means Add mode, otherwise Edit mode.
    /// </summary>
    public class PeopleDialogModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter?.ToString();
            bool isAddMode = value is int uid && uid == -1;

            return param switch
            {
                "Title" => isAddMode ? "ADD PERSON" : "EDIT PERSON",
                "Height" => isAddMode ? 620 : 620,
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
