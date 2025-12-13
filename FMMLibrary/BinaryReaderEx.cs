using System.Drawing;
using System.Text;

namespace FMMLibrary
{
    public class BinaryReaderEx(Stream input) : BinaryReader(input, Encoding.UTF8, false)
    {
        /// <summary>
        /// Reads a length-prefixed string from the binary reader.
        /// The first 4 bytes represent the string length.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A UTF-8 decoded string.</returns>
        public override string ReadString()
        {
            int length = ReadInt32();
            var bytes = ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Reads a 16-bit RGB565 color value and converts it to a <see cref="Color"/> object.
        /// </summary>
        /// <param name="reader">The binary reader to read from.</param>
        /// <returns>A <see cref="Color"/> object representing the RGB color.</returns>
        public Color ReadColor()
        {
            short color = ReadInt16();
            int red = color >> 10;
            int green = (color >> 5) & 0x1F;
            int blue = color & 0x1F;

            return Color.FromArgb(red << 3, green << 3, blue << 3);
        }

        public DateOnly ReadDate()
        {
            int storedDay = ReadInt16();
            int year = ReadInt16();
            if (storedDay == -1 || year == -1)
                return DateOnly.MinValue;

            // storedDay is 0-based day of year (0 = Jan 1)
            return new DateOnly(year, 1, 1).AddDays(storedDay);
        }
    }
}
