using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMELibrary
{
    /// <summary>
    /// 03 00 00 00 - ID
    /// F8 FF FF FF - Unknown
    /// 08 00 00 00 - Name Length
    /// 42 6F 74 73 77 61 6E 61 - Name
    /// 00          - Separator
    /// 08 00 00 00 - Name Length
    /// 42 6f 74 73 77 61 6e 61 - Name
    /// 00          - Separator
    /// 03 00 00 00 -
    /// 42 4f 54 -
    /// 03 00 03 00 - Country + League
    /// 84 0c 7d 26 21 04 fe 7f 20 20 ff 7f 02 20 21 04 1f 33 21 04 
    /// 21 04 21 04 1f 33 fe 7f ff 32 fe 7f 20 20 0a 20 21 04 fe 7f 
    /// 21 04 21 04 21 04 fe 7f 21 04 fe 7f 21 04 21 04 ff 20 20 20 
    /// ff 7f 20 20 20 20 20 20 ff 7f 20 20 ff 7f 20 20 20 20 01 20 
    /// ff 7f 20 70 20 20 20 20 ff 7f 20 70 20 20 20 70 20 20 20 20 
    /// 01 20 ff 7f 20 70 20 20 20 20 ff 7f 20 70 20 20 20 70 20 20 
    /// 20 20 01 20 ff 7f 20 70 20 20 20 20 ff 7f 20 70 20 20 20 70 
    /// 20 20 20 20 20 ff ff ff ff ff ff ff ff 20 ff ff ff ff 20 34 
    /// 03 ff ff 08 20 20 20 ff ff ff ff ff ff ff 20 20 b8 0b 17 32 
    /// 20 20 76 73 20 20 ff ff ff ff ff ff ff ff ff ff ff ff 20 20 
    /// 20 20 ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff 
    /// ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff 
    /// ff ff ff ff ff ff ff ff ff ff 01 20 20 20 ff 20 20 ff 20 20 
    /// ff 20 20 ff 20 20 ff 20 20 ff 20 20 ff 20 20 ff 20 20 ff 20 
    /// 20 ff 20 20 ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff 
    /// ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff ff 
    /// ff ff ff ff ff 
    /// </summary>
    public class Club
    {
        /// <summary>
        /// 4 bytes
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 4 bytes
        /// </summary>
        public byte[] Uid { get; set; } = Array.Empty<byte>();

        public string FullName { get; set; } = string.Empty;

        public string ShortName { get; set; } = string.Empty;

        /// <summary>
        /// 3 bytes
        /// </summary>
        public string CodeName { get; set; } = string.Empty;

        /// <summary>
        /// 2 bytes
        /// </summary>
        public string Based { get; set; } = string.Empty;
        /// <summary>
        /// 2 bytes
        /// </summary>
        public string Nation { get; set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public byte[] Others1 { get; set; } = Array.Empty<byte>();

        public string League { get; set; } = string.Empty;

        public byte[] Others2 { get; set; } = Array.Empty<byte>();

        public override string ToString()
        {
            return $"{Id} {FullName}";
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(Uid);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(FullName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(FullName));
            bytes.Add(0x00);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(ShortName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(ShortName));
            bytes.Add(0x00);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(CodeName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(CodeName));

            bytes.AddRange(Convert.FromHexString(Based));
            bytes.AddRange(Convert.FromHexString(Nation));
            return bytes.ToArray();
        }
    }
}
