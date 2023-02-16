using System.Drawing;

namespace FMELibrary
{
    public class Kit
    {
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public Color[] Colors { get; set; }

        public Kit(BinaryReader reader)
        {
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();

            Colors = new Color[10];
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = reader.ReadColor();
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Unknown2);

            for (int i = 0; i < Colors.Length; i++)
                writer.WriteEx(Colors[i]);
        }
    }
}
