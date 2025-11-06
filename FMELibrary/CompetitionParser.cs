namespace FMELibrary
{
    /// <summary>
    /// Parses and manages competition data from binary files.
    /// </summary>
    public class CompetitionParser
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
        /// Item count
        /// </summary>
        public short Count { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public List<Competition> Items { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompetitionParser"/> class.
        /// </summary>
        /// <param name="path">The file path of the source data.</param>
        /// <param name="reader">The binary reader containing the competition data.</param>
        private CompetitionParser(string path, BinaryReader reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            Items = [];
        }

        /// <summary>
        /// Asynchronously loads competition data from the specified file path.
        /// </summary>
        /// <param name="path">The file path to load competition data from.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the loaded <see cref="CompetitionParser"/> instance.</returns>
        public static async Task<CompetitionParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var parser = new CompetitionParser(path, reader);

            await Task.Run(() =>
            {
                while (ms.Position < ms.Length)
                {
                    var item = new Competition(reader);
                    parser.Items.Add(item);
                }
            });

            return parser;
        }

        /// <summary>
        /// Converts the competition data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized competition data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

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
