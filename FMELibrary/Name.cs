using System.Text;

namespace FMELibrary
{
    public class Name
    {
        public int Id { get; set; }
        public byte[] Nation { get; set; } = Array.Empty<byte>();
        public byte[] Others { get; set; } = Array.Empty<byte>();
        public int Length { get; set; }
        public string Value { get; set; } = string.Empty;

        public string NationValue => BitConverter.ToString(Nation);
        public string OthersValue => BitConverter.ToString(Others);

        public override string ToString()
        {
            return $"{Id} {Value}";
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte> { 0x00, 0x00, 0x00, 0x00 };
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(Nation);
            bytes.AddRange(Others);
            bytes.AddRange(BitConverter.GetBytes(Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(Value));
            return bytes.ToArray();
        }
    }
}
