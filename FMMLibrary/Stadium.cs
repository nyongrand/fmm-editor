namespace FMMLibrary
{
    /// <summary>
    /// Represents a stadium record loaded from or written to the game data files.
    /// </summary>
    public class Stadium
    {
        /// <summary>
        /// Record index identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique stadium identifier used by the game database.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// <see cref="City"/> reference key from the game data.
        /// </summary>
        public short CityId { get; set; }

        /// <summary>
        /// Base seating capacity.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Maximum seating capacity after expansion.
        /// </summary>
        public int ExpansionCapacity { get; set; }

        /// <summary>
        /// Primary display name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Secondary display name.
        /// </summary>
        public string Name2 { get; set; }

        /// <summary>
        /// Record terminator byte used in the binary layout.
        /// </summary>
        public byte Terminator { get; set; }

        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }
        public byte Unknown5 { get; set; }

        /// <summary>
        /// Creates a stadium by reading its binary representation from the provided reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the stadium record.</param>
        public Stadium(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();
            CityId = reader.ReadInt16();
            Capacity = reader.ReadInt32();
            ExpansionCapacity = reader.ReadInt32();
            Name = reader.ReadString();
            Name2 = reader.ReadString();
            Terminator = reader.ReadByte();
            Unknown3 = reader.ReadByte();
            Unknown4 = reader.ReadByte();
            Unknown5 = reader.ReadByte();
        }

        /// <summary>
        /// Writes this stadium to a binary writer using the game file layout.
        /// </summary>
        /// <param name="writer">Binary writer to receive the stadium data.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);
            writer.Write(CityId);
            writer.Write(Capacity);
            writer.Write(ExpansionCapacity);
            writer.Write(Name);
            writer.Write(Name2);
            writer.Write(Terminator);
            writer.Write(Unknown3);
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
            return $"{Uid} {Name}";
        }
    }
}
