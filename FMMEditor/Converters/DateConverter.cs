using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter to convert game date integer to DateTime and back.
    /// The game stores dates as: lower 16 bits = day of year, upper 16 bits = year.
    /// </summary>
    public class DateConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int date && date > 0)
            {
                try
                {
                    short days = (short)(date & 0xFFFF);
                    short year = (short)((date >> 16) & 0xFFFF);
                    return new DateTime(year, 1, 1).AddDays(days);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                int days = date.DayOfYear - 1; // Days since Jan 1
                int year = date.Year;
                return (year << 16) | (days & 0xFFFF);
            }
            return 0;
        }

        /// <summary>
        /// Converts a game date integer to a formatted string.
        /// </summary>
        public static string ToDateString(int date)
        {
            if (date <= 0) return "-";
            try
            {
                short days = (short)(date & 0xFFFF);
                short year = (short)((date >> 16) & 0xFFFF);
                return new DateTime(year, 1, 1).AddDays(days).ToString("yyyy-MM-dd");
            }
            catch
            {
                return "-";
            }
        }

        /// <summary>
        /// Converts a game date integer to a DateTime.
        /// </summary>
        public static DateTime? ToDateTime(int date)
        {
            if (date <= 0) return null;
            try
            {
                short days = (short)(date & 0xFFFF);
                short year = (short)((date >> 16) & 0xFFFF);
                return new DateTime(year, 1, 1).AddDays(days);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a DateTime to a game date integer.
        /// </summary>
        public static int FromDateTime(DateTime? date)
        {
            if (date == null) return 0;
            int days = date.Value.DayOfYear - 1; // Days since Jan 1
            int year = date.Value.Year;
            return (year << 16) | (days & 0xFFFF);
        }
    }
}
