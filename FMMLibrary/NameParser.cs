namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages name data from binary files.
    /// </summary>
    public class NameParser
    {
        private readonly List<Name> items;

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
        public int Count { get; private set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public IReadOnlyList<Name> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="NameParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the name data.</param>
        private NameParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            items = new List<Name>(Count);
        }

        /// <summary>
        /// Asynchronously loads name data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load name data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="NameParser"/> instance.</returns>
        public static async Task<NameParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new NameParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new Name(reader);
                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified name to the collection, assigning a unique identifier if necessary.
        /// </summary>
        /// <param name="item">The name to add to the collection. If the item's Id is less than or equal to zero, a new unique Id is assigned.</param>
        public void Add(Name item)
        {
            var nextId = Items.Count > 0 ? Items.Max(x => x.Id) + 1 : 0;
            item.Id = item.Id >= 0 ? item.Id : nextId;
            items.Add(item);
            Count++;
        }

        public void Replace(IEnumerable<Name> names)
        {
            items.Clear();
            Count = 0;

            foreach (var item in names)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Converts the name data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized name data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);

            writer.Write(Header);
            writer.Write(Count);
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
