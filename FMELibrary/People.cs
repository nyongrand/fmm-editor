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

        public short Unknown5 { get; set; }

        public short Unknown6 { get; set; }

        public short Unknown7 { get; set; }

        public short Unknown8 { get; set; }

        public short Unknown9 { get; set; }

        public int Unknown10 { get; set; }

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

        public List<(int, int)> Unknown20 { get; set; }


        private string? _firstName;
        private string? _lastName;

        public People(BinaryReader reader, List<Name> firstNames, List<Name> lastNames)
        {
            Unknown1 = reader.ReadByte();
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            if (Uid == 15048541)
            {
                // Debug breakpoint placeholder
            }

            FirstNameId = reader.ReadInt32();
            LastNameId = reader.ReadInt32();
            CommonNameId = reader.ReadInt32();

            _firstName = firstNames.Where(x => x.Id == FirstNameId).FirstOrDefault()?.Value;
            _lastName = lastNames.Where(x => x.Id == LastNameId).FirstOrDefault()?.Value;
            Console.WriteLine($"{Uid} - {Uid:X8}: {_firstName} {_lastName}");

            DateOfBirth = reader.ReadDate();

            NationId = reader.ReadInt16();
            OtherNationalityCount = reader.ReadInt16();
            OtherNationalities = [];
            for (int i = 0; i < OtherNationalityCount; i++)
                OtherNationalities.Add(reader.ReadInt16());

            Ethnicity = reader.ReadByte();
            Unknown3 = reader.ReadInt32();
            Type = reader.ReadByte();
            Unknown5 = reader.ReadInt16();
            Unknown6 = reader.ReadInt16();
            Unknown7 = reader.ReadInt16();
            Unknown8 = reader.ReadInt16();
            Unknown9 = reader.ReadInt16();
            if (Type == 1)
                Unknown10 = reader.ReadInt32();

            RecentCall = reader.ReadInt32();
            Unknown12 = reader.ReadInt16();
            Unknown13 = reader.ReadInt32();
            Unknown14 = reader.ReadInt32();
            Unknown15 = reader.ReadInt32();

            Unknown15a = reader.ReadInt32();
            Unknown15b = reader.ReadInt32();
            Unknown15c = reader.ReadInt32();
            Unknown15d = reader.ReadInt32();
            Unknown15e = reader.ReadInt32();

            Unknown16 = reader.ReadByte();
            Unknown17 = reader.ReadByte();
            if (Type != 1) // Only exists for Type != 1
            {
                Unknown18 = reader.ReadInt32();
                if (Unknown17 != 0)
                    Unknown19 = reader.ReadInt16();
            }

            MainLanguageCount = reader.ReadByte();
            MainLanguages = new (short, byte)[MainLanguageCount];
            for (int i = 0; i < MainLanguageCount; i++)
            {
                MainLanguages[i] = (reader.ReadInt16(), reader.ReadByte());
            }

            OtherLanguageCount = reader.ReadByte();
            OtherLanguages = new (short, byte)[OtherLanguageCount];
            for (int i = 0; i < OtherLanguageCount; i++)
            {
                OtherLanguages[i] = (reader.ReadInt16(), reader.ReadByte());
            }

            Unknown20Count = reader.ReadByte();
            Unknown20 = new List<(int, int)>(Unknown20Count);
            for (int i = 0; i < Unknown20Count; i++)
            {
                Unknown20.Add((reader.ReadInt32(), reader.ReadInt32()));
            }

            if (Uid == 15048541)
            {
                // Debug breakpoint placeholder
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Unknown1);

        }
    }
}
