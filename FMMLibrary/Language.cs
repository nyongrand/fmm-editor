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

        public Language(BinaryReaderEx reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();
            Name = reader.ReadString();
            OtherName = reader.ReadString();
            NationId = reader.ReadInt16();
            Difficulty = reader.ReadByte();
        }

        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);
            writer.Write(Name);
            writer.Write(OtherName);
            writer.Write(NationId);
            writer.Write(Difficulty);
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
            return Name;
        }
    }
}
