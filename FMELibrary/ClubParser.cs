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

        public List<Club> Clubs
        {
            get
            {
                if (clubs == null)
                    throw new InvalidOperationException("Parse() is not called");

                return clubs;
            }

            set => clubs = value;
        }

        private List<Club>? clubs = null;

        public ClubParser(string file)
        {
            FilePath = file;
        }

        public async Task ParseAsync()
        {
            var data = await File.ReadAllBytesAsync(FilePath);

            Header = data.Take(8).ToArray();
            Count = BitConverter.ToInt32(data.Skip(8).Take(4).ToArray());
            Clubs = new List<Club>();

            var separator = "0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FF0000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
            var base64 = Convert.ToHexString(data.Skip(12).ToArray());
            var chunks = base64.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var chunk in chunks)
            {
                var bytes = Convert.FromHexString(chunk);

                try
                {
                    var currentIndex = 0;
                    Club club = new()
                    {
                        Id = BitConverter.ToInt32(Slice(bytes, ref currentIndex, 4)),
                        Uid = bytes.Slice(ref currentIndex, 4),
                    };

                    var len1 = BitConverter.ToInt32(Slice(bytes, ref currentIndex, 4));
                    club.FullName = Encoding.UTF8.GetString(Slice(bytes, ref currentIndex, len1));
                    currentIndex++;

                    var len2 = BitConverter.ToInt32(Slice(bytes, ref currentIndex, 4));
                    club.ShortName = Encoding.UTF8.GetString(Slice(bytes, ref currentIndex, len2));
                    currentIndex++;

                    var len3 = BitConverter.ToInt32(Slice(bytes, ref currentIndex, 4));
                    club.CodeName = Encoding.UTF8.GetString(Slice(bytes, ref currentIndex, len3));

                    club.Based = Convert.ToHexString(Slice(bytes, ref currentIndex, 2));
                    club.Nation = Convert.ToHexString(Slice(bytes, ref currentIndex, 2));

                    club.Others1 = Slice(bytes, ref currentIndex, 154);

                    club.League = Convert.ToHexString(Slice(bytes, ref currentIndex, 2));

                    club.Others2 = bytes.Skip(currentIndex).ToArray();

                    Clubs.Add(club);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static T[] Slice<T>(T[] originalArray, ref int currentIndex, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Number of elements to remove must be non-negative.");
            }

            if (currentIndex + length > originalArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Number of elements to remove cannot be greater than the number of elements in the list.");
            }

            T[] result = new T[length];
            Array.Copy(originalArray, currentIndex, result, 0, length);
            currentIndex += length;
            return result;
        }

        public async Task<List<Club>> ParseAsync2()
        {
            List<byte> bytes = (await File.ReadAllBytesAsync(FilePath)).ToList();

            Header = bytes.Slice(8);
            Count = BitConverter.ToInt32(bytes.Slice(4));

            Clubs = new List<Club>();
            while (bytes.Count > 0)
            {
                Club club = new()
                {
                    Id = BitConverter.ToInt32(bytes.Slice(4)),
                    Uid = bytes.Slice(4),
                };

                var len1 = BitConverter.ToInt32(bytes.Slice(4));
                club.FullName = Encoding.UTF8.GetString(bytes.Slice(len1));

                bytes.Slice(1);

                var len2 = BitConverter.ToInt32(bytes.Slice(4));
                club.ShortName = Encoding.UTF8.GetString(bytes.Slice(len2));

                bytes.Slice(1);

                var len3 = BitConverter.ToInt32(bytes.Slice(4));
                club.CodeName = Encoding.UTF8.GetString(bytes.Slice(len3));

                club.Based = Convert.ToHexString(bytes.Slice(2));
                club.Nation = Convert.ToHexString(bytes.Slice(2));

                club.Others1 = bytes.Slice(158);

                club.League = Convert.ToHexString(bytes.Slice(2));

                club.Others2 = bytes.Slice(163);

                var nextId = BitConverter.GetBytes(club.Id + 1);
                for (var i = 0; i < bytes.Count; i++)
                {
                    if (bytes[i] == nextId[0] &&
                        bytes[i + 1] == nextId[1] &&
                        bytes[i + 2] == nextId[2] &&
                        bytes[i + 3] == nextId[3])
                    {
                        if (i > 0)
                        {
                            club.Others1 = club.Others1.Concat(bytes.Slice(i)).ToArray();
                        }

                        break;
                    }
                }

                Clubs.Add(club);

                if (club.Id == 431)
                {

                }

                Console.WriteLine(club.FullName);

                if (Clubs.Count == Count) break;
            }

            return Clubs;
        }

        public List<Club> Parse()
        {
            var clubs = new List<Club>();
            using (var file = File.OpenRead(FilePath))
            using (var reader = new BinaryReader(file))
            {
                Header = reader.ReadBytes(8);
                Count = reader.ReadInt32();

                while (file.Position < file.Length)
                {
                    var club = ReadClub(reader);
                    clubs.Add(club);

                    Console.WriteLine(club.FullName);

                    if (clubs.Count == Count) break;
                }
            }

            return clubs;
        }

        private static Club ReadClub(BinaryReader reader)
        {
            var club = new Club
            {
                Id = reader.ReadInt32(),
                Uid = reader.ReadBytes(4),
                FullName = ReadString(reader)
            };

            reader.ReadByte();
            club.ShortName = ReadString(reader);

            reader.ReadByte();
            club.CodeName = ReadString(reader);

            club.Based = Convert.ToHexString(reader.ReadBytes(2));
            club.Nation = Convert.ToHexString(reader.ReadBytes(2));
            club.Others1 = reader.ReadBytes(158);
            club.League = Convert.ToHexString(reader.ReadBytes(2));
            club.Others2 = reader.ReadBytes(165);

            return club;
        }

        private static string ReadString(BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return Encoding.UTF8.GetString(reader.ReadBytes(length));
        }

        public async Task Save(string? filepath = null)
        {
            List<byte> bytes = new(Header);
            bytes.AddRange(BitConverter.GetBytes(Clubs.Count));

            foreach (var name in Clubs)
            {
                bytes.AddRange(name.ToBytes());
            }

            await File.WriteAllBytesAsync(filepath ?? FilePath, bytes.ToArray());
        }
    }
}