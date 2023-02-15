namespace FMELibrary
{
    public class CompetitionParser
    {
        public string FilePath { get; set; }

        /// <summary>
        /// File header, 8 bytes
        /// </summary>
        public byte[] Header { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Item count, 4 bytes
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public List<Competition> Items { get; set; }

        public CompetitionParser(string path)
        {
            using var file = File.OpenRead(path);
            using var reader = new BinaryReader(file);

            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt16();
            Items = new List<Competition>();

            while (file.Position < file.Length)
            {
                try
                {
                    var item = new Competition(reader);
                    Items.Add(item);
                }
                catch
                {
                    var d = Items.Last().ToString();
                    break;
                }
            }
        }
    }
}
