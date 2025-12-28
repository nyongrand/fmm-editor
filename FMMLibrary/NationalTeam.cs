namespace FMMLibrary
{
    public class NationalTeam
    {
        /// <summary>
        /// Probably deprecated, all Nations have the value 0xFF7F
        /// </summary>
        public short Color1 { get; set; }

        /// <summary>
        /// Probably deprecated, all Nations have the value 0xD400
        /// </summary>
        public short Color2 { get; set; }

        /// <summary>
        /// Probably deprecated, all Nations have the valuee 0x0000
        /// </summary>
        public short Color3 { get; set; }

        /// <summary>
        /// Probably deprecated, all Nations have the value 0xFF7F
        /// </summary>
        public short Color4 { get; set; }

        /// <summary>
        /// Importance rating used by scheduling/AI: 0 = Not set, 1 = Very important,
        /// 2 = Important, 3 = Fairly unimportant, 4 = Unimportant.
        /// </summary>
        public byte GameImportance { get; set; }

        /// <summary>
        /// Rival nation identifier.
        /// </summary>
        public short RivalId { get; set; }

        /// <summary>
        /// Unknown flag byte.
        /// </summary>
        public byte Unknown4 { get; set; }

        /// <summary>
        /// Whether the team has an official ranking.
        /// </summary>
        public bool IsRanked { get; set; }

        /// <summary>
        /// FIFA/UEFA ranking position.
        /// </summary>
        public short Ranking { get; set; }

        /// <summary>
        /// Ranking points value.
        /// </summary>
        public short Points { get; set; }

        /// <summary>
        /// Always 0x0000 in observed files (reserved padding).
        /// </summary>
        public short Unknown5 { get; set; }

        /// <summary>
        /// Coefficient values for European competition seeding, stored per season
        /// from 2014/15 through 2024/25 (count stored as a single byte in file).
        /// </summary>
        public float[] Coefficients { get; set; } = [];

        /// <summary>
        /// Unknown data block (11 bytes).
        /// </summary>
        public byte[] Unknown6 { get; set; } = [];

        /// <summary>
        /// Reads a national team block from the provided binary reader.
        /// </summary>
        /// <param name="reader">Binary reader positioned at the start of the team block.</param>
        public NationalTeam(BinaryReaderEx reader)
        {
            Color1 = reader.ReadInt16();
            Color2 = reader.ReadInt16();
            Color3 = reader.ReadInt16();
            Color4 = reader.ReadInt16();

            GameImportance = reader.ReadByte();
            RivalId = reader.ReadInt16();
            Unknown4 = reader.ReadByte();

            IsRanked = reader.ReadBoolean();
            Ranking = reader.ReadInt16();
            Points = reader.ReadInt16();

            Unknown5 = reader.ReadInt16();

            Coefficients = new float[reader.ReadByte()];
            for (int i = 0; i < Coefficients.Length; i++)
                Coefficients[i] = reader.ReadSingle();

            Unknown6 = reader.ReadBytes(11);
        }

        /// <summary>
        /// Writes this national team block to the provided binary writer.
        /// </summary>
        /// <param name="writer">Binary writer positioned where the block should be emitted.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Color1);
            writer.Write(Color2);
            writer.Write(Color3);
            writer.Write(Color4);

            writer.Write(GameImportance);
            writer.Write(RivalId);
            writer.Write(Unknown4);

            writer.Write(IsRanked);
            writer.Write(Ranking);
            writer.Write(Points);

            writer.Write(Unknown5);

            writer.Write((byte)Coefficients.Length);
            for (int i = 0; i < Coefficients.Length; i++)
                writer.Write(Coefficients[i]);

            writer.Write(Unknown6);
        }
    }
}
