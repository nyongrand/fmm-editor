namespace FMELibrary
{
    public class ExtraName
    {
        public string Name { get; set; }
        public short Unknown1 { get; set; }
        public byte Unknown2 { get; set; }

        public ExtraName(BinaryReader reader)
        {
            Name = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadInt16();
            Unknown2 = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteBytes(Name);
            writer.WriteBytes(Unknown1);
            writer.WriteBytes(Unknown2);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }
    }
}
