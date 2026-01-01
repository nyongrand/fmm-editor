using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMMLibrary
{
    public class Official
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public int Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public short Unknown3 { get; set; }
        public short Unknown4 { get; set; }

        public byte Unknown5 { get; set; }
        public byte Unknown6 { get; set; }
        public byte Unknown7 { get; set; }
        public byte Unknown8 { get; set; }
        public byte Unknown9 { get; set; }

        public int UnknownA { get; set; }
        public int UnknownB { get; set; }
        public int UnknownC { get; set; }
        public int UnknownD { get; set; }
        public int UnknownE { get; set; }
        public int UnknownF { get; set; }
        public int UnknownG { get; set; }
        public int UnknownH { get; set; }
        public int UnknownI { get; set; }
        public int UnknownJ { get; set; }
        public int UnknownK { get; set; }
        public int UnknownL { get; set; }
        public int UnknownM { get; set; }
        public int UnknownN { get; set; }
        public int UnknownO { get; set; }
        public int UnknownP { get; set; }
        public int UnknownQ { get; set; }
        public int UnknownR { get; set; }
        public int UnknownS { get; set; }

        public Official(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();
            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadInt16();
            Unknown4 = reader.ReadInt16();

            Unknown5 = reader.ReadByte();
            Unknown6 = reader.ReadByte();
            Unknown7 = reader.ReadByte();
            Unknown8 = reader.ReadByte();
            Unknown9 = reader.ReadByte();

            UnknownA = reader.ReadInt32();
            UnknownB = reader.ReadInt32();
            UnknownC = reader.ReadInt32();
            UnknownD = reader.ReadInt32();
            UnknownE = reader.ReadInt32();
            UnknownF = reader.ReadInt32();
            UnknownG = reader.ReadInt32();
            UnknownH = reader.ReadInt32();
            UnknownI = reader.ReadInt32();
            UnknownJ = reader.ReadInt32();
            UnknownK = reader.ReadInt32();
            UnknownL = reader.ReadInt32();
            UnknownM = reader.ReadInt32();
            UnknownN = reader.ReadInt32();
            UnknownO = reader.ReadInt32();
            UnknownP = reader.ReadInt32();

            UnknownQ = reader.ReadInt32();
            UnknownR = reader.ReadInt32();
            UnknownS = reader.ReadInt32();
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
