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

        public int Unknown1 { get; set; }
        public short Unknown2 { get; set; }
        public byte Unknown3 { get; set; }

        public (short, byte)[] Languages { get; set; }

        public byte IsActive { get; set; }
        public short? Color1 { get; set; }
        public int? Unknown5 { get; set; }
        public short? Color2 { get; set; }

        public byte? Unknown6 { get; set; }
        public short? Unknown7 { get; set; }
        public byte? Unknown8 { get; set; }

        public byte? IsRanked { get; set; }
        public short? Ranking { get; set; }
        public short? Points { get; set; }

        public short? Unknown9 { get; set; }
        public float[] Coefficients1 { get; set; } = [];

        //public (string, short, byte)[] ExtraNames { get; set; }
        public byte[] Unknown10 { get; set; } = [];
        public byte HasCoefficient2 { get; set; }
        public byte[] Unknown11 { get; set; } = [];
        public byte? Unknown12 { get; set; }
        public short? Unknown13 { get; set; }
        public float[] Coefficients2 { get; set; } = [];
        public byte[] Unknown14 { get; set; } = [];

        public Nation(BinaryReader reader)
        {
            Uid = reader.ReadInt32();
            Id = reader.ReadInt16();

            Name = reader.ReadStringEx();
            Terminator1 = reader.ReadByte();
            Nationality = reader.ReadStringEx();
            Terminator2 = reader.ReadByte();
            CodeName = reader.ReadStringEx();

            ContinentId = reader.ReadInt16();
            CityId = reader.ReadInt16();
            StadiumId = reader.ReadInt16();

            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadByte();

            Languages = new (short, byte)[reader.ReadByte()];
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = (reader.ReadInt16(), reader.ReadByte());

            IsActive = reader.ReadByte();
            if (IsActive == 1)
            {
                Color1 = reader.ReadInt16();
                Unknown5 = reader.ReadInt32();
                Color2 = reader.ReadInt16();

                Unknown6 = reader.ReadByte();
                Unknown7 = reader.ReadInt16();
                Unknown8 = reader.ReadByte();

                IsRanked = reader.ReadByte();
                Ranking = reader.ReadInt16();
                Points = reader.ReadInt16();

                Unknown9 = reader.ReadInt16();

                Coefficients1 = new float[reader.ReadByte()];
                for (int i = 0; i < Coefficients1.Length; i++)
                    Coefficients1[i] = reader.ReadSingle();

                //ExtraNames = new (string, short, byte)[reader.ReadByte()];
                //for (int i = 0; i < ExtraNames.Length; i++)
                //    ExtraNames[i] = (reader.ReadString(reader.ReadInt32()), reader.ReadInt16(), reader.ReadByte());

                Unknown10 = reader.ReadBytes(11);
            }

            HasCoefficient2 = reader.ReadByte();
            if (HasCoefficient2 == 1)
            {
                Unknown11 = reader.ReadBytes(16);
                Unknown12 = reader.ReadByte();
                Unknown13 = reader.ReadInt16();

                Coefficients2 = new float[reader.ReadByte()];
                for (int i = 0; i < Coefficients2.Length; i++)
                    Coefficients2[i] = reader.ReadSingle();

                Unknown14 = reader.ReadBytes(11);
            }
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
            writer.WriteEx(Uid);
            writer.WriteEx(Id);

            writer.WriteEx(Name);
            writer.WriteEx(Terminator1);
            writer.WriteEx(Nationality);
            writer.WriteEx(Terminator2);
            writer.WriteEx(CodeName);

            writer.WriteEx(ContinentId);
            writer.WriteEx(CityId);
            writer.WriteEx(StadiumId);

            writer.WriteEx(Unknown1);
            writer.WriteEx(Unknown2);
            writer.WriteEx(Unknown3);

            writer.WriteEx((byte)Languages.Length);
            for (int i = 0; i < Languages.Length; i++)
            {
                writer.WriteEx(Languages[i].Item1);
                writer.WriteEx(Languages[i].Item2);
            }

            writer.WriteEx(IsActive);

            if (IsActive == 1)
            {
                writer.WriteEx(Color1);
                writer.WriteEx(Unknown5);
                writer.WriteEx(Color2);

                writer.WriteEx(Unknown6);
                writer.WriteEx(Unknown7);
                writer.WriteEx(Unknown8);

                writer.WriteEx(IsRanked);
                writer.WriteEx(Ranking);
                writer.WriteEx(Points);

                writer.WriteEx(Unknown9);
                writer.WriteEx((byte)Coefficients1.Length);
                for (int i = 0; i < Coefficients1.Length; i++)
                {
                    writer.WriteEx(Coefficients1[i]);
                }

                writer.WriteEx(Unknown10);
            }

            writer.WriteEx(HasCoefficient2);
            if (HasCoefficient2 == 1)
            {
                writer.WriteEx(Unknown11);
                writer.WriteEx(Unknown12);
                writer.WriteEx(Unknown13);

                writer.WriteEx((byte)Coefficients2.Length);
                for (int i = 0; i < Coefficients2.Length; i++)
                {
                    writer.WriteEx(Coefficients2[i]);
                }

                writer.WriteEx(Unknown14);
            }

            //writer.Write((byte)ExtraNames.Length);
            //for (int i = 0; i < ExtraNames.Length; i++)
            //{
            //    writer.WriteEx(ExtraNames[i].Item1);
            //    writer.Write(ExtraNames[i].Item2);
            //    writer.Write(ExtraNames[i].Item3);
            //}
        }
    }
}
