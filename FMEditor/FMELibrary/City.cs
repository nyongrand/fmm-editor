namespace FMELibrary
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
            writer.WriteBytes(Uid);
            writer.WriteBytes(NationId);
            writer.WriteBytes(Latitude);
            writer.WriteBytes(Longitude);
            writer.WriteBytes(Attraction);
            writer.WriteBytes(RegionId);
            writer.WriteBytes(Unknown1);
        }

        public override string ToString()
        {
            return $"{Id} {Uid}";
        }
    }
}