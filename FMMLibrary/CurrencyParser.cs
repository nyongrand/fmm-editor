namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages currency data from binary files.
    /// </summary>
    public class CurrencyParser : IParser<Currency>
    {
        private readonly List<Currency> items;

        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File header, 8 bytes
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// Original item count when loading the file.
        /// </summary>
        public short Count { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public IReadOnlyList<Currency> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the currency data.</param>
        private CurrencyParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            items = new List<Currency>(Count);
        }

        /// <summary>
        /// Asynchronously loads currency data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load currency data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="CurrencyParser"/> instance.</returns>
        public static async Task<CurrencyParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new CurrencyParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new Currency(reader);
                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified currency to the collection.
        /// </summary>
        /// <param name="item">The currency to add to the collection.</param>
        public void Add(Currency item)
        {
            items.Add(item);
            Count++;
        }

        /// <summary>
        /// Converts the currency data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized currency data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);

            writer.Write(Header);
            writer.Write((short)Items.Count);
            foreach (var item in Items)
            {
                item.Write(writer);
            }

            return stream.ToArray();
        }

        /// <summary>
        /// Save data back to file path
        /// </summary>
        /// <param name="filepath">Optional file path. If null, saves to the original file path.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task Save(string? filepath = null)
        {
            var bytes = ToBytes();
            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes);
        }
    }
}
