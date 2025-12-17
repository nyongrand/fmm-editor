namespace FMMLibrary
{
    /// <summary>
    /// Parses and manages club data from binary files.
    /// </summary>
    public class ClubParser : IParser<Club>
    {
        private readonly List<Club> items;

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

        /// <summary>
        /// List of all items
        /// </summary>
        public IReadOnlyList<Club> Items => items.AsReadOnly();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClubParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the club data.</param>
        private ClubParser(string path, BinaryReaderEx reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            items = new List<Club>(Count);
        }

        /// <summary>
        /// Asynchronously loads club data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load club data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="ClubParser"/> instance.</returns>
        public static async Task<ClubParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var reader = new BinaryReaderEx(fs);
            var parser = new ClubParser(path, reader);

            while (fs.Position < fs.Length)
            {
                var item = new Club(reader);
                if (item.Id == -1)
                    item.Id = parser.Items.Count;

                parser.items.Add(item);
            }

            return await Task.FromResult(parser);
        }

        /// <summary>
        /// Adds the specified club to the collection. The Id should always be -1 (0xFFFFFFFF).
        /// </summary>
        /// <param name="item">The club to add to the collection.</param>
        public void Add(Club item)
        {
            // Club.Id should always be -1 (0xFFFFFFFF) as per specification
            item.Id = -1;

            items.Add(item);
            Count++;
        }

        /// <summary>
        /// Converts the club data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized club data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);

            writer.Write(Header);
            writer.Write(Items.Count);
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