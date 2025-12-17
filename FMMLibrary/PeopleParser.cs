namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages people data from binary files.
    /// </summary>
    public class PeopleParser : IParser<People>
    {
        private readonly List<People> items;

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
        public int Count { get; set; }

        public byte PaddingByte { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public IReadOnlyList<People> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="PeopleParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the people data.</param>
        private PeopleParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            PaddingByte = reader.ReadByte();
            items = new List<People>(Count);
        }

        /// <summary>
        /// Asynchronously loads people data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load people data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="PeopleParser"/> instance.</returns>
        public static async Task<PeopleParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new PeopleParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new People(reader);
                if (item.Id == -1)
                    item.Id = parser.Items.Count;
                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified person to the collection. The Id should always be -1 (0xFFFFFFFF).
        /// </summary>
        /// <param name="item">The person to add to the collection.</param>
        public void Add(People item)
        {
            // People.Id should always be -1 (0xFFFFFFFF) as per specification
            item.Id = -1;

            items.Add(item);
            Count++;
        }

        /// <summary>
        /// Converts the people data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized people data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);

            writer.Write(Header);
            writer.Write(Count);
            writer.Write(PaddingByte);
            foreach (var item in Items)
                item.Write(writer);

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
