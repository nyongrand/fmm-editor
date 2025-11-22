namespace FMELibrary
{
    public class People
    {
        /// <summary>
        /// Always 0xFFFFFFFF
        /// </summary>
        public int Id { get; set; }

        public int Uid { get; set; }

        public int FirstNameId { get; set; }

        public int LastNameId { get; set; }

        public int CommonNameId { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public short NationId { get; set; }

        public short OtherNationalityCount { get; set; }

        public List<short> OtherNationalities { get; set; }

        public byte Ethnicity { get; set; }

        public int Unknown1 { get; set; }

        public byte Type { get; set; }

        public DateOnly UnknownDate { get; set; }

        public short NationalCaps { get; set; }

        public short NationalGoals { get; set; }

        public byte NationalU21Caps { get; set; }

        public byte NationalU21Goals { get; set; }

        public int Unknown2 { get; set; }

        public DateOnly RecentCall { get; set; }

        public short Unknown3 { get; set; }

        public int Unknown4 { get; set; }

        public int Unknown5 { get; set; }

        public int Unknown6a { get; set; }
        public int Unknown6b { get; set; }
        public int Unknown6c { get; set; }
        public int Unknown6d { get; set; }
        public int Unknown6e { get; set; }
        public int? Unknown6f { get; set; }

        public byte Unknown7 { get; set; }
        public byte Unknown8 { get; set; }
        public int? Unknown9 { get; set; }
        public short? Unknown10 { get; set; }

        public byte MainLanguageCount { get; set; }

        public (short Id, byte Proficiency)[] MainLanguages { get; set; }

        public byte OtherLanguageCount { get; set; }

        public (short Id, byte Proficiency)[] OtherLanguages { get; set; }

        public byte RelationsCount { get; set; }

        public long[] Relations { get; set; }

        public byte Unknown21 { get; set; }


        public People(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            FirstNameId = reader.ReadInt32();
            LastNameId = reader.ReadInt32();
            CommonNameId = reader.ReadInt32();
            DateOfBirth = reader.ReadDate();

            NationId = reader.ReadInt16();
            OtherNationalityCount = reader.ReadInt16();
            OtherNationalities = [];
            for (int i = 0; i < OtherNationalityCount; i++)
                OtherNationalities.Add(reader.ReadInt16());

            Ethnicity = reader.ReadByte();
            Unknown1 = reader.ReadInt32();
            Type = reader.ReadByte();
            UnknownDate = reader.ReadDate();

            NationalCaps = reader.ReadInt16();
            NationalGoals = reader.ReadInt16();
            NationalU21Caps = reader.ReadByte();
            NationalU21Goals = reader.ReadByte();

            Unknown2 = reader.ReadInt32();
            RecentCall = reader.ReadDate();

            Unknown3 = reader.ReadInt16();
            Unknown4 = reader.ReadInt32();
            Unknown5 = reader.ReadInt32();

            Unknown6a = reader.ReadInt32();
            Unknown6b = reader.ReadInt32();
            Unknown6c = reader.ReadInt32();
            Unknown6d = reader.ReadInt32();
            Unknown6e = reader.ReadInt32();

            var isFFFF = reader.ReadInt32();
            if (isFFFF == -1)
                Unknown6f = isFFFF;
            else
                reader.BaseStream.Position -= 4;

            Unknown7 = reader.ReadByte();
            Unknown8 = reader.ReadByte();

            var peek2 = reader.ReadInt16();
            reader.BaseStream.Position -= 2;
            if (peek2 <= 0)
            {
                Unknown9 = reader.ReadInt32();
                if (Unknown9 == -1)
                    Unknown10 = reader.ReadInt16();
            }

            MainLanguageCount = reader.ReadByte();
            MainLanguages = new (short Id, byte Proficiency)[MainLanguageCount];
            for (int i = 0; i < MainLanguageCount; i++)
            {
                MainLanguages[i] = (reader.ReadInt16(), reader.ReadByte());
            }

            OtherLanguageCount = reader.ReadByte();
            OtherLanguages = new (short Id, byte Proficiency)[OtherLanguageCount];
            for (int i = 0; i < OtherLanguageCount; i++)
            {
                OtherLanguages[i] = (reader.ReadInt16(), reader.ReadByte());
            }

            RelationsCount = reader.ReadByte();
            Relations = new long[RelationsCount];
            for (int i = 0; i < RelationsCount; i++)
            {
                Relations[i] = reader.ReadInt64();
            }
            Unknown21 = reader.ReadByte();
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

            writer.WriteEx(FirstNameId);
            writer.WriteEx(LastNameId);
            writer.WriteEx(CommonNameId);
            writer.WriteEx(DateOfBirth);

            writer.WriteEx(NationId);
            writer.WriteEx((short)OtherNationalities.Count);
            foreach (var nat in OtherNationalities)
                writer.WriteEx(nat);

            writer.WriteEx(Ethnicity);
            writer.WriteEx(Unknown1);
            writer.WriteEx(Type);
            writer.WriteEx(UnknownDate);

            writer.WriteEx(NationalCaps);
            writer.WriteEx(NationalGoals);
            writer.WriteEx(NationalU21Caps);
            writer.WriteEx(NationalU21Goals);

            writer.WriteEx(Unknown2);
            writer.WriteEx(RecentCall);

            writer.WriteEx(Unknown3);
            writer.WriteEx(Unknown4);
            writer.WriteEx(Unknown5);

            writer.WriteEx(Unknown6a);
            writer.WriteEx(Unknown6b);
            writer.WriteEx(Unknown6c);
            writer.WriteEx(Unknown6d);
            writer.WriteEx(Unknown6e);
            writer.WriteEx(Unknown6f);

            writer.WriteEx(Unknown7);
            writer.WriteEx(Unknown8);
            writer.WriteEx(Unknown9);
            writer.WriteEx(Unknown10);

            writer.WriteEx((byte)MainLanguages.Length);
            foreach (var lang in MainLanguages)
            {
                writer.WriteEx(lang.Id);
                writer.WriteEx(lang.Proficiency);
            }

            writer.WriteEx((byte)OtherLanguages.Length);
            foreach (var lang in OtherLanguages)
            {
                writer.WriteEx(lang.Id);
                writer.WriteEx(lang.Proficiency);
            }

            writer.WriteEx((byte)Relations.Length);
            foreach (var unk in Relations)
                writer.WriteEx(unk);

            writer.WriteEx(Unknown21);
        }
    }
}
