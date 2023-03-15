namespace FMELibrary
{
    public class Nation
    {
        public int Uid { get; set; }
        public short Id { get; set; }
        public string Name { get; set; }
        public byte Unknown1 { get; set; }
        public string Nationality { get; set; }
        public byte Unknown2 { get; set; }
        public string CodeName { get; set; }
        public short Continent { get; set; }
        public short City { get; set; }
        public short Stadium { get; set; }
        public byte Unknown3 { get; set; }
        public short Color1 { get; set; }
        public int Unknown4 { get; set; }
        public short Color2 { get; set; }
        public byte Unknown5 { get; set; }
        public byte Unknown6 { get; set; }
        public short Unknown7 { get; set; }
        public short Unknown8 { get; set; }
        public byte Unknown9 { get; set; }
        public short Unknown10 { get; set; }
        public bool IsRanked { get; set; }
        public short Ranking { get; set; }
        public short Points { get; set; }
        public int Unknown11 { get; set; }
        public float[] Coefficients { get; set; }
        public ExtraName[] ExtraNames { get; set; }
        public Language[] Languages { get; set; }
        public byte[] Unknown12 { get; set; }

        public Nation(BinaryReader reader)
        {
            Uid = reader.ReadInt32();
            Id = reader.ReadInt16();
            Name = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadByte();
            Nationality = reader.ReadString(reader.ReadInt32());
            Unknown2 = reader.ReadByte();
            CodeName = reader.ReadString(reader.ReadInt32());
            Continent = reader.ReadInt16();
            City = reader.ReadInt16();
            Stadium = reader.ReadInt16();
            Unknown3 = reader.ReadByte();
            Color1 = reader.ReadInt16();
            Unknown4 = reader.ReadInt32();
            Color2 = reader.ReadInt16();
            Unknown5 = reader.ReadByte();
            Unknown6 = reader.ReadByte();
            Unknown7 = reader.ReadInt16();
            Unknown8 = reader.ReadInt16();
            Unknown9 = reader.ReadByte();
            Unknown10 = reader.ReadInt16();
            IsRanked = reader.ReadBoolean();
            Ranking = reader.ReadInt16();
            Points = reader.ReadInt16();
            Unknown11 = reader.ReadInt32();

            Coefficients = new float[reader.ReadByte()];
            for (int i = 0; i < Coefficients.Length; i++)
                Coefficients[i] = reader.ReadSingle();

            ExtraNames = new ExtraName[reader.ReadByte()];
            for (int i = 0; i < ExtraNames.Length; i++)
                ExtraNames[i] = new ExtraName(reader);

            Languages = new Language[reader.ReadByte()];
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = new Language(reader);

            Unknown12 = reader.ReadBytes(11);
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteBytes(Uid);
            writer.WriteBytes(Id);
            writer.WriteBytes(Name);
            writer.WriteBytes(Unknown1);
            writer.WriteBytes(Nationality);
            writer.WriteBytes(Unknown2);
            writer.WriteBytes(CodeName);
            writer.WriteBytes(Continent);
            writer.WriteBytes(City);
            writer.WriteBytes(Stadium);
            writer.WriteBytes(Unknown3);
            writer.WriteBytes(Color1);
            writer.WriteBytes(Unknown4);
            writer.WriteBytes(Color2);
            writer.WriteBytes(Unknown5);
            writer.WriteBytes(Unknown6);
            writer.WriteBytes(Unknown7);
            writer.WriteBytes(Unknown8);
            writer.WriteBytes(Unknown9);
            writer.WriteBytes(Unknown10);
            writer.WriteBytes(IsRanked);
            writer.WriteBytes(Ranking);
            writer.WriteBytes(Points);
            writer.WriteBytes(Unknown11);

            writer.WriteBytes((byte)Coefficients.Length);
            for (int i = 0; i < Coefficients.Length; i++)
            {
                writer.WriteBytes(Coefficients[i]);
            }

            writer.WriteBytes((byte)ExtraNames.Length);
            for (int i = 0; i < ExtraNames.Length; i++)
            {
                writer.WriteBytes(ExtraNames[i].Item1);
                writer.WriteBytes(ExtraNames[i].Item2);
                writer.WriteBytes(ExtraNames[i].Item3);
            }

            writer.WriteBytes((byte)Languages.Length);
            for (int i = 0; i < Languages.Length; i++)
            {
                writer.WriteBytes(Languages[i].Item1);
                writer.WriteBytes(Languages[i].Item2);
            }

            writer.WriteBytes(Unknown12);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        public override string ToString()
        {
            return $"{Id} {Uid} {Name}";
        }
    }
}
