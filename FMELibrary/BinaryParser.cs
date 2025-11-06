using System.Drawing;
using System.Text;

namespace FMELibrary
{
    /// <summary>
    /// Provides extension methods for reading and writing binary data with custom serialization logic.
    /// </summary>
    public static class BinaryParser
    {
        public static string ReadStringEx(this BinaryReader reader)
        {
            int len = reader.ReadInt32();
            return reader.ReadString(len);
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
                case byte b:
                    writer.Write(b);
                    break;
                case short s:
                    writer.Write(s);
                    break;
                case int i:
                    writer.Write(i);
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
                case null:
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported type: {value?.GetType().FullName}");
            }
        }
    }
}
