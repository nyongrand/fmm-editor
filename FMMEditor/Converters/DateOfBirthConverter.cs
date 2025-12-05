using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter to convert game date integer to DateTime and back.
    /// The game stores dates as days since a base date (typically 1900-01-01 or similar).
    /// </summary>
    public class DateOfBirthConverter : IValueConverter
    {
        // Base date for the game's date system (adjust if needed based on actual game format)
        private static readonly DateTime BaseDate = new(1900, 1, 1);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int days && days > 0)
            {
                try
                {
                    return BaseDate.AddDays(days);
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
                return (int)(date - BaseDate).TotalDays;
            }
            return 0;
        }

        /// <summary>
        /// Converts a game date integer to a formatted string.
        /// </summary>
        public static string ToDateString(int days)
        {
            if (days <= 0) return "-";
            try
            {
                return BaseDate.AddDays(days).ToString("dd/MM/yyyy");
            }
            catch
            {
                return "-";
            }
        }

        /// <summary>
        /// Converts a game date integer to a DateTime.
        /// </summary>
        public static DateTime? ToDateTime(int days)
        {
            if (days <= 0) return null;
            try
            {
                return BaseDate.AddDays(days);
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
            return (int)(date.Value - BaseDate).TotalDays;
        }
    }
}
