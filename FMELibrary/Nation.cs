namespace FMELibrary
{
    public class Nation
    {
        public int Uid { get; set; }
        public short Id { get; set; }

        public string Name { get; set; }
        public byte Terminator1 { get; set; }
        public string Nationality { get; set; }
        public byte Terminator2 { get; set; }
        public string CodeName { get; set; }

        public short ContinentId { get; set; }
        public short CityId { get; set; }
        public short StadiumId { get; set; }

        public byte[] Unknown1 { get; set; }

        public (short, byte)[] Languages { get; set; }

        public byte Unknown2 { get; set; }
        public short Color1 { get; set; }
        public int Unknown3 { get; set; }
        public short Color2 { get; set; }

        public byte Unknown4 { get; set; }
        public short Unknown5 { get; set; }
        public byte Unknown6 { get; set; }

        public byte IsRanked { get; set; }
        public short Ranking { get; set; }
        public short Points { get; set; }

        public short Unknown7 { get; set; }
        public float[] Coefficients { get; set; }

        //public (string, short, byte)[] ExtraNames { get; set; }
        public byte[] Unknown12 { get; set; }

        public Nation(BinaryReader reader)
        {
            Uid = reader.ReadInt32();
            Id = reader.ReadInt16();

            Name = reader.ReadString(reader.ReadInt32());
            Terminator1 = reader.ReadByte();
            Nationality = reader.ReadString(reader.ReadInt32());
            Terminator2 = reader.ReadByte();
            CodeName = reader.ReadString(reader.ReadInt32());

            ContinentId = reader.ReadInt16();
            CityId = reader.ReadInt16();
            StadiumId = reader.ReadInt16();

            Unknown1 = reader.ReadBytes(7);

            Languages = new (short, byte)[reader.ReadByte()];
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = (reader.ReadInt16(), reader.ReadByte());

            Unknown2 = reader.ReadByte();
            Color1 = reader.ReadInt16();
            Unknown3 = reader.ReadInt32();
            Color2 = reader.ReadInt16();

            Unknown4 = reader.ReadByte();
            Unknown5 = reader.ReadInt16();
            Unknown6 = reader.ReadByte();

            IsRanked = reader.ReadByte();
            Ranking = reader.ReadInt16();
            Points = reader.ReadInt16();

            Unknown7 = reader.ReadInt16();

            Coefficients = new float[reader.ReadByte()];
            for (int i = 0; i < Coefficients.Length; i++)
                Coefficients[i] = reader.ReadSingle();

            //ExtraNames = new (string, short, byte)[reader.ReadByte()];
            //for (int i = 0; i < ExtraNames.Length; i++)
            //    ExtraNames[i] = (reader.ReadString(reader.ReadInt32()), reader.ReadInt16(), reader.ReadByte());

            Unknown12 = reader.ReadBytes(43);
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
            //writer.Write(Uid);
            //writer.Write(Id);
            //writer.WriteEx(Name);
            //writer.Write(Terminator1);
            //writer.WriteEx(Nationality);
            //writer.Write(Terminator2);
            //writer.WriteEx(CodeName);
            //writer.Write(ContinentId);
            //writer.Write(CityId);
            //writer.Write(StadiumId);
            //writer.Write(Unknown1);
            //writer.Write(Color1);
            //writer.Write(Unknown3);
            //writer.Write(Color2);
            //writer.Write(Unknown5);
            //writer.Write(Unknown6);
            //writer.Write(Unknown7);
            //writer.Write(Unknown7);
            //writer.Write(Unknown9);
            //writer.Write(Unknown10);
            //writer.Write(IsRanked);
            //writer.Write(Ranking);
            //writer.Write(Points);
            //writer.Write(Unknown11);

            //writer.Write((byte)Coefficients.Length);
            //for (int i = 0; i < Coefficients.Length; i++)
            //{
            //    writer.Write(Coefficients[i]);
            //}

            //writer.Write((byte)ExtraNames.Length);
            //for (int i = 0; i < ExtraNames.Length; i++)
            //{
            //    writer.WriteEx(ExtraNames[i].Item1);
            //    writer.Write(ExtraNames[i].Item2);
            //    writer.Write(ExtraNames[i].Item3);
            //}

            //writer.Write((byte)Languages.Length);
            //for (int i = 0; i < Languages.Length; i++)
            //{
            //    writer.Write(Languages[i].Item1);
            //    writer.Write(Languages[i].Item2);
            //}

            writer.Write(Unknown12);
        }
    }
}
