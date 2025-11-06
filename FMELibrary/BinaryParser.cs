using System.Drawing;
using System.Text;

namespace FMELibrary
{
    public static class BinaryParser
    {
        public static string ReadString(this BinaryReader reader, int length)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        public static string ReadStringEx(this BinaryReader reader)
        {
            int len = reader.ReadInt32();
            return reader.ReadString(len);
        }

        public static Color ReadColor(this BinaryReader reader)
        {
            short color = reader.ReadInt16();
            int red = color >> 10;
            int green = (color >> 5) & 0x1F;
            int blue = color & 0x1F;

            return Color.FromArgb(red << 3, green << 3, blue << 3);
        }

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
