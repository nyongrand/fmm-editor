namespace FMELibrary
{
    public class StringResource
    {
        public class ClubName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadStringEx();
            public byte FullNameTerminator { get; set; } = reader.ReadByte();
            public string ShortName { get; set; } = reader.ReadStringEx();
            public byte ShortNameTerminator { get; set; } = reader.ReadByte();
            public string SixLetterName { get; set; } = reader.ReadStringEx();
            public string ThreeLetterName { get; set; } = reader.ReadStringEx();

            public override string ToString() => FullName;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(FullName);
                writer.WriteEx(FullNameTerminator);
                writer.WriteEx(ShortName);
                writer.WriteEx(ShortNameTerminator);
                writer.WriteEx(SixLetterName);
                writer.WriteEx(ThreeLetterName);
            }
        }

        public class NationName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string Name { get; set; } = reader.ReadStringEx();
            public byte NameTerminator { get; set; } = reader.ReadByte();
            public string Demonym { get; set; } = reader.ReadStringEx();
            public byte DemonymTerminator { get; set; } = reader.ReadByte();
            public string Code { get; set; } = reader.ReadStringEx();
            public byte Unknown { get; set; } = reader.ReadByte();

            public override string ToString() => Name;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(Name);
                writer.WriteEx(NameTerminator);
                writer.WriteEx(Demonym);
                writer.WriteEx(DemonymTerminator);
                writer.WriteEx(Code);
                writer.WriteEx(Unknown);
            }
        }

        public class ContinentName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string Name { get; set; } = reader.ReadStringEx();
            public byte NameTerminator { get; set; } = reader.ReadByte();
            public string CodeName { get; set; } = reader.ReadStringEx();
            public string Demonym { get; set; } = reader.ReadStringEx();
            public byte DemonymTerminator { get; set; } = reader.ReadByte();

            public override string ToString() => Name;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(Name);
                writer.WriteEx(NameTerminator);
                writer.WriteEx(CodeName);
                writer.WriteEx(Demonym);
                writer.WriteEx(DemonymTerminator);
            }
        }

        public class CompetitionName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadStringEx();
            public byte FullNameTerminator { get; set; } = reader.ReadByte();
            public string ShortName { get; set; } = reader.ReadStringEx();
            public byte ShortNameTerminator { get; set; } = reader.ReadByte();
            public string CodeName { get; set; } = reader.ReadStringEx();

            public override string ToString() => FullName;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(FullName);
                writer.WriteEx(FullNameTerminator);
                writer.WriteEx(ShortName);
                writer.WriteEx(ShortNameTerminator);
                writer.WriteEx(CodeName);
            }
        }

        public class StadiumName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadStringEx();
            public string ShortName { get; set; } = reader.ReadStringEx();
            public byte Terminator { get; set; } = reader.ReadByte();
            public byte Unknown { get; set; } = reader.ReadByte();

            public override string ToString() => FullName;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(FullName);
                writer.WriteEx(ShortName);
                writer.WriteEx(Terminator);
                writer.WriteEx(Unknown);
            }
        }

        public class AwardName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadStringEx();
            public byte FullNameTerminator { get; set; } = reader.ReadByte();
            public string ShortName { get; set; } = reader.ReadStringEx();
            public byte ShortNameTerminator { get; set; } = reader.ReadByte();

            public override string ToString() => FullName;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(FullName);
                writer.WriteEx(FullNameTerminator);
                writer.WriteEx(ShortName);
                writer.WriteEx(ShortNameTerminator);
            }
        }

        public class OtherName(BinaryReader reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string Name { get; set; } = reader.ReadStringEx();
            public byte Terminator { get; set; } = reader.ReadByte();

            public override string ToString() => Name;

            public void Write(BinaryWriter writer)
            {
                writer.WriteEx(Uid);
                writer.WriteEx(Name);
                writer.WriteEx(Terminator);
            }
        }
    }
}
