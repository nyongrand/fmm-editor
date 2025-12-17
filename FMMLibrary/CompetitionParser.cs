namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages competition data from binary files.
    /// </summary>
    public class CompetitionParser : IParser<Competition>
    {
        private readonly List<Competition> items;

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
        public IReadOnlyList<Competition> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the competition data.</param>
        private CompetitionParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            items = new List<Competition>(Count);
        }

        /// <summary>
        /// Asynchronously loads competition data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load competition data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="CompetitionParser"/> instance.</returns>
        public static async Task<CompetitionParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new CompetitionParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new Competition(reader);
                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified competition to the collection.
        /// </summary>
        /// <param name="item">The competition to add to the collection.</param>
        public void Add(Competition item)
        {
            items.Add(item);
            Count++;
        }

        /// <summary>
        /// Converts the competition data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized competition data.</returns>
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
