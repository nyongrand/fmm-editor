namespace FMELibrary
{
    public class People
    {
        public byte Unknown1 { get; set; }

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

        public int Unknown3 { get; set; }

        public byte Type { get; set; }

        public int UnknownDate { get; set; }

        public short NationalCaps { get; set; }

        public short NationalGoal { get; set; }

        public byte NationalU21Caps { get; set; }
        public byte NationalU21Goals { get; set; }

        public int? Unknown10 { get; set; }

        public int RecentCall { get; set; }

        public short Unknown12 { get; set; }

        public int Unknown13 { get; set; }

        public int Unknown14 { get; set; }

        public int Unknown15 { get; set; }

        public int Unknown15a { get; set; }
        public int Unknown15b { get; set; }
        public int Unknown15c { get; set; }
        public int Unknown15d { get; set; }
        public int Unknown15e { get; set; }

        public byte Unknown16 { get; set; }
        public byte Unknown17 { get; set; }

        public int? Unknown18 { get; set; }
        public short? Unknown19 { get; set; }

        public byte MainLanguageCount { get; set; }

        public (short Id, byte Proficiency)[] MainLanguages { get; set; }

        public byte OtherLanguageCount { get; set; }

        public (short Id, byte Proficiency)[] OtherLanguages { get; set; }

        public byte Unknown20Count { get; set; }

        public long[] Unknown20 { get; set; }


        private string? _firstName;
        private string? _lastName;

        private static readonly int[] _specialIds = [
            103409, 103607, 124521, 142512, 155126, 155129, 350411, 352719, 353194, 353514, 353954, 356951, 357788, 357873, 359538,
            410436, 413732, 414872, 416746, 537851, 561318, 675472, 714504, 719130, 830987, 864223, 929769, 947423, 947676, 956666,
        ];

        public People(BinaryReader reader, List<Name> firstNames, List<Name> lastNames)
        {
            Unknown1 = reader.ReadByte();
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            FirstNameId = reader.ReadInt32();
            LastNameId = reader.ReadInt32();
            CommonNameId = reader.ReadInt32();

            _firstName = firstNames.Where(x => x.Id == FirstNameId).FirstOrDefault()?.Value;
            _lastName = lastNames.Where(x => x.Id == LastNameId).FirstOrDefault()?.Value;
            Console.WriteLine($"{Uid} - {Convert.ToHexString(BitConverter.GetBytes(Uid))}: {_firstName} {_lastName}");

            // read 2 bytes day of year and 2 bytes year
            DateOfBirth = reader.ReadDate();

            NationId = reader.ReadInt16();
            OtherNationalityCount = reader.ReadInt16();
            OtherNationalities = [];
            for (int i = 0; i < OtherNationalityCount; i++)
                OtherNationalities.Add(reader.ReadInt16());

            Ethnicity = reader.ReadByte();
            Unknown3 = reader.ReadInt32();
            Type = reader.ReadByte();
            UnknownDate = reader.ReadInt32();

            NationalCaps = reader.ReadInt16();
            NationalGoal = reader.ReadInt16();
            NationalU21Caps = reader.ReadByte();
            NationalU21Goals = reader.ReadByte();

            Unknown10 = reader.ReadInt32();
            RecentCall = reader.ReadInt32();

            Unknown12 = reader.ReadInt16();
            Unknown13 = reader.ReadInt32();
            Unknown14 = reader.ReadInt32();

            if (Type == 1 && !_specialIds.Contains(Uid))
                Unknown15 = reader.ReadInt32();

            Unknown15a = reader.ReadInt32();
            Unknown15b = reader.ReadInt32();
            Unknown15c = reader.ReadInt32();
            Unknown15d = reader.ReadInt32();
            Unknown15e = reader.ReadInt32();

            Unknown16 = reader.ReadByte();
            Unknown17 = reader.ReadByte();
            if (Unknown17 != 0) // Only exists for Type != 1
            {
                Unknown18 = reader.ReadInt32();
                Unknown19 = reader.ReadInt16();
            }
            else
            {
                if (Type != 1)
                    Unknown18 = reader.ReadInt32();
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

            Unknown20Count = reader.ReadByte();
            Unknown20 = new long[Unknown20Count];
            for (int i = 0; i < Unknown20Count; i++)
            {
                Unknown20[i] = reader.ReadInt64();
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
            writer.WriteEx(Unknown1);
            writer.WriteEx(Id);
            writer.WriteEx(Uid);

            writer.WriteEx(FirstNameId);
            writer.WriteEx(LastNameId);
            writer.WriteEx(CommonNameId);

            writer.WriteEx(DateOfBirth);
            writer.WriteEx(NationId);
            writer.WriteEx(OtherNationalityCount);
            foreach (var nat in OtherNationalities)
                writer.WriteEx(nat);

            writer.WriteEx(Ethnicity);
            writer.WriteEx(Unknown3);
            writer.WriteEx(Type);
            writer.WriteEx(UnknownDate);
            writer.WriteEx(NationalCaps);
            writer.WriteEx(NationalGoal);
            writer.WriteEx(NationalU21Caps);
            writer.WriteEx(NationalU21Goals);
            writer.WriteEx(Unknown10);
            writer.WriteEx(RecentCall);
            writer.WriteEx(Unknown12);
            writer.WriteEx(Unknown13);
            writer.WriteEx(Unknown14);
            if (Type == 1 && !_specialIds.Contains(Uid))
                writer.WriteEx(Unknown15);
            writer.WriteEx(Unknown15a);
            writer.WriteEx(Unknown15b);
            writer.WriteEx(Unknown15c);
            writer.WriteEx(Unknown15d);
            writer.WriteEx(Unknown15e);
            writer.WriteEx(Unknown16);
            writer.WriteEx(Unknown17);
            if (Unknown17 != 0) // Only exists for Type != 1
            {
                writer.WriteEx(Unknown18);
                writer.WriteEx(Unknown19);
            }
            else
            {
                if (Type != 1)
                    writer.WriteEx(Unknown18);
            }
            writer.WriteEx(MainLanguageCount);
            foreach (var lang in MainLanguages)
            {
                writer.WriteEx(lang.Id);
                writer.WriteEx(lang.Proficiency);
            }
            writer.WriteEx(OtherLanguageCount);
            foreach (var lang in OtherLanguages)
            {
                writer.WriteEx(lang.Id);
                writer.WriteEx(lang.Proficiency);
            }
            writer.WriteEx(Unknown20Count);
            foreach (var unk in Unknown20)
                writer.WriteEx(unk);
        }
    }
}
