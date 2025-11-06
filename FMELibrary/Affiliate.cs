using System.Reflection.PortableExecutable;

namespace FMELibrary
{
    /// <summary>
    /// Represents an affiliation relationship between two clubs.
    /// </summary>
    public class Affiliate
    {
        /// <summary>
        /// Gets or sets an unknown integer value.
        /// </summary>
        public int Unknown1 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the first club in the affiliation.
        /// </summary>
        public int Club1Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the second club in the affiliation.
        /// </summary>
        public int Club2Id { get; set; }

        /// <summary>
        /// Gets or sets the start day of the affiliation.
        /// </summary>
        public short StartDay { get; set; }

        /// <summary>
        /// Gets or sets the start year of the affiliation.
        /// </summary>
        public short StartYear { get; set; }

        /// <summary>
        /// Gets or sets the end day of the affiliation.
        /// </summary>
        public short EndDay { get; set; }

        /// <summary>
        /// Gets or sets the end year of the affiliation.
        /// </summary>
        public short EndYear { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Affiliate"/> class by reading from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader containing the affiliate data.</param>
        public Affiliate(BinaryReader reader)
        {
            Unknown1 = reader.ReadInt32();
            Club1Id = reader.ReadInt32();
            Club2Id = reader.ReadInt32();
            StartDay = reader.ReadInt16();
            StartYear = reader.ReadInt16();
            EndDay = reader.ReadInt16();
            EndYear = reader.ReadInt16();
            Unknown2 = reader.ReadByte();
        }

        /// <summary>
        /// Writes the affiliate data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the affiliate data to.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Club1Id);
            writer.Write(Club2Id);
            writer.Write(StartDay);
            writer.Write(StartYear);
            writer.Write(EndDay);
            writer.Write(EndYear);
            writer.Write(Unknown2);
        }
    }
}
