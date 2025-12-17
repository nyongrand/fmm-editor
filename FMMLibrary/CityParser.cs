namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages city data from binary files.
    /// </summary>
    public class CityParser
    {
        private readonly List<City> items;

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
        public IReadOnlyList<City> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="CityParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the city data.</param>
        private CityParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            items = [];
        }

        /// <summary>
        /// Asynchronously loads nation data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load city data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="CityParser"/> instance.</returns>
        public static async Task<CityParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            using var reader = new BinaryReaderEx(ms);
            var parser = new CityParser(path, reader);

            await Task.Run(() =>
            {
                while (ms.Position < ms.Length)
                {
                    var item = new City(reader);
                    parser.items.Add(item);
                }
            });

            return parser;
        }

        /// <summary>
        /// Adds the specified competition to the collection.
        /// </summary>
        /// <param name="item">The competition to add to the collection.</param>
        public void Add(City item)
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
