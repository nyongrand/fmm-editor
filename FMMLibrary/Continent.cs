namespace FMELibrary
{
    public class Continent
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public byte Unknown1 { get; set; }
        public string CodeName { get; set; }
        public string Nationality { get; set; }
        public byte Unknown2 { get; set; }

        public Continent(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Name = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadByte();
            CodeName = reader.ReadString(reader.ReadInt32());
            Nationality = reader.ReadString(reader.ReadInt32());
            Unknown2 = reader.ReadByte();
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteBytes(Id);
            writer.WriteBytes(Name);
            writer.WriteBytes(Unknown1);
            writer.WriteBytes(CodeName);
            writer.WriteBytes(Nationality);
            writer.WriteBytes(Unknown2);
        }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }
    }
}
