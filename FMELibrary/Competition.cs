namespace FMELibrary
{
    /// <summary>
    /// Represents a football competition (league, cup, etc.) with all its properties.
    /// </summary>
    public class Competition
    {
        /// <summary>
        /// Gets or sets the competition identifier.
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the competition.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Gets or sets the full name of the competition.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// Gets or sets the short name of the competition.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the code name of the competition.
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// Gets or sets the type of competition.
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// Gets or sets the continent identifier.
        /// </summary>
        public short ContinentId { get; set; }

        /// <summary>
        /// Gets or sets the nation identifier.
        /// </summary>
        public short NationId { get; set; }

        /// <summary>
        /// Gets or sets the first color value.
        /// </summary>
        public short Color1 { get; set; }

        /// <summary>
        /// Gets or sets the second color value.
        /// </summary>
        public short Color2 { get; set; }

        /// <summary>
        /// Gets or sets the competition's reputation.
        /// </summary>
        public short Reputation { get; set; }

        /// <summary>
        /// Gets or sets the competition level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Gets or sets the main competition identifier.
        /// </summary>
        public short MainComp { get; set; }

        /// <summary>
        /// Gets or sets the array of qualifier data (8 bytes each).
        /// </summary>
        public byte[][] Qualifiers { get; set; }

        /// <summary>
        /// Gets or sets the first ranking value.
        /// </summary>
        public int Rank1 { get; set; }

        /// <summary>
        /// Gets or sets the second ranking value.
        /// </summary>
        public int Rank2 { get; set; }

        /// <summary>
        /// Gets or sets the third ranking value.
        /// </summary>
        public int Rank3 { get; set; }

        /// <summary>
        /// Gets or sets the first year value.
        /// </summary>
        public short Year1 { get; set; }

        /// <summary>
        /// Gets or sets the second year value.
        /// </summary>
        public short Year2 { get; set; }

        /// <summary>
        /// Gets or sets the third year value.
        /// </summary>
        public short Year3 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown3 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown4 { get; set; }

        #region Extra

        /// <summary>
        /// Gets or sets the nation name (extra/computed field).
        /// </summary>
        public string Nation { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Competition"/> class by reading from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader containing the competition data.</param>
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

        /// <summary>
        /// Converts the competition data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized competition data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the competition data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the competition data to.</param>
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

        /// <summary>
        /// Returns a string representation of the competition.
        /// </summary>
        /// <returns>A string containing the competition's ID, UID, and full name.</returns>
        public override string ToString()
        {
            return $"{Id} - {Uid} - {FullName}";
        }
    }
}
