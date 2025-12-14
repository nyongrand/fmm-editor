using System;
using System.Globalization;
using System.Windows.Data;

namespace FMMEditor.Converters
{
    public class BytesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
            {
                return BitConverter.ToString(bytes).Replace("-", "");
            }
            else if (value is int[] ints)
            {
                var byteList = new System.Collections.Generic.List<byte>();
                foreach (var i in ints)
                {
                    byteList.AddRange(BitConverter.GetBytes(i));
                }
                return BitConverter.ToString(byteList.ToArray()).Replace("-", "");
            }

            return value?.ToString() ?? "NULL";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string hexString)
                return null;

            try
            {
                // Remove spaces and dashes
                hexString = hexString.Replace(" ", "").Replace("-", "");

                if (string.IsNullOrEmpty(hexString))
                {
                    if (targetType == typeof(byte[]))
                        return Array.Empty<byte>();
                    else if (targetType == typeof(int[]))
                        return Array.Empty<int>();
                }

                // Convert hex string to bytes
                var bytes = new byte[hexString.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                // If target is int array, convert bytes to ints
                if (targetType == typeof(int[]))
                {
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
                // Return null on error, which will keep the original value
                return null;
            }
        }
    }
}
