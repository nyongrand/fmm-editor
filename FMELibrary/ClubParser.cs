using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMELibrary
{
    public class ClubParser
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

        public List<Club> Items { get; set; }

        public ClubParser(string path)
        {
            using var file = File.OpenRead(path);
            using var reader = new BinaryReader(file);

            FilePath = path;
            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            Items = new List<Club>();

            while (file.Position < file.Length)
            {
                try
                {
                    var item = new Club(reader);
                    Items.Add(item);
                }
                catch
                {
                    var d = Items.Last().ToString();
                    break;
                }

                if (Items.Count == Count) break;
            }
        }

        public async Task Save(string? filepath = null)
        {
            List<byte> bytes = new(Header);
            bytes.AddRange(BitConverter.GetBytes(Items.Count));

            foreach (var name in Items)
            {
                bytes.AddRange(name.ToBytes());
            }

            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes.ToArray());
        }
    }
}