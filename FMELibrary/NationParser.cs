namespace FMELibrary
{
    public class NationParser
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
        public List<Nation> Items { get; set; }

        private NationParser(string path, BinaryReader reader)
        {
            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            Items = [];
        }

        public static async Task<NationParser> Load(string path)
        {
            using var fs = File.OpenRead(path);
            using var ms = new MemoryStream();
            fs.CopyTo(ms);
            ms.Position = 0;

            using var reader = new BinaryReader(ms);
            var parser = new NationParser(path, reader);

            await Task.Run(() =>
            {
                while (ms.Position < ms.Length)
                {
                    var item = new Nation(reader);
                    parser.Items.Add(item);
                }
            });

            return parser;
        }

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
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task Save(string? filepath = null)
        {
            var bytes = ToBytes();
            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes);
        }
    }
}
