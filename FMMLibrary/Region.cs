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
        /// Single terminator byte that follows <see cref="Name"/> in the binary data.
        /// </summary>
        public byte Terminator { get; set; }

        /// <summary>
        /// Identifier of the <see cref="Nation"/> this region belongs to.
        /// </summary>
        public short NationId { get; set; }

        /// <summary>
        /// Unknown flag/metadata byte.
        /// </summary>
        public short WeatherId { get; set; }

        /// <summary>
        /// Creates a region by reading its binary representation from the provided reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the region record.</param>
        public Region(BinaryReaderEx reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();
            Name = reader.ReadString();
            Terminator = reader.ReadByte();
            NationId = reader.ReadInt16();
            WeatherId = reader.ReadInt16();
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
            writer.Write(Terminator);
            writer.Write(NationId);
            writer.Write(WeatherId);
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
