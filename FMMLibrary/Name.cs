namespace FMMLibrary
{
    public class Name
    {
        /// <summary>
        /// Always 0x00000000
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
        /// Country of origin
        /// </summary>
        public int NationUid { get; set; }
        
        /// <summary>
        /// Display name of the nation (not persisted)
        /// </summary>
        public string? NationName { get; set; }

        public short Unknown2 { get; set; }

        /// <summary>
        /// Possible values:<br/>
        /// 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 255<br/>
        /// </summary>
        public byte Unknown3 { get; set; }

        public string Value { get; set; }

        public Name(BinaryReader reader)
        {
            Unknown1 = reader.ReadInt32();
            Id = reader.ReadInt32();
            Gender = reader.ReadByte();
            NationUid = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadByte();
            Value = reader.ReadStringEx();
        }

        public Name(byte gender, int nationUid, short unknown2, byte unknown3, string value)
        {
            Unknown1 = 0;
            Id = -1;
            Gender = gender;
            NationUid = nationUid;
            Unknown2 = unknown2;
            Unknown3 = unknown3;
            Value = value;
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Unknown1);
            writer.WriteEx(Id);
            writer.WriteEx(Gender);
            writer.WriteEx(NationUid);
            writer.WriteEx(Unknown2);
            writer.WriteEx(Unknown3);
            writer.WriteEx(Value);
        }

        public override string ToString() => Value;
    }
}
