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

        public static byte[] GetBytes(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return BitConverter.GetBytes(bytes.Length).Concat(bytes).ToArray();
        }

        //public static Color ReadColor(this BinaryReader reader)
        //{
        //    int color = reader.ReadInt16();
        //    return Color.FromArgb(color / (1 << 10), color % (1 << 10) / (1 << 5), color % (1 << 5));
        //}

        //public static byte[] GetBytes(this Color color)
        //{
        //    short s = (color.R << 10) | (color.G << 5) | color.B;

        //    var bytes = Encoding.UTF8.GetBytes(text);
        //    return BitConverter.GetBytes(bytes.Length).Concat(bytes).ToArray();
        //}
    }
}
