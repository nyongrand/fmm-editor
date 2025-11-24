namespace FMMLibrary
{
    public class City
    {
        public short Id { get; set; }

        public int Uid { get; set; }

        public short NationId { get; set; }

        public byte[] Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public short Unknown4 { get; set; }
        public byte Unknown5 { get; set; }
        public byte Unknown6 { get; set; }

        public City(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();

            NationId = reader.ReadInt16();
            Unknown2 = reader.ReadBytes(7);
            Unknown3 = reader.ReadByte();
            Unknown4 = reader.ReadInt16();
            Unknown5 = reader.ReadByte();
            Unknown6 = reader.ReadByte();
        }

        /// <summary>
        /// Converts the nation data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized nation data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the nation data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the nation data to.</param>
        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Id);
            writer.WriteEx(Uid);
            writer.WriteEx(NationId);
            writer.WriteEx(Unknown2);
            writer.WriteEx(Unknown3);
            writer.WriteEx(Unknown4);
            writer.WriteEx(Unknown5);
            writer.WriteEx(Unknown6);
        }
    }
}
