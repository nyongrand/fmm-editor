namespace FMMLibrary
{
    public class NonPlayer
    {
        public int Id { get; set; }
        public short Unknown1 { get; set; }
        public short Unknown2 { get; set; }

        public short UnknownA { get; set; }
        public short UnknownB { get; set; }
        public short UnknownC { get; set; }
        public short UnknownD { get; set; }
        public short UnknownE { get; set; }
        public short UnknownF { get; set; }
        public short UnknownG { get; set; }
        public short UnknownH { get; set; }
        public short UnknownI { get; set; }
        public short UnknownJ { get; set; }
        public short UnknownK { get; set; }
        public short UnknownL { get; set; }
        public short UnknownM { get; set; }
        public short UnknownN { get; set; }
        public short UnknownO { get; set; }

        public byte Unknown3 { get; set; }

        public NonPlayer(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            Unknown1 = reader.ReadInt16();
            Unknown2 = reader.ReadInt16();

            UnknownA = reader.ReadInt16();
            UnknownB = reader.ReadInt16();
            UnknownC = reader.ReadInt16();
            UnknownD = reader.ReadInt16();
            UnknownE = reader.ReadInt16();
            UnknownF = reader.ReadInt16();
            UnknownG = reader.ReadInt16();
            UnknownH = reader.ReadInt16();
            UnknownI = reader.ReadInt16();
            UnknownJ = reader.ReadInt16();
            UnknownK = reader.ReadInt16();
            UnknownL = reader.ReadInt16();
            UnknownM = reader.ReadInt16();
            UnknownN = reader.ReadInt16();
            UnknownO = reader.ReadInt16();

            Unknown3 = reader.ReadByte();
        }

        public void Write(BinaryWriterEx writer)
        {
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
