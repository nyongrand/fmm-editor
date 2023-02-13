using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;

namespace FMELibrary
{
    public class Competition
    {
        public short Id { get; set; }
        public int Uid { get; set; }

        public string FullName { get; set; } = string.Empty;
        public byte Unknown1 { get; set; }

        public string ShortName { get; set; } = string.Empty;
        public byte Unknown2 { get; set; }

        public string CodeName { get; set; } = string.Empty;
        public byte Type { get; set; }

        public short Continent { get; set; }
        public short Nation { get; set; }
        public byte[] Colors { get; set; }
        //public Color Color1 { get; set; }
        //public Color Color2 { get; set; }
        public short Reputation { get; set; }
        public byte Level { get; set; }
        public short MainComp { get; set; }
        public byte[][] Qualifiers { get; set; }
        public int Rank1 { get; set; }
        public int Rank2 { get; set; }
        public int Rank3 { get; set; }
        public short Year1 { get; set; }
        public short Year2 { get; set; }
        public short Year3 { get; set; }
        public byte Unknown3 { get; set; }

        public Competition(BinaryReader reader)
        {
            Id = reader.ReadInt16();
            Uid = reader.ReadInt32();

            FullName = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadByte();
            ShortName = reader.ReadString(reader.ReadInt32());
            Unknown2 = reader.ReadByte();
            CodeName = reader.ReadString(reader.ReadInt32());

            Type = reader.ReadByte();
            Continent = reader.ReadInt16();
            Nation = reader.ReadInt16();
            Colors = reader.ReadBytes(4);
            //Color1 = reader.ReadColor();
            //Color2 = reader.ReadColor();
            Reputation = reader.ReadInt16();
            Level = reader.ReadByte();
            MainComp = reader.ReadInt16();

            Qualifiers = new byte[reader.ReadInt32()][];
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                Qualifiers[i] = reader.ReadBytes(8);
            }

            Rank1 = reader.ReadInt32();
            Rank2 = reader.ReadInt32();
            Rank3 = reader.ReadInt32();
            Year1 = reader.ReadInt16();
            Year2 = reader.ReadInt16();
            Year3 = reader.ReadInt16();

            Unknown3 = reader.ReadByte();
        }

        public byte[] ToBytes()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(Id));
            bytes.AddRange(BitConverter.GetBytes(Uid));

            bytes.AddRange(FullName.GetBytes());
            bytes.Add(Unknown1);
            bytes.AddRange(ShortName.GetBytes());
            bytes.Add(Unknown2);
            bytes.AddRange(CodeName.GetBytes());

            bytes.Add(Type);
            bytes.AddRange(BitConverter.GetBytes(Continent));
            bytes.AddRange(BitConverter.GetBytes(Nation));
            bytes.AddRange(Colors);
            bytes.AddRange(BitConverter.GetBytes(Reputation));
            bytes.Add(Level);
            bytes.AddRange(BitConverter.GetBytes(MainComp));

            bytes.AddRange(BitConverter.GetBytes(Qualifiers.Length));
            for (int i = 0; i < Qualifiers.Length; i++)
            {
                bytes.AddRange(Qualifiers[i]);
            }

            bytes.AddRange(BitConverter.GetBytes(Rank1));
            bytes.AddRange(BitConverter.GetBytes(Rank2));
            bytes.AddRange(BitConverter.GetBytes(Rank3));
            bytes.AddRange(BitConverter.GetBytes(Year1));
            bytes.AddRange(BitConverter.GetBytes(Year2));
            bytes.AddRange(BitConverter.GetBytes(Year3));
            bytes.Add(Unknown3);

            return bytes.ToArray();
        }

        public override string ToString()
        {
            return $"{Id} - {Uid} - {FullName}";
        }
    }
}
