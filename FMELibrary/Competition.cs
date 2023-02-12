using System.Text;

namespace FMELibrary
{
    public class Competition
    {
        public int Id { get; set; }
        public int Uid { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string ShortName { get; set; } = string.Empty;

        public string CodeName { get; set; } = string.Empty;

        public byte[] Others1 { get; set; } = Array.Empty<byte>();

        public override string ToString()
        {
            return $"{Uid} => {Id}, // {FullName}";
            //return $"{Id} - {Uid} - {FullName}";
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Id));

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(FullName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(FullName));
            bytes.Add(0x00);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(ShortName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(ShortName));
            bytes.Add(0x00);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(CodeName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(CodeName));

            bytes.AddRange(Others1);

            return bytes.ToArray();
        }
    }
}
