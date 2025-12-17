namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages nation data from binary files.
    /// </summary>
    public class NationParser
    {
        private readonly List<Nation> items;

        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File header, 8 bytes
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// Item count
        /// </summary>
        public short Count { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public IReadOnlyList<Nation> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="NationParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the nation data.</param>
        private NationParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            items = new List<Nation>(Count);
        }

        /// <summary>
        /// Asynchronously loads nation data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load nation data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="NationParser"/> instance.</returns>
        public static async Task<NationParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new NationParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new Nation(reader);
                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified person to the collection. The Id should always be -1 (0xFFFFFFFF).
        /// </summary>
        /// <param name="item">The person to add to the collection.</param>
        public void Add(Nation item)
        {
            items.Add(item);
            Count++;
        }

        /// <summary>
        /// Converts the nation data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized nation data.</returns>
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
