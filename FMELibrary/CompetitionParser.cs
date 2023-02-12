using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

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

        public List<Competition> Items
        {
            get
            {
                if (items == null)
                    throw new InvalidOperationException("Parse() is not called");

                return items;
            }

            set => items = value;
        }

        private List<Competition>? items = null;

        public CompetitionParser(string path)
        {
            FilePath = path;

            using var file = File.OpenRead(FilePath);
            using var reader = new BinaryReader(file);

            Header = reader.ReadBytes(8);
            Count = reader.ReadInt32();
            Items = new List<Competition>();

            while (file.Position < file.Length)
            {
                try
                {
                    var item = Read(reader);
                    item.Id = Items.Count;
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

        private static Competition Read(BinaryReader reader)
        {
            var item = new Competition
            {
                Uid = reader.ReadInt32(),
                FullName = ReadString(reader)
            };

            reader.ReadByte();
            item.ShortName = ReadString(reader);

            reader.ReadByte();
            item.CodeName = ReadString(reader);

            var len = GetOthersLength(item.Uid);
            item.Others1 = reader.ReadBytes(len);

            return item;
        }

        private static string ReadString(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        private static int GetOthersLength(int uid)
        {
            return uid switch
            {
                40 => 263, // Major League Soccer
                90 => 503, // European International League
                91 => 167, // European International League Division A
                92 => 167, // European International League Division B
                93 => 167, // European International League Division C
                94 => 95,  // European International League Division D

                100104 => 359, // World Cup African Qualifying Section
                100105 => 583, // World Cup European Qualifying Section

                102415 => 791, // Copa Libertadores
                127286 => 415, // Asian Champions League
                127299 => 471, // African Champions League

                136408 => 239, // Korean FA Cup
                145509 => 47,  // North American Gold Cup
                157149 => 55,  // North American U20 Championship
                219740 => 191, // North American League
                _ => 39,
            };
        }
    }
}
