using System.Diagnostics;

namespace FMELibrary
{
    public class ClubParser
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
        public int Count { get; set; }

        /// <summary>
        /// List of all items
        /// </summary>
        public List<Club> Items { get; set; }

        public ClubParser(string path)
        {
            using var file = File.OpenRead(path);
            using var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using var reader = new BinaryReader(memoryStream);

            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            Items = new List<Club>();

            while (file.Position < file.Length)
            {
                var item = new Club(reader);
                Items.Add(item);
            }
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

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
        /// <param name="filepath"></param>
        /// <returns></returns>
        public async Task Save(string? filepath = null)
        {
            var bytes = ToBytes();
            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes);
        }
    }
}