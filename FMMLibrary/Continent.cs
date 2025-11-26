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

        public Continent(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Name = reader.ReadStringEx();
            Unknown1 = reader.ReadByte();
            CodeName = reader.ReadStringEx();
            Nationality = reader.ReadStringEx();
            Unknown2 = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Id);
            writer.WriteEx(Name);
            writer.WriteEx(Unknown1);
            writer.WriteEx(CodeName);
            writer.WriteEx(Nationality);
            writer.WriteEx(Unknown2);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }
    }
}
