using System.Text;

namespace FMELibrary
{
    public class Club
    {
        /// <summary>
        /// 4 bytes
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 4 bytes
        /// </summary>
        public int Uid { get; set; }

        public string FullName { get; set; }
        public byte Unknown1 { get; set; }

        public string ShortName { get; set; }
        public byte Unknown2 { get; set; }

        public string CodeName { get; set; }

        /// <summary>
        /// 2 bytes
        /// </summary>
        public short Based { get; set; }
        /// <summary>
        /// 2 bytes
        /// </summary>
        public short Nation { get; set; }

        /// <summary>
        /// 2x6 bytes
        /// </summary>
        public byte[] Colors { get; set; }

        /// <summary>
        /// 10x6 bytes
        /// </summary>
        public Kit[] Kits { get; set; }

        public byte Status { get; set; }
        public byte Academy { get; set; }
        public byte Facilities { get; set; }
        public short AttAvg { get; set; }
        public short AttMin { get; set; }
        public short AttMax { get; set; }
        public byte Reserves { get; set; }

        public short League { get; set; }
        public short Unknown4 { get; set; }
        public byte Unknown5 { get; set; }
        public short Stadium { get; set; }
        public short LastLeague { get; set; }

        public byte[] Unknown6 { get; set; }

        public byte LeaguePos { get; set; }
        public short Reputation { get; set; }

        public byte[] Unknown7 { get; set; }
        public Affiliate[] Affiliates { get; set; }
        public int[] Players { get; set; }
        public int[] Unknown8 { get; set; }
        public int MainClub { get; set; }
        public int IsNational { get; set; }
        public byte[] Unknown9 { get; set; }
        public byte[] Unknown10 { get; set; }

        public override string ToString()
        {
            return $"{Id} {FullName}";
        }

        public Club(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            FullName = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadByte();
            ShortName = reader.ReadString(reader.ReadInt32());
            Unknown2 = reader.ReadByte();
            CodeName = reader.ReadString(reader.ReadInt32());

            Based = reader.ReadInt16();
            Nation = reader.ReadInt16();

            Colors = reader.ReadBytes(12);
            Kits = new Kit[6];
            for (int i = 0; i < Kits.Length; i++)
            {
                Kits[i] = new Kit(reader);
            }

            Status = reader.ReadByte();
            Academy = reader.ReadByte();
            Facilities = reader.ReadByte();
            AttAvg = reader.ReadInt16();
            AttMin = reader.ReadInt16();
            AttMax = reader.ReadInt16();
            Reserves = reader.ReadByte();
            League = reader.ReadInt16();

            Unknown4 = reader.ReadInt16();
            Unknown5 = reader.ReadByte();
            Stadium = reader.ReadInt16();
            LastLeague = reader.ReadInt16();

            Unknown6 = reader.ReadBytes(12);

            LeaguePos = reader.ReadByte();
            Reputation = reader.ReadInt16();
            Unknown7 = reader.ReadBytes(20);

            Affiliates = new Affiliate[reader.ReadInt16()];
            for (int i = 0; i < Affiliates.Length; i++)
            {
                Affiliates[i] = new Affiliate(reader);
            }

            Players = new int[reader.ReadInt16()];
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i] = reader.ReadInt32();
            }

            Unknown8 = new int[11];
            for (int i = 0; i < Unknown8.Length; i++)
            {
                Unknown8[i] = reader.ReadInt32();
            }

            MainClub = reader.ReadInt32();
            IsNational = reader.ReadInt16();

            Unknown9 = reader.ReadBytes(33);
            Unknown10 = reader.ReadBytes(40);
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(BitConverter.GetBytes(Uid));

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(FullName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(FullName));
            bytes.Add(0x00);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(ShortName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(ShortName));
            bytes.Add(0x00);

            bytes.AddRange(BitConverter.GetBytes(Encoding.UTF8.GetBytes(CodeName).Length));
            bytes.AddRange(Encoding.UTF8.GetBytes(CodeName));

            return bytes.ToArray();
        }
    }
}
