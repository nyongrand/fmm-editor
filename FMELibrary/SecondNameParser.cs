using System.Text;

namespace FMELibrary
{
    public class SecondNameParser
    {
        public string FilePath { get; set; }
        public int Count { get; set; }
        public byte[] Header { get; set; } = Array.Empty<byte>();

        public List<Name> Names
        {
            get
            {
                if (names == null)
                    throw new InvalidOperationException("Parse() is not called");

                return names;
            }

            set => names = value;
        }

        private List<Name>? names = null;

        public SecondNameParser(string file)
        {
            FilePath = file;
        }

        public async Task<List<Name>> Parse()
        {
            byte[] bytes = await File.ReadAllBytesAsync(FilePath);

            Header = bytes.Take(8).ToArray();
            Count = ConvertToInt(bytes, 8);
            Names = new List<Name>();

            for (int i = 12; i < bytes.Length; i++)
            {
                Name name = new()
                {
                    Id = ConvertToInt(bytes, i + 4),
                    Nation = Convert.ToHexString(bytes.Skip(i + 8).Take(4).ToArray()),
                    Others = Convert.ToHexString(bytes.Skip(i + 12).Take(4).ToArray()),
                };

                var length = ConvertToInt(bytes, i + 16);
                name.Value = Encoding.UTF8
                    .GetString(bytes.Skip(i + 20)
                    .Take(length)
                    .ToArray());

                Names.Add(name);

                i += 19 + length;
                if (Names.Count == Count) break;
            }

            return Names;
        }

        public async Task Save(string? filepath = null)
        {
            List<byte> bytes = new(Header);
            bytes.AddRange(BitConverter.GetBytes(Names.Count));

            foreach (var name in Names)
            {
                bytes.AddRange(name.ToBytes());
            }

            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes.ToArray());
        }

        private static int ConvertToInt(byte[] bytes, int start, int size = 4)
        {
            return BitConverter.ToInt32(bytes.Skip(start).Take(size).ToArray());
        }
    }
}