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

        public NationParser(string path)
        {
            using var file = File.OpenRead(path);
            using var reader = new BinaryReader(file);

            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            Items = new List<Nation>();

            while (file.Position < file.Length)
            {
                try
                {
                    var item = new Nation(reader);
                    Items.Add(item);
                }
                catch
                {
                    var d = Items.Last().ToString();
                    break;
                }
            }
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
