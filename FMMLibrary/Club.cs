using System.Drawing;

namespace FMMLibrary
{
    /// <summary>
    /// Represents a football club with all its properties and attributes.
    /// </summary>
    public class Club
    {
        /// <summary>
        /// Gets or sets the club index.
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
        /// FullName terminator
        /// </summary>
        public byte FullNameTerminator { get; set; }

        /// <summary>
        /// Gets or sets the short name of the club.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// ShortName terminator
        /// </summary>
        public byte ShortNameTerminator { get; set; }

        /// <summary>
        /// Gets or sets the first code name of the club.
        /// </summary>
        public string SixLetterName { get; set; }

        /// <summary>
        /// Gets or sets the second code name of the club.
        /// </summary>
        public string ThreeLetterName { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the nation where the club is based.
        /// </summary>
        public short BasedId { get; set; }

        /// <summary>
        /// Gets or sets the nation identifier of the club.
        /// </summary>
        public short NationId { get; set; }

        /// <summary>
        /// Gets or sets the club's color values (6 colors).
        /// </summary>
        public Color[] Colors { get; set; }

        /// <summary>
        /// Gets or sets the club's kits (6 kits).
        /// </summary>
        public Kit[] Kits { get; set; }

        /// <summary>
        /// Gets or sets the club's status.
        /// 0 - National
        /// 1 - Professional
        /// 2 - Semi Pro
        /// 3 - Amateur
        /// 22 - Unknown
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
        /// Gets or sets the reserves team count.
        /// </summary>
        public byte Reserves { get; set; }

        /// <summary>
        /// Gets or sets the league identifier.
        /// </summary>
        public short LeagueId { get; set; }

        /// <summary>
        /// Always 0xFFFF
        /// </summary>
        public short OtherDivision { get; set; }

        /// <summary>
        /// Mostly 0x00, with 1 team being 0x0A
        /// </summary>
        public byte OtherLastPosition { get; set; }

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
            FullNameTerminator = reader.ReadByte();
            ShortName = reader.ReadStringEx();
            ShortNameTerminator = reader.ReadByte();
            SixLetterName = reader.ReadStringEx();
            ThreeLetterName = reader.ReadStringEx();

            BasedId = reader.ReadInt16();
            NationId = reader.ReadInt16();

            Colors = new Color[6];
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = reader.ReadColor();
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

            OtherDivision = reader.ReadInt16();
            OtherLastPosition = reader.ReadByte();
            Stadium = reader.ReadInt16();
            LastLeague = reader.ReadInt16();

            Unknown4Flag = reader.ReadBoolean();
            if (Unknown4Flag)
                Unknown4 = reader.ReadBytes(68);
            else
                Unknown4 = [];

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
            writer.WriteEx(Id);
            writer.WriteEx(Uid);

            writer.WriteEx(FullName);
            writer.WriteEx(FullNameTerminator);
            writer.WriteEx(ShortName);
            writer.WriteEx(ShortNameTerminator);
            writer.WriteEx(SixLetterName);
            writer.WriteEx(ThreeLetterName);

            writer.WriteEx(BasedId);
            writer.WriteEx(NationId);

            for (int i = 0; i < Colors.Length; i++)
                writer.WriteEx(Colors[i]);

            for (int i = 0; i < Kits.Length; i++)
                Kits[i].Write(writer);

            writer.WriteEx(Status);
            writer.WriteEx(Academy);
            writer.WriteEx(Facilities);
            writer.WriteEx(AttAvg);
            writer.WriteEx(AttMin);
            writer.WriteEx(AttMax);
            writer.WriteEx(Reserves);
            writer.WriteEx(LeagueId);

            writer.WriteEx(OtherDivision);
            writer.WriteEx(OtherLastPosition);
            writer.WriteEx(Stadium);
            writer.WriteEx(LastLeague);

            writer.WriteEx(Unknown4Flag);
            if (Unknown4Flag)
                writer.WriteEx(Unknown4);

            writer.WriteEx(Unknown5.Length);
            writer.WriteEx(Unknown5);

            writer.WriteEx(LeaguePos);
            writer.WriteEx(Reputation);
            writer.WriteEx(Unknown6);

            writer.WriteEx((short)Affiliates.Length);
            for (int i = 0; i < Affiliates.Length; i++)
                Affiliates[i].Write(writer);

            writer.WriteEx((short)Players.Length);
            for (int i = 0; i < Players.Length; i++)
                writer.WriteEx(Players[i]);

            for (int i = 0; i < Unknown7.Length; i++)
                writer.WriteEx(Unknown7[i]);

            writer.WriteEx(MainClub);
            writer.WriteEx(IsNational);

            writer.WriteEx(Unknown8);
            writer.WriteEx(Unknown9);
            writer.WriteEx(IsWomanFlag);
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
