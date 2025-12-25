namespace FMMLibrary
{
    /// <summary>
    /// Represents an in-game currency entry with identifier, display name, and exchange rate.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// Unique database identifier.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Display name of the currency.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Exchange rate relative to the game's base currency.
        /// </summary>
        public int ExchangeRate { get; set; }

        /// <summary>
        /// Creates a currency by reading its binary representation from the provided reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the currency record.</param>
        public Currency(BinaryReaderEx reader)
        {
            Uid = reader.ReadInt32();
            Name = reader.ReadString();
            ExchangeRate = reader.ReadInt32();
        }

        /// <summary>
        /// Writes this currency to a binary writer using the game file layout.
        /// </summary>
        /// <param name="writer">Binary writer to receive the currency data.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Uid);
            writer.Write(Name);
            writer.Write(ExchangeRate);
        }

        /// <summary>
        /// Serializes the currency to a byte array using the game file format.
        /// </summary>
        /// <returns>Byte array containing the serialized currency.</returns>
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
