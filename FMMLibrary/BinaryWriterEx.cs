using System.Drawing;
using System.Text;

namespace FMMLibrary
{
    public class BinaryWriterEx(Stream output) : BinaryWriter(output, Encoding.UTF8, false)
    {
        public override void Write(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            Write(bytes.Length);
            Write(bytes);
        }

        public void Write(Color value)
        {
            int red = value.R >> 3;
            int green = value.G >> 3;
            int blue = value.B >> 3;

            Write((ushort)((red << 10) | (green << 5) | blue));
        }

        public void Write(DateOnly date)
        {
            if (date == DateOnly.MinValue)
            {
                Write((short)-1);
                Write((short)-1);
                return;
            }

            // Calculate 0-based day of year (0 = Jan 1)
            int dayOfYear = date.DayOfYear - 1;
            Write((short)dayOfYear);
            Write((short)date.Year);
        }
    }
}
