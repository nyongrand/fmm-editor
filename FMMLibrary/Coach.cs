namespace FMMLibrary
{
    public class Coach
    {
        public int Id { get; set; }
        public sbyte CoachSpeciality { get; set; }
        public sbyte Unknown2 { get; set; }
        public sbyte Unknown3 { get; set; }

        public Coach(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            CoachSpeciality = reader.ReadSByte();
            Unknown2 = reader.ReadSByte();
            Unknown3 = reader.ReadSByte();
        }

        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(CoachSpeciality);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }
    }
}
