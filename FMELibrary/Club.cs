namespace FMELibrary
{
    /// <summary>
    /// Represents a football club with all its properties and attributes.
    /// </summary>
    public class Club
    {
        /// <summary>
        /// Gets or sets the club identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the club.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Gets or sets the full name of the club.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown0 { get; set; }

        /// <summary>
        /// Gets or sets the short name of the club.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// Gets or sets the first code name of the club.
        /// </summary>
        public string CodeName1 { get; set; }

        /// <summary>
        /// Gets or sets the second code name of the club.
        /// </summary>
        public string CodeName2 { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the city where the club is based.
        /// </summary>
        public short BasedId { get; set; }

        /// <summary>
        /// Gets or sets the nation identifier of the club.
        /// </summary>
        public short NationId { get; set; }

        /// <summary>
        /// Gets or sets the club's color values (6 colors).
        /// </summary>
        public short[] Colors { get; set; }

        /// <summary>
        /// Gets or sets the club's kits (6 kits).
        /// </summary>
        public Kit[] Kits { get; set; }

        /// <summary>
        /// Gets or sets the club's status.
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Gets or sets the academy rating.
        /// </summary>
        public byte Academy { get; set; }

        /// <summary>
        /// Gets or sets the facilities rating.
        /// </summary>
        public byte Facilities { get; set; }

        /// <summary>
        /// Gets or sets the average attendance.
        /// </summary>
        public short AttAvg { get; set; }

        /// <summary>
        /// Gets or sets the minimum attendance.
        /// </summary>
        public short AttMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum attendance.
        /// </summary>
        public short AttMax { get; set; }

        /// <summary>
        /// Gets or sets the reserves status.
        /// </summary>
        public byte Reserves { get; set; }

        /// <summary>
        /// Gets or sets the league identifier.
        /// </summary>
        public short LeagueId { get; set; }

        /// <summary>
        /// Gets or sets an unknown short value.
        /// </summary>
        public short Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown3 { get; set; }

        /// <summary>
        /// Gets or sets the stadium identifier.
        /// </summary>
        public short Stadium { get; set; }

        /// <summary>
        /// Gets or sets the last league identifier.
        /// </summary>
        public short LastLeague { get; set; }

        /// <summary>
        /// Gets or sets the type flag for Unknown4 data.
        /// </summary>
        public bool Unknown4Flag { get; set; }

        /// <summary>
        /// Gets or sets conditional unknown data (68 bytes if Unknown4Type is 1).
        /// </summary>
        public byte[] Unknown4 { get; set; }

        /// <summary>
        /// Gets or sets variable-length unknown data.
        /// </summary>
        public byte[] Unknown5 { get; set; }

        /// <summary>
        /// Gets or sets the current league position.
        /// </summary>
        public byte LeaguePos { get; set; }

        /// <summary>
        /// Gets or sets the club's reputation.
        /// </summary>
        public short Reputation { get; set; }

        /// <summary>
        /// Gets or sets unknown data (20 bytes).
        /// </summary>
        public byte[] Unknown6 { get; set; }

        /// <summary>
        /// Gets or sets the array of affiliated clubs.
        /// </summary>
        public Affiliate[] Affiliates { get; set; }

        /// <summary>
        /// Gets or sets the array of player identifiers.
        /// </summary>
        public int[] Players { get; set; }

        /// <summary>
        /// Gets or sets unknown data (11 integers).
        /// </summary>
        public int[] Unknown7 { get; set; }

        /// <summary>
        /// Gets or sets the main club identifier (for reserve/B teams).
        /// </summary>
        public int MainClub { get; set; }

        /// <summary>
        /// Gets or sets whether this is a national team.
        /// </summary>
        public short IsNational { get; set; }

        /// <summary>
        /// Gets or sets unknown data (33 bytes).
        /// </summary>
        public byte[] Unknown8 { get; set; }

        /// <summary>
        /// Gets or sets unknown data (40 bytes).
        /// </summary>
        public byte[] Unknown9 { get; set; }

        /// <summary>
        /// Gets or sets unknown data (2 bytes).
        /// </summary>
        public short IsWomanFlag { get; set; }

        #region Extra

        public string Competition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the city where the club is based (extra/computed field).
        /// </summary>
        public string Based { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the nation name (extra/computed field).
        /// </summary>
        public string Nation { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Club"/> class by reading from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader containing the club data.</param>
        public Club(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            FullName = reader.ReadStringEx();
            Unknown0 = reader.ReadByte();
            ShortName = reader.ReadStringEx();
            Unknown1 = reader.ReadByte();
            CodeName1 = reader.ReadStringEx();
            CodeName2 = reader.ReadStringEx();

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

            Unknown4Flag = reader.ReadBoolean();
            if (Unknown4Flag)
                Unknown4 = reader.ReadBytes(68);
            else
                Unknown4 = [];

            //            Unknown5 = new byte[reader.ReadInt32()];
            //            Unknown5 = reader.ReadBytes(Unknown5.Length);

            Unknown5 = reader.ReadBytes(reader.ReadInt32());

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
            IsWomanFlag = reader.ReadInt16();
        }

        /// <summary>
        /// Converts the club data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized club data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the club data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the club data to.</param>
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

            writer.Write(Unknown4Flag);
            if (Unknown4Flag)
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
            writer.Write(IsWomanFlag);
        }

        /// <summary>
        /// Returns a string representation of the club.
        /// </summary>
        /// <returns>A string containing the club's UID and full name.</returns>
        public override string ToString()
        {
            return $"{FullName}";
        }
    }
}
