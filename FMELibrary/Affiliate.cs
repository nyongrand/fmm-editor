namespace FMELibrary
{
    public class Affiliate
    {
        public int Unknown1 { get; set; }
        public int Club1Id { get; set; }
        public int Club2Id { get; set; }
        public short StartDay { get; set; }
        public short StartYear { get; set; }
        public short EndDay { get; set; }
        public short EndYear { get; set; }
        public byte Unknown2 { get; set; }

        public Affiliate(BinaryReader reader)
        {
            Unknown1 = reader.ReadInt32();
            Club1Id = reader.ReadInt32();
            Club2Id = reader.ReadInt32();
            StartDay = reader.ReadInt16();
            StartYear = reader.ReadInt16();
            EndDay = reader.ReadInt16();
            EndYear = reader.ReadInt16();
            Unknown2 = reader.ReadByte();
        }
    }
}
