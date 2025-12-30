namespace FMMLibrary
{
    /// <summary>
    /// Represents a nation entry in an FMM database (.dat) with identification data,
    /// textual metadata, language list, and optional male/female national team blocks.
    /// </summary>
    public class Nation
    {
        /// <summary>
        /// Unique database identifier for the nation (int32 in the file).
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Short identifier used by references elsewhere in the database (int16).
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Display name of the nation (zero-terminated string in the file).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Single terminator byte that follows <see cref="Name"/> in the binary data.
        /// </summary>
        public byte Terminator1 { get; set; }

        /// <summary>
        /// Demonym / nationality adjective for the nation (zero-terminated string).
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Single terminator byte that follows <see cref="Nationality"/> in the binary data.
        /// </summary>
        public byte Terminator2 { get; set; }

        /// <summary>
        /// Internal short code name for the nation (zero-terminated string).
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// Identifier of the continent the nation belongs to.
        /// </summary>
        public short ContinentId { get; set; }

        /// <summary>
        /// Identifier of the nation's capital city.
        /// </summary>
        public short CapitalId { get; set; }

        /// <summary>
        /// Identifier of the nation's primary stadium.
        /// </summary>
        public short StadiumId { get; set; }

        /// <summary>
        /// Development state flag (1 = Developed, 2 = Developing, 3 = Third World).
        /// </summary>
        public byte StateOfDevelopment { get; set; }

        /// <summary>
        /// Unknown flag byte; may indicate EU membership or similar region flag.
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// Always 0x0000 in observed files (reserved padding).
        /// </summary>
        public short Unknown2 { get; set; }

        /// <summary>
        /// Region identifier used for grouping nations.
        /// </summary>
        public short Region { get; set; }

        /// <summary>
        /// Always 0x00 in observed files (reserved padding).
        /// </summary>
        public byte Unknown3 { get; set; }

        /// <summary>
        /// Languages spoken in the nation as pairs of language ID and proficiency level.
        /// The count is stored as a single byte in the file.
        /// </summary>
        public (short Id, byte Proficiency)[] Languages { get; set; }

        /// <summary>
        /// Indicates whether a male national team block follows in the file.
        /// </summary>
        public bool HasMaleTeam { get; set; }

        public NationalTeam? MaleTeam { get; set; }

        /// <summary>
        /// Indicates whether a female national team block follows in the file.
        /// </summary>
        public bool HasFemaleTeam { get; set; }

        public NationalTeam? FemaleTeam { get; set; }

        /// <summary>
        /// Creates a nation by reading its binary representation from the provided reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the nation record.</param>
        public Nation(BinaryReaderEx reader)
        {
            Uid = reader.ReadInt32();
            Id = reader.ReadInt16();

            Name = reader.ReadString();
            Terminator1 = reader.ReadByte();
            Nationality = reader.ReadString();
            Terminator2 = reader.ReadByte();
            CodeName = reader.ReadString();

            ContinentId = reader.ReadInt16();
            CapitalId = reader.ReadInt16();
            StadiumId = reader.ReadInt16();

            StateOfDevelopment = reader.ReadByte();
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadInt16();
            Region = reader.ReadInt16();
            Unknown3 = reader.ReadByte();

            Languages = new (short Id, byte Proficiency)[reader.ReadByte()];
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = (reader.ReadInt16(), reader.ReadByte());

            HasMaleTeam = reader.ReadBoolean();
            MaleTeam = HasMaleTeam ? new NationalTeam(reader) : null;

            HasFemaleTeam = reader.ReadBoolean();
            FemaleTeam = HasFemaleTeam ? new NationalTeam(reader) : null;
        }

        /// <summary>
        /// Serializes the nation to a byte array using the game file format.
        /// </summary>
        /// <returns>Byte array containing the serialized nation.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes this nation to a binary writer using the game file layout.
        /// </summary>
        /// <param name="writer">Binary writer to receive the nation data.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Uid);
            writer.Write(Id);

            writer.Write(Name);
            writer.Write(Terminator1);
            writer.Write(Nationality);
            writer.Write(Terminator2);
            writer.Write(CodeName);

            writer.Write(ContinentId);
            writer.Write(CapitalId);
            writer.Write(StadiumId);

            writer.Write(StateOfDevelopment);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Region);
            writer.Write(Unknown3);

            writer.Write((byte)Languages.Length);
            for (int i = 0; i < Languages.Length; i++)
            {
                writer.Write(Languages[i].Id);
                writer.Write(Languages[i].Proficiency);
            }

            writer.Write(HasMaleTeam);
            if (HasMaleTeam && MaleTeam != null)
                MaleTeam.Write(writer);

            writer.Write(HasFemaleTeam);
            if (HasFemaleTeam && FemaleTeam != null)
                FemaleTeam.Write(writer);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
