namespace FMMLibrary
{
    public class City
    {
        public short Id { get; set; }
        public int Uid { get; set; }
        public short NationId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public byte Attraction { get; set; }
        public short RegionId { get; set; }
        public byte Unknown { get; set; }

        public City(BinaryReaderEx reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();
            NationId = reader.ReadInt16();
            Latitude = reader.ReadSingle();
            Longitude = reader.ReadSingle();
            Attraction = reader.ReadByte();
            RegionId = reader.ReadInt16();
            Unknown = reader.ReadByte();
        }

        /// <summary>
        /// Writes the nation data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the nation data to.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);
            writer.Write(NationId);
            writer.Write(Latitude);
            writer.Write(Longitude);
            writer.Write(Attraction);
            writer.Write(RegionId);
            writer.Write(Unknown);
        }

        /// <summary>
        /// Converts the nation data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized nation data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        public override string ToString()
        {
            return $"{Id} {Uid}";
        }
    }
}
