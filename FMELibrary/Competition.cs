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
        public short Color1 { get; set; }
        public short Color2 { get; set; }
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
        public byte Unknown4 { get; set; }

        #region Extra

        public string Nation { get; set; } = string.Empty;

        #endregion

        public Competition(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();

            FullName = reader.ReadStringEx();
            Unknown1 = reader.ReadByte();
            ShortName = reader.ReadStringEx();
            Unknown2 = reader.ReadByte();
            CodeName = reader.ReadStringEx();

            Type = reader.ReadByte();
            ContinentId = reader.ReadInt16();
            NationId = reader.ReadInt16();
            Color1 = reader.ReadInt16();
            Color2 = reader.ReadInt16();
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
            Unknown4 = reader.ReadByte();
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
            writer.WriteEx(Id);
            writer.WriteEx(Uid);

            writer.WriteEx(FullName);
            writer.WriteEx(Unknown1);
            writer.WriteEx(ShortName);
            writer.WriteEx(Unknown2);
            writer.WriteEx(CodeName);

            writer.WriteEx(Type);
            writer.WriteEx(ContinentId);
            writer.WriteEx(NationId);
            writer.WriteEx(Color1);
            writer.WriteEx(Color2);
            writer.WriteEx(Reputation);
            writer.WriteEx(Level);
            writer.WriteEx(MainComp);

            writer.WriteEx(Qualifiers.Length);
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                writer.WriteEx(Qualifiers[i]);
            }

            writer.WriteEx(Rank1);
            writer.WriteEx(Rank2);
            writer.WriteEx(Rank3);
            writer.WriteEx(Year1);
            writer.WriteEx(Year2);
            writer.WriteEx(Year3);
            writer.WriteEx(Unknown3);
            writer.WriteEx(Unknown4);
        }

        public override string ToString()
        {
            return $"{Id} - {Uid} - {FullName}";
        }
    }
}
