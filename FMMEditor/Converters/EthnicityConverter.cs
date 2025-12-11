using System;
using System.Globalization;
using System.Windows.Data;
using FMMLibrary;

namespace FMMEditor.Converters
{
    /// <summary>
    /// Converter to convert Ethnicity byte value to display string.
    /// </summary>
    public class EthnicityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte ethnicityValue)
            {
                return ToDisplayString(ethnicityValue);
            }
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts an ethnicity byte value to a display string.
        /// </summary>
        public static string ToDisplayString(byte ethnicity)
        {
            return (Ethnicity)ethnicity switch
            {
                Ethnicity.NorthenEuropean => "Northern European",
                Ethnicity.MediteranianHispanic => "Mediterranean/Hispanic",
                Ethnicity.NorthAfricanMiddleEastern => "North African/Middle Eastern",
                Ethnicity.AfricanCaribean => "African/Caribbean",
                Ethnicity.Asian => "Asian",
                Ethnicity.SouthEastAsian => "South East Asian",
                Ethnicity.PacificIslander => "Pacific Islander",
                Ethnicity.NativeAmerican => "Native American",
                Ethnicity.NativeAustralian => "Native Australian",
                Ethnicity.MixedRace => "Mixed Race",
                Ethnicity.EastAsian => "East Asian",
                Ethnicity.Unknown => "Unknown",
                _ => "-"
            };
        }
    }
}
