namespace FMMLibrary
{
    public class Language
    {
        public short Id { get; set; }
        public int Uid { get; set; }
        public string Name { get; set; }
        public string OtherName { get; set; }
        public short NationId { get; set; }

        /// <summary>
        /// Language difficulty level (1-20)
        /// </summary>
        public byte Difficulty { get; set; }

        public Language(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();
            Name = reader.ReadStringEx();
            OtherName = reader.ReadStringEx();
            NationId = reader.ReadInt16();
            Difficulty = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Id);
            writer.WriteEx(Uid);
            writer.WriteEx(Name);
            writer.WriteEx(OtherName);
            writer.WriteEx(NationId);
            writer.WriteEx(Difficulty);
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
            return Name;
        }
    }
}
