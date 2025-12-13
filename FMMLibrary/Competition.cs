using Color = System.Drawing.Color;

namespace FMMLibrary
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
        /// Unique identifier for the competition.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// The full name of the competition.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Full name terminator byte.
        /// </summary>
        public byte FullNameTerminator { get; set; }

        /// <summary>
        /// The abbreviated, short name of the competition.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Short name terminator byte.
        /// </summary>
        public byte ShortNameTerminator { get; set; }

        /// <summary>
        /// A three-letter identification mark for the competition for use in small spaces.
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// Gets or sets the type of competition.<br/>
        /// Domestic Top Division<br/>
        /// Domestic Division
        /// Domestic Main Cup
        /// Domestic League Cup
        /// Domesttic Cup
        /// Super Cup
        /// Reserve Division
        /// U23 Division
        /// U22 Division
        /// U21 Division
        /// U20 Division
        /// U19 Division
        /// U18 Division
        /// Reserve Cup
        /// 
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
        /// The primary text color of the competition's title bar.
        /// </summary>
        public Color ForegroundColor { get; set; }

        /// <summary>
        /// The primary background color of the competition's title bar.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Competition's reputation, 0-200 scale.
        /// </summary>
        public short Reputation { get; set; }

        /// <summary>
        /// Gets or sets the competition level.
        /// </summary>
        public byte Level { get; set; }

        /// <summary>
        /// Indicates the parent competition identifier, -1 if none.
        /// </summary>
        public short ParentCompetitionId { get; set; }

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
        /// Player gender
        /// </summary>
        public bool IsWomen { get; set; }

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
        public Competition(BinaryReaderEx reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();

            FullName = reader.ReadString();
            FullNameTerminator = reader.ReadByte();
            ShortName = reader.ReadString();
            ShortNameTerminator = reader.ReadByte();
            CodeName = reader.ReadString();

            Type = reader.ReadByte();
            ContinentId = reader.ReadInt16();
            NationId = reader.ReadInt16();
            ForegroundColor = reader.ReadColor();
            BackgroundColor = reader.ReadColor();
            Reputation = reader.ReadInt16();
            Level = reader.ReadByte();
            ParentCompetitionId = reader.ReadInt16();

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
            IsWomen = reader.ReadBoolean();
        }

        /// <summary>
        /// Converts the competition data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized competition data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the competition data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the competition data to.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);

            writer.Write(FullName);
            writer.Write(FullNameTerminator);
            writer.Write(ShortName);
            writer.Write(ShortNameTerminator);
            writer.Write(CodeName);

            writer.Write(Type);
            writer.Write(ContinentId);
            writer.Write(NationId);
            writer.Write(ForegroundColor);
            writer.Write(BackgroundColor);
            writer.Write(Reputation);
            writer.Write(Level);
            writer.Write(ParentCompetitionId);

            writer.Write(Qualifiers.Length);
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                writer.Write(Qualifiers[i]);
            }

            writer.Write(Rank1);
            writer.Write(Rank2);
            writer.Write(Rank3);
            writer.Write(Year1);
            writer.Write(Year2);
            writer.Write(Year3);
            writer.Write(Unknown3);
            writer.Write(IsWomen);
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
