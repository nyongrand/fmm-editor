using System.Text;

namespace FMELibrary
{
    public static class BinaryParser
    {
        public static string ReadString(this BinaryReader reader, int length)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        public static System.Drawing.Color ReadColor(this BinaryReader reader)
        {
            short color = reader.ReadInt16();
            int red = color >> 10;
            int green = (color >> 5) & 0x1F;
            int blue = color & 0x1F;

            return System.Drawing.Color.FromArgb(red << 3, green << 3, blue << 3);
        }

        public static void WriteBytes(this BinaryWriter writer, bool value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, byte value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, short value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, int value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, float value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, byte[] value)
        {
            writer.Write(value);
        }

        public static void WriteBytes(this BinaryWriter writer, string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }

        public static void WriteBytes(this BinaryWriter writer, System.Drawing.Color color)
        {
            int red = color.R >> 3;
            int green = color.G >> 3;
            int blue = color.B >> 3;
            short s = (short)((red << 10) | (green << 5) | blue);
            writer.Write(s);
        }
    }
}
