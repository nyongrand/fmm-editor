using System.Drawing;

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

        public class NationalTeam
        {
            /// <summary>
            /// Probably deprecated, all Nations have the value 0xFF7F
            /// </summary>
            public short Color1 { get; set; }

            /// <summary>
            /// Probably deprecated, all Nations have the value 0xD400
            /// </summary>
            public short Color2 { get; set; }

            /// <summary>
            /// Probably deprecated, all Nations have the valuee 0x0000
            /// </summary>
            public short Color3 { get; set; }

            /// <summary>
            /// Probably deprecated, all Nations have the value 0xFF7F
            /// </summary>
            public short Color4 { get; set; }

            /// <summary>
            /// Importance rating used by scheduling/AI: 0 = Not set, 1 = Very important,
            /// 2 = Important, 3 = Fairly unimportant, 4 = Unimportant.
            /// </summary>
            public byte GameImportance { get; set; }

            /// <summary>
            /// Rival nation identifier.
            /// </summary>
            public short RivalId { get; set; }

            /// <summary>
            /// Unknown flag byte.
            /// </summary>
            public byte Unknown4 { get; set; }

            /// <summary>
            /// Whether the team has an official ranking.
            /// </summary>
            public bool IsRanked { get; set; }

            /// <summary>
            /// FIFA/UEFA ranking position.
            /// </summary>
            public short Ranking { get; set; }

            /// <summary>
            /// Ranking points value.
            /// </summary>
            public short Points { get; set; }

            /// <summary>
            /// Always 0x0000 in observed files (reserved padding).
            /// </summary>
            public short Unknown5 { get; set; }

            /// <summary>
            /// Coefficient values for European competition seeding, stored per season
            /// from 2014/15 through 2024/25 (count stored as a single byte in file).
            /// </summary>
            public float[] Coefficients { get; set; } = [];

            /// <summary>
            /// Unknown data block (11 bytes).
            /// </summary>
            public byte[] Unknown6 { get; set; } = [];

            /// <summary>
            /// Reads a national team block from the provided binary reader.
            /// </summary>
            /// <param name="reader">Binary reader positioned at the start of the team block.</param>
            public NationalTeam(BinaryReaderEx reader)
            {
                Color1 = reader.ReadInt16();
                Color2 = reader.ReadInt16();
                Color3 = reader.ReadInt16();
                Color4 = reader.ReadInt16();

                GameImportance = reader.ReadByte();
                RivalId = reader.ReadInt16();
                Unknown4 = reader.ReadByte();

                IsRanked = reader.ReadBoolean();
                Ranking = reader.ReadInt16();
                Points = reader.ReadInt16();

                Unknown5 = reader.ReadInt16();

                Coefficients = new float[reader.ReadByte()];
                for (int i = 0; i < Coefficients.Length; i++)
                    Coefficients[i] = reader.ReadSingle();

                Unknown6 = reader.ReadBytes(11);
            }

            /// <summary>
            /// Writes this national team block to the provided binary writer.
            /// </summary>
            /// <param name="writer">Binary writer positioned where the block should be emitted.</param>
            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Color1);
                writer.Write(Color2);
                writer.Write(Color3);
                writer.Write(Color4);

                writer.Write(GameImportance);
                writer.Write(RivalId);
                writer.Write(Unknown4);

                writer.Write(IsRanked);
                writer.Write(Ranking);
                writer.Write(Points);

                writer.Write(Unknown5);

                writer.Write((byte)Coefficients.Length);
                for (int i = 0; i < Coefficients.Length; i++)
                    writer.Write(Coefficients[i]);

                writer.Write(Unknown6);
            }
        }

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
