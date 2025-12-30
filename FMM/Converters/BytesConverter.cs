using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace FMM.Converters;

public class BytesConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "", StringComparison.Ordinal);
        }

        if (value is int[] ints)
        {
            var byteList = new List<byte>();
            foreach (var i in ints)
            {
                byteList.AddRange(BitConverter.GetBytes(i));
            }
            return BitConverter.ToString(byteList.ToArray()).Replace("-", "", StringComparison.Ordinal);
        }

        return value?.ToString() ?? "NULL";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string hexString)
            return BindingOperations.DoNothing;

        try
        {
            hexString = hexString.Replace(" ", "", StringComparison.Ordinal)
                .Replace("-", "", StringComparison.Ordinal);

            if (string.IsNullOrEmpty(hexString))
            {
                if (targetType == typeof(byte[]))
                    return Array.Empty<byte>();
                if (targetType == typeof(int[]))
                    return Array.Empty<int>();
            }

            if (hexString.Length % 2 != 0)
                return BindingOperations.DoNothing;

            var bytes = new byte[hexString.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            if (targetType == typeof(int[]))
            {
                if (bytes.Length % 4 != 0)
                    return BindingOperations.DoNothing;

                var ints = new int[bytes.Length / 4];
                for (int i = 0; i < ints.Length; i++)
                {
                    ints[i] = BitConverter.ToInt32(bytes, i * 4);
                }
                return ints;
            }

            return bytes;
        }
        catch
        {
            return BindingOperations.DoNothing;
        }
    }
}
