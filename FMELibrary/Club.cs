namespace FMELibrary
{
    public class Club
    {
        public int Id { get; set; }
        public int Uid { get; set; }
        public string FullName { get; set; }
        public byte Unknown0 { get; set; }
        public string ShortName { get; set; }
        public byte Unknown1 { get; set; }
        public string CodeName1 { get; set; }
        public string CodeName2 { get; set; }
        public short BasedId { get; set; }
        public short NationId { get; set; }
        public short[] Colors { get; set; }
        public Kit[] Kits { get; set; }
        public byte Status { get; set; }
        public byte Academy { get; set; }
        public byte Facilities { get; set; }
        public short AttAvg { get; set; }
        public short AttMin { get; set; }
        public short AttMax { get; set; }
        public byte Reserves { get; set; }
        public short LeagueId { get; set; }
        public short Unknown2 { get; set; }
        public byte Unknown3 { get; set; }
        public short Stadium { get; set; }
        public short LastLeague { get; set; }
        public byte Unknown4Type { get; set; }
        public byte[] Unknown4 { get; set; }
        public byte[] Unknown5 { get; set; }
        public byte LeaguePos { get; set; }
        public short Reputation { get; set; }
        public byte[] Unknown6 { get; set; }
        public Affiliate[] Affiliates { get; set; }
        public int[] Players { get; set; }
        public int[] Unknown7 { get; set; }
        public int MainClub { get; set; }
        public short IsNational { get; set; }
        public byte[] Unknown8 { get; set; }
        public byte[] Unknown9 { get; set; }
        public byte[] Unknown10 { get; set; }

        #region Extra

        public string Based { get; set; } = string.Empty;
        public string Nation { get; set; } = string.Empty;

        #endregion

        public Club(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            FullName = reader.ReadString(reader.ReadInt32());
            Unknown0 = reader.ReadByte();
            ShortName = reader.ReadString(reader.ReadInt32());
            Unknown1 = reader.ReadByte();
            CodeName1 = reader.ReadString(reader.ReadInt32());
            CodeName2 = reader.ReadString(reader.ReadInt32());

            BasedId = reader.ReadInt16();
            NationId = reader.ReadInt16();

            Colors = new short[6];
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = reader.ReadInt16();
            }

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
            LeagueId = reader.ReadInt16();

            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadByte();
            Stadium = reader.ReadInt16();
            LastLeague = reader.ReadInt16();

            Unknown4Type = reader.ReadByte();
            if (Unknown4Type == 1)
                Unknown4 = reader.ReadBytes(68);
            else
                Unknown4 = [];

            Unknown5 = new byte[reader.ReadInt32()];
            Unknown5 = reader.ReadBytes(Unknown5.Length);

            LeaguePos = reader.ReadByte();
            Reputation = reader.ReadInt16();
            Unknown6 = reader.ReadBytes(20);

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

            Unknown7 = new int[11];
            for (int i = 0; i < Unknown7.Length; i++)
            {
                Unknown7[i] = reader.ReadInt32();
            }

            MainClub = reader.ReadInt32();
            IsNational = reader.ReadInt16();

            Unknown8 = reader.ReadBytes(33);
            Unknown9 = reader.ReadBytes(40);
            Unknown10 = reader.ReadBytes(2);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Uid);

            writer.WriteEx(FullName);
            writer.Write(Unknown0);
            writer.WriteEx(ShortName);
            writer.Write(Unknown1);
            writer.WriteEx(CodeName1);
            writer.WriteEx(CodeName2);

            writer.Write(BasedId);
            writer.Write(NationId);

            for (int i = 0; i < Colors.Length; i++)
                writer.Write(Colors[i]);

            for (int i = 0; i < Kits.Length; i++)
                Kits[i].Write(writer);

            writer.Write(Status);
            writer.Write(Academy);
            writer.Write(Facilities);
            writer.Write(AttAvg);
            writer.Write(AttMin);
            writer.Write(AttMax);
            writer.Write(Reserves);
            writer.Write(LeagueId);

            writer.Write(Unknown2);
            writer.Write(Unknown3);
            writer.Write(Stadium);
            writer.Write(LastLeague);

            writer.Write(Unknown4Type);
            if (Unknown4Type == 1)
                writer.Write(Unknown4);

            writer.Write(Unknown5.Length);
            writer.Write(Unknown5);

            writer.Write(LeaguePos);
            writer.Write(Reputation);
            writer.Write(Unknown6);

            writer.Write((short)Affiliates.Length);
            for (int i = 0; i < Affiliates.Length; i++)
                Affiliates[i].Write(writer);

            writer.Write((short)Players.Length);
            for (int i = 0; i < Players.Length; i++)
                writer.Write(Players[i]);

            for (int i = 0; i < Unknown7.Length; i++)
                writer.Write(Unknown7[i]);

            writer.Write(MainClub);
            writer.Write(IsNational);

            writer.Write(Unknown8);
            writer.Write(Unknown9);
            writer.Write(Unknown10);
        }

        public override string ToString()
        {
            return $"{Uid} {FullName}";
        }
    }
}
