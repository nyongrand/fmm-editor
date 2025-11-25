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
        public byte Unknown1 { get; set; }

        public City(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();
            NationId = reader.ReadInt16();
            Latitude = reader.ReadSingle();
            Longitude = reader.ReadSingle();
            Attraction = reader.ReadByte();
            RegionId = reader.ReadInt16();
            Unknown1 = reader.ReadByte();
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
            writer.WriteEx(Latitude);
            writer.WriteEx(Longitude);
            writer.WriteEx(Attraction);
            writer.WriteEx(RegionId);
            writer.WriteEx(Unknown1);
        }

        public override string ToString()
        {
            return $"{Id} {Uid}";
        }
    }
}
