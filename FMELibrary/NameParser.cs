namespace FMELibrary
{
    /// <summary>
    /// Parses and manages nation data from binary files.
    /// </summary>
    public class NameParser
    {
        /// <summary>
        /// File path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File header, 8 bytes
        /// </summary>
        public byte[] Header { get; set; }

        /// <summary>
        /// Item count when loading the file.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public IReadOnlyList<Name> Items => items.AsReadOnly();

        private readonly List<Name> items;

        /// <summary>
        /// Initializes a new instance of the <see cref="NationParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the nation data.</param>
        private NameParser(string path, BinaryReader reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            items = [];
        }

        /// <summary>
        /// Asynchronously loads nation data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load nation data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="NameParser"/> instance.</returns>
        public static async Task<NameParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var parser = new NameParser(path, reader);

            await Task.Run(() =>
            {
                while (ms.Position < ms.Length)
                {
                    var item = new Name(reader);
                    parser.items.Add(item);
                }
            });

            return parser;
        }

        /// <summary>
        /// Adds the specified name to the collection, assigning a unique identifier if necessary.
        /// </summary>
        /// <param name="item">The name to add to the collection. If the item's Id is less than or equal to zero, a new unique Id is
        /// assigned.</param>
        public void Add(Name item)
        {
            var nextId = Items.Max(x => x.Id) + 1;
            item.Id = item.Id > 0 ? item.Id : nextId;
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
            using var writer = new BinaryWriter(stream);

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
