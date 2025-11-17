namespace FMELibrary
{
    public class Name
    {
        /// <summary>
        /// Alwat 0x00000000
        /// </summary>
        public int Unknown1 { get; set; }

        public int Id { get; set; }

        /// <summary>
        /// Indicates the gender associated with the first name.<br/>
        /// Possible values:<br/>
        /// 0 - Male<br/>
        /// 1 - Female<br/>
        /// </summary>
        public byte Gender { get; set; }

        /// <summary>
        /// Probably Country of origin
        /// </summary>
        public int Unknown3 { get; set; }

        public short Unknown4 { get; set; }

        /// <summary>
        /// Possible values:<br/>
        /// 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 255<br/>
        /// </summary>
        public byte Unknown5 { get; set; }

        public string Value { get; set; }

        public Name(BinaryReader reader)
        {
            Unknown1 = reader.ReadInt32();
            Id = reader.ReadInt32();
            Gender = reader.ReadByte();
            Unknown3 = reader.ReadInt32();
            Unknown4 = reader.ReadInt16();
            Unknown5 = reader.ReadByte();
            Value = reader.ReadStringEx();
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Unknown1);
            writer.WriteEx(Id);
            writer.WriteEx(Gender);
            writer.WriteEx(Unknown3);
            writer.WriteEx(Unknown4);
            writer.WriteEx(Unknown5);
            writer.WriteEx(Value);
        }

        public override string ToString() => Value;
    }
}
