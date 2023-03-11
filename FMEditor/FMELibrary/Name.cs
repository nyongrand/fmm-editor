using System.Text;

namespace FMELibrary
{
    public class Name
    {
        public int Id { get; set; }
        public string Nation { get; set; } = string.Empty;
        public string Others { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{Id} {Value}";
        }

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
