using System.Text;

namespace FMELibrary
{
    /// <summary>
    /// Parses and manages name data from binary files.
    /// </summary>
    public class NameParser
    {
        /// <summary>
        /// Gets or sets the file path of the source data.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the total count of name entries.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the file header (8 bytes).
        /// </summary>
        public byte[] Header { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of name entries. Throws an exception if accessed before Parse() is called.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when accessed before Parse() is called.</exception>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="NameParser"/> class.
        /// </summary>
        /// <param name="file">The file path to parse.</param>
        public NameParser(string file)
        {
            FilePath = file;
        }

        /// <summary>
        /// Asynchronously parses the name data from the file.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the list of parsed names.</returns>
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

        /// <summary>
        /// Asynchronously saves the name data to a file.
        /// </summary>
        /// <param name="filepath">Optional file path. If null, saves to the original file path.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
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

        /// <summary>
        /// Converts a byte array segment to a 32-bit integer.
        /// </summary>
        /// <param name="bytes">The source byte array.</param>
        /// <param name="start">The starting index in the byte array.</param>
        /// <param name="size">The number of bytes to convert (default is 4).</param>
        /// <returns>A 32-bit integer value.</returns>
        private static int ConvertToInt(byte[] bytes, int start, int size = 4)
        {
            return BitConverter.ToInt32(bytes.Skip(start).Take(size).ToArray());
        }
    }
}