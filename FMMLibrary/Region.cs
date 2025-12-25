namespace FMMLibrary
{
    /// <summary>
    /// Represents a geographic region with identifiers, display name, and related metadata.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Short identifier used by references elsewhere in the database.
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Unique database identifier.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Display name of the region.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Identifier of the nation this region belongs to.
        /// </summary>
        public short NationId { get; set; }

        /// <summary>
        /// Unknown flag/metadata byte.
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// Unknown flag/metadata byte.
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Unknown flag/metadata byte.
        /// </summary>
        public byte Unknown3 { get; set; }

        /// <summary>
        /// Creates a region by reading its binary representation from the provided reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the region record.</param>
        public Region(BinaryReaderEx reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();
            Name = reader.ReadString();
            NationId = reader.ReadInt16();
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            Unknown3 = reader.ReadByte();
        }

        /// <summary>
        /// Writes this region to a binary writer using the game file layout.
        /// </summary>
        /// <param name="writer">Binary writer to receive the region data.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);
            writer.Write(Name);
            writer.Write(NationId);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
        }

        /// <summary>
        /// Serializes the region to a byte array using the game file format.
        /// </summary>
        /// <returns>Byte array containing the serialized region.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
