using System.Drawing;
using System.Text;

namespace FMELibrary
{
    /// <summary>
    /// Provides extension methods for reading and writing binary data with custom serialization logic.
    /// </summary>
    public static class BinaryParser
    {
        /// <summary>
        /// Reads a length-prefixed string from the binary reader.
        /// The first 4 bytes represent the string length.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A UTF-8 decoded string.</returns>
        public static string ReadStringEx(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Reads a 16-bit RGB565 color value and converts it to a <see cref="Color"/> object.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A <see cref="Color"/> object representing the RGB color.</returns>
        public static Color ReadColor(this BinaryReader reader)
        {
            short color = reader.ReadInt16();
            int red = color >> 10;
            int green = (color >> 5) & 0x1F;
            int blue = color & 0x1F;

            return Color.FromArgb(red << 3, green << 3, blue << 3);
        }

        public static DateOnly ReadDate(this BinaryReader reader)
        {
            int dayOfYear = reader.ReadInt16();
            int year = reader.ReadInt16();
            if (dayOfYear == -1 || year == -1)
                return DateOnly.MinValue;

            return new DateOnly(year, 1, 1).AddDays(dayOfYear - 1);
        }

        /// <summary>
        /// Writes various data types to the binary writer with automatic type detection and appropriate serialization.
        /// </summary>
        /// <param name="writer">The binary writer to write to.</param>
        /// <param name="value">The value to write. Supported types: byte, short, int, float, byte[], string, Color, and null.</param>
        /// <exception cref="InvalidOperationException">Thrown when the value type is not supported.</exception>
        public static void WriteEx(this BinaryWriter writer, object? value)
        {
            switch (value)
            {
                case bool b:
                    writer.Write(b);
                    break;

                case byte b:
                    writer.Write(b);
                    break;

                case short s:
                    writer.Write(s);
                    break;

                case int i:
                    writer.Write(i);
                    break;

                case long l:
                    writer.Write(l);
                    break;

                case float f:
                    writer.Write(f);
                    break;

                case byte[] b:
                    writer.Write(b);
                    break;

                case string s:
                    var bytes = Encoding.UTF8.GetBytes(s);
                    writer.Write(bytes.Length);
                    writer.Write(bytes);
                    break;

                case Color c:
                    int red = c.R >> 3;
                    int green = c.G >> 3;
                    int blue = c.B >> 3;
                    writer.Write((short)((red << 10) | (green << 5) | blue));
                    break;

                case DateOnly d:
                    int dayOfYear = d == DateOnly.MinValue ? -1 : d.DayOfYear;
                    int year = d == DateOnly.MinValue ? -1 : d.Year;
                    writer.Write((short)dayOfYear);
                    writer.Write((short)year);
                    break;

                case null:
                    break;

                default:
                    throw new InvalidOperationException($"Unsupported type: {value?.GetType().FullName}");
            }
        }
    }
}
