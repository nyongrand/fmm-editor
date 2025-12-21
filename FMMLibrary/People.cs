namespace FMMLibrary
{
    public class People
    {
        /// <summary>
        /// Always 0xFFFFFFFF
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// The first name associated with the entity. see <see cref="Name"/>
        /// </summary>
        public int FirstNameId { get; set; }

        /// <summary>
        /// The second name associated with the entity. see <see cref="Name"/>
        /// </summary>
        public int LastNameId { get; set; }

        /// <summary>
        /// The common name associated with the entity. see <see cref="Name"/>
        /// </summary>
        public int CommonNameId { get; set; }

        public int DateOfBirth { get; set; }

        public short NationId { get; set; }

        public short OtherNationalityCount { get; set; }

        public List<short> OtherNationalities { get; set; }

        /// <summary>
        /// Gets or sets the code representing the individual's ethnicity.
        /// </summary>
        /// <remarks>The value corresponds to a predefined set of ethnicity codes. Refer to the
        /// documentation or enumeration for valid values. see <see cref="FMMLibrary.Ethnicity"/></remarks>
        public byte Ethnicity { get; set; }

        /// <summary>
        /// Looks like City ID but not sure
        /// </summary>
        public int Unknown1 { get; set; }

        public byte Type { get; set; }

        public int UnknownDate { get; set; }

        public short NationalCaps { get; set; }

        public short NationalGoals { get; set; }

        public byte NationalU21Caps { get; set; }

        public byte NationalU21Goals { get; set; }

        public int ClubId { get; set; }

        public int JoinedDate { get; set; }

        /// <summary>
        /// Always 0x0000
        /// </summary>
        public short Unknown3 { get; set; }

        public byte Adaptability { get; set; }
        public byte Ambition { get; set; }
        public byte Controversy { get; set; }
        public byte Loyality { get; set; }
        public byte Pressure { get; set; }
        public byte Professionalism { get; set; }
        public byte Sportmanship { get; set; }
        public byte Temperament { get; set; }

        public int PlayerId { get; set; }
        public int Unknown6b { get; set; }
        public int Unknown6c { get; set; }
        public int Unknown6d { get; set; }
        public int Unknown6e { get; set; }
        public int? Unknown6f { get; set; }

        public byte Unknown7 { get; set; }
        public byte Unknown8 { get; set; }
        public int? Unknown9 { get; set; }
        public short? Unknown10 { get; set; }

        public byte DefaultLanguageCount { get; set; }

        public (short Id, byte Proficiency)[] DefaultLanguages { get; set; }

        public byte OtherLanguageCount { get; set; }

        public (short Id, byte Proficiency)[] OtherLanguages { get; set; }

        public byte RelationshipCount { get; set; }

        public Relationship[] Relationships { get; set; }

        public byte Unknown21 { get; set; }

        /// <summary>
        /// Parameterless constructor for creating new People instances.
        /// </summary>
        public People()
        {
            Id = -1;
            OtherNationalities = [];
            DefaultLanguages = [];
            OtherLanguages = [];
            Relationships = [];
        }

        public People(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            FirstNameId = reader.ReadInt32();
            LastNameId = reader.ReadInt32();
            CommonNameId = reader.ReadInt32();
            DateOfBirth = reader.ReadInt32();

            NationId = reader.ReadInt16();
            OtherNationalityCount = reader.ReadInt16();
            OtherNationalities = [];
            for (int i = 0; i < OtherNationalityCount; i++)
                OtherNationalities.Add(reader.ReadInt16());

            Ethnicity = reader.ReadByte();
            Unknown1 = reader.ReadInt32();
            Type = reader.ReadByte();
            UnknownDate = reader.ReadInt32();

            NationalCaps = reader.ReadInt16();
            NationalGoals = reader.ReadInt16();
            NationalU21Caps = reader.ReadByte();
            NationalU21Goals = reader.ReadByte();

            ClubId = reader.ReadInt32();
            JoinedDate = reader.ReadInt32();

            Unknown3 = reader.ReadInt16();

            Adaptability = reader.ReadByte();
            Ambition = reader.ReadByte();
            Controversy = reader.ReadByte();
            Loyality = reader.ReadByte();
            Pressure = reader.ReadByte();
            Professionalism = reader.ReadByte();
            Sportmanship = reader.ReadByte();
            Temperament = reader.ReadByte();

            PlayerId = reader.ReadInt32();
            Unknown6b = reader.ReadInt32();
            Unknown6c = reader.ReadInt32();
            Unknown6d = reader.ReadInt32();
            Unknown6e = reader.ReadInt32();

            var isFFFFFFFF = reader.ReadInt32();
            if (isFFFFFFFF == -1)
                Unknown6f = isFFFFFFFF;
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

            DefaultLanguageCount = reader.ReadByte();
            DefaultLanguages = new (short Id, byte Proficiency)[DefaultLanguageCount];
            for (int i = 0; i < DefaultLanguageCount; i++)
            {
                DefaultLanguages[i] = (reader.ReadInt16(), reader.ReadByte());
            }

            OtherLanguageCount = reader.ReadByte();
            OtherLanguages = new (short Id, byte Proficiency)[OtherLanguageCount];
            for (int i = 0; i < OtherLanguageCount; i++)
            {
                OtherLanguages[i] = (reader.ReadInt16(), reader.ReadByte());
            }

            RelationshipCount = reader.ReadByte();
            Relationships = new Relationship[RelationshipCount];
            for (int i = 0; i < RelationshipCount; i++)
            {
                Relationships[i] = new Relationship(reader);
            }
            Unknown21 = reader.ReadByte();
        }

        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);

            writer.Write(FirstNameId);
            writer.Write(LastNameId);
            writer.Write(CommonNameId);
            writer.Write(DateOfBirth);

            writer.Write(NationId);
            writer.Write((short)OtherNationalities.Count);
            foreach (var nat in OtherNationalities)
                writer.Write(nat);

            writer.Write(Ethnicity);
            writer.Write(Unknown1);
            writer.Write(Type);
            writer.Write(UnknownDate);

            writer.Write(NationalCaps);
            writer.Write(NationalGoals);
            writer.Write(NationalU21Caps);
            writer.Write(NationalU21Goals);

            writer.Write(ClubId);
            writer.Write(JoinedDate);
            writer.Write(Unknown3);

            writer.Write(Adaptability);
            writer.Write(Ambition);
            writer.Write(Controversy);
            writer.Write(Loyality);
            writer.Write(Pressure);
            writer.Write(Professionalism);
            writer.Write(Sportmanship);
            writer.Write(Temperament);

            writer.Write(PlayerId);
            writer.Write(Unknown6b);
            writer.Write(Unknown6c);
            writer.Write(Unknown6d);
            writer.Write(Unknown6e);
            if (Unknown6f.HasValue)
                writer.Write(Unknown6f.Value);

            writer.Write(Unknown7);
            writer.Write(Unknown8);
            if (Unknown9.HasValue)
                writer.Write(Unknown9.Value);
            if (Unknown10.HasValue)
                writer.Write(Unknown10.Value);

            writer.Write((byte)DefaultLanguages.Length);
            foreach (var lang in DefaultLanguages)
            {
                writer.Write(lang.Id);
                writer.Write(lang.Proficiency);
            }

            writer.Write((byte)OtherLanguages.Length);
            foreach (var lang in OtherLanguages)
            {
                writer.Write(lang.Id);
                writer.Write(lang.Proficiency);
            }

            writer.Write((byte)Relationships.Length);
            foreach (var relationship in Relationships)
                relationship.Write(writer);

            writer.Write(Unknown21);
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
