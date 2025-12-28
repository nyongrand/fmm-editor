using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FMM.Converters;

public class GenderToSymbolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte genderByte)
            return genderByte == 0 ? "M" : genderByte == 1 ? "F" : "?";

        if (value is int genderInt)
            return genderInt == 0 ? "M" : genderInt == 1 ? "F" : "?";

        return "?";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => BindingOperations.DoNothing;
}
