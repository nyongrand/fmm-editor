using System.Drawing;

namespace FMELibrary
{
    public class Competition
    {
        public short Id { get; set; }
        public int Uid { get; set; }
        public string FullName { get; set; }
        public byte Unknown1 { get; set; }
        public string ShortName { get; set; }
        public byte Unknown2 { get; set; }
        public string CodeName { get; set; }
        public byte Type { get; set; }
        public short ContinentId { get; set; }
        public short NationId { get; set; }
        public Color Color1 { get; set; }
        public Color Color2 { get; set; }
        public short Reputation { get; set; }
        public byte Level { get; set; }
        public short MainComp { get; set; }
        public byte[][] Qualifiers { get; set; }
        public int Rank1 { get; set; }
        public int Rank2 { get; set; }
        public int Rank3 { get; set; }
        public short Year1 { get; set; }
        public short Year2 { get; set; }
        public short Year3 { get; set; }
        public byte Unknown3 { get; set; }

        #region Extra

        public string Nation { get; set; } = string.Empty;

        #endregion

        public Competition(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();

            FullName = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadByte();
            ShortName = reader.ReadString(reader.ReadInt32());
            Unknown2 = reader.ReadByte();
            CodeName = reader.ReadString(reader.ReadInt32());

            Type = reader.ReadByte();
            ContinentId = reader.ReadInt16();
            NationId = reader.ReadInt16();
            Color1 = reader.ReadColor();
            Color2 = reader.ReadColor();
            Reputation = reader.ReadInt16();
            Level = reader.ReadByte();
            MainComp = reader.ReadInt16();

            Qualifiers = new byte[reader.ReadInt32()][];
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                Qualifiers[i] = reader.ReadBytes(8);
            }

            Rank1 = reader.ReadInt32();
            Rank2 = reader.ReadInt32();
            Rank3 = reader.ReadInt32();
            Year1 = reader.ReadInt16();
            Year2 = reader.ReadInt16();
            Year3 = reader.ReadInt16();

            Unknown3 = reader.ReadByte();
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
            writer.Write(Id);
            writer.Write(Uid);

            writer.WriteEx(FullName);
            writer.Write(Unknown1);
            writer.WriteEx(ShortName);
            writer.Write(Unknown2);
            writer.WriteEx(CodeName);

            writer.Write(Type);
            writer.Write(ContinentId);
            writer.Write(NationId);
            writer.WriteEx(Color1);
            writer.WriteEx(Color2);
            writer.Write(Reputation);
            writer.Write(Level);
            writer.Write(MainComp);

            writer.Write(Qualifiers.Length);
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                writer.Write(Qualifiers[i]);
            }

            writer.Write(Rank1);
            writer.Write(Rank2);
            writer.Write(Rank3);
            writer.Write(Year1);
            writer.Write(Year2);
            writer.Write(Year3);
            writer.Write(Unknown3);
        }

        public override string ToString()
        {
            return $"{Id} - {Uid} - {FullName}";
        }
    }
}
