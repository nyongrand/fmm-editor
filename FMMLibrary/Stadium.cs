namespace FMMLibrary
{
    /// <summary>
    /// Represents a stadium entry with identifiers, metadata, and display name.
    /// </summary>
    public class Stadium
    {
        /// <summary>
        /// Internal identifier (can be -1 for new items).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique database identifier.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Unknown 16-bit field following identifiers.
        /// </summary>
        public short Unknown1 { get; set; }

        /// <summary>
        /// Unknown 32-bit value (reserved/flags).
        /// </summary>
        public int Unknown2 { get; set; }

        /// <summary>
        /// Unknown 32-bit value (reserved/flags).
        /// </summary>
        public int Unknown3 { get; set; }

        /// <summary>
        /// Display name of the stadium.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unknown 32-bit value (reserved/flags).
        /// </summary>
        public int Unknown4 { get; set; }

        /// <summary>
        /// Unknown 32-bit value (reserved/flags).
        /// </summary>
        public int Unknown5 { get; set; }

        /// <summary>
        /// Creates a stadium by reading its binary representation from the provided reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the stadium record.</param>
        public Stadium(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();
            Unknown1 = reader.ReadInt16();
            Unknown2 = reader.ReadInt32();
            Unknown3 = reader.ReadInt32();
            Name = reader.ReadString();
            Unknown4 = reader.ReadInt32();
            Unknown5 = reader.ReadInt32();
        }

        /// <summary>
        /// Writes this stadium to a binary writer using the game file layout.
        /// </summary>
        /// <param name="writer">Binary writer to receive the stadium data.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Name);
            writer.Write(Unknown4);
            writer.Write(Unknown5);
        }

        /// <summary>
        /// Serializes the stadium to a byte array using the game file format.
        /// </summary>
        /// <returns>Byte array containing the serialized stadium.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        public override string ToString()
        {
            return $"{Id} {Name}";
        }
    }
}
