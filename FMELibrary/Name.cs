using System.Text;

namespace FMELibrary
{
    /// <summary>
    /// Represents a name entry with associated metadata.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Gets or sets the identifier for this name entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the nation identifier (stored as hexadecimal string).
        /// </summary>
        public string Nation { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets other associated data (stored as hexadecimal string).
        /// </summary>
        public string Others { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the actual name value.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Returns a string representation of the name entry.
        /// </summary>
        /// <returns>A string containing the ID and name value.</returns>
        public override string ToString()
        {
            return $"{Id} {Value}";
        }

        /// <summary>
        /// Converts the name entry to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized name data.</returns>
        public byte[] ToBytes()
        {
            var name = Encoding.UTF8.GetBytes(Value);
            var bytes = new List<byte> { 0x00, 0x00, 0x00, 0x00 };
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(Convert.FromHexString(Nation));
            bytes.AddRange(Convert.FromHexString(Others));
            bytes.AddRange(BitConverter.GetBytes(name.Length));
            bytes.AddRange(name);
            return bytes.ToArray();
        }
    }
}
