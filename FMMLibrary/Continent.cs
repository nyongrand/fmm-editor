namespace FMMLibrary
{
    public class Continent
    {
        public short Id { get; set; }
        public string Name { get; set; }
        public byte Unknown1 { get; set; }
        public string CodeName { get; set; }
        public string Nationality { get; set; }
        public byte Unknown2 { get; set; }

        public Continent(BinaryReaderEx reader)
        {
            Id = reader.ReadInt16();
            Name = reader.ReadString();
            Unknown1 = reader.ReadByte();
            CodeName = reader.ReadString();
            Nationality = reader.ReadString();
            Unknown2 = reader.ReadByte();
        }

        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Name);
            writer.Write(Unknown1);
            writer.Write(CodeName);
            writer.Write(Nationality);
            writer.Write(Unknown2);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }
    }
}
