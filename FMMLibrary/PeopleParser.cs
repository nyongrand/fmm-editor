namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages nation data from binary files.
    /// </summary>
    public class PeopleParser
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
        /// Initializes a new instance of the <see cref="`"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the nation data.</param>
        private PeopleParser(string path, BinaryReader reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            PaddingByte = reader.ReadByte();
            items = [];
        }

        /// <summary>
        /// Asynchronously loads nation data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load nation data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="PeopleParser"/> instance.</returns>
        public static async Task<PeopleParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var parser = new PeopleParser(path, reader);

            await Task.Run(() =>
            {
                while (ms.Position < ms.Length)
                {
                    var item = new People(reader);
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
        public void Add(People item)
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
