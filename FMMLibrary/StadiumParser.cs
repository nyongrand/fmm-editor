namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages stadium data from binary files.
    /// </summary>
    public class StadiumParser : IParser<Stadium>
    {
        private readonly List<Stadium> items;

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
        public IReadOnlyList<Stadium> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="StadiumParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the stadium data.</param>
        private StadiumParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            items = new List<Stadium>(Count);
        }

        /// <summary>
        /// Asynchronously loads stadium data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load stadium data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="StadiumParser"/> instance.</returns>
        public static async Task<StadiumParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new StadiumParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new Stadium(reader);
                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified continent to the collection.
        /// </summary>
        /// <param name="item">The stadium to add to the collection.</param>
        public void Add(Stadium item)
        {
            items.Add(item);
            Count++;
        }

        /// <summary>
        /// Converts the continent data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized continent data.</returns>
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
