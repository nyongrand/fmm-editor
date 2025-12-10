using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter to calculate age from a date of birth integer.
    /// </summary>
    public class AgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int dateOfBirth && dateOfBirth > 0)
            {
                var birthDate = DateConverter.ToDateTime(dateOfBirth);
                if (birthDate == null) return 0;

                var today = DateTime.Today;
                var age = today.Year - birthDate.Value.Year;

                if (birthDate.Value.Date > today.AddYears(-age)) age--;

                return age;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
