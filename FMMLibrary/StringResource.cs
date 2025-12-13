namespace FMMLibrary
{
    public class StringResource
    {
        public class ClubName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadString();
            public byte FullNameTerminator { get; set; } = reader.ReadByte();
            public string ShortName { get; set; } = reader.ReadString();
            public byte ShortNameTerminator { get; set; } = reader.ReadByte();
            public string SixLetterName { get; set; } = reader.ReadString();
            public string ThreeLetterName { get; set; } = reader.ReadString();

            public override string ToString() => FullName;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(FullName);
                writer.Write(FullNameTerminator);
                writer.Write(ShortName);
                writer.Write(ShortNameTerminator);
                writer.Write(SixLetterName);
                writer.Write(ThreeLetterName);
            }
        }

        public class NationName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string Name { get; set; } = reader.ReadString();
            public byte NameTerminator { get; set; } = reader.ReadByte();
            public string Demonym { get; set; } = reader.ReadString();
            public byte DemonymTerminator { get; set; } = reader.ReadByte();
            public string Code { get; set; } = reader.ReadString();
            public byte Unknown { get; set; } = reader.ReadByte();

            public override string ToString() => Name;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(Name);
                writer.Write(NameTerminator);
                writer.Write(Demonym);
                writer.Write(DemonymTerminator);
                writer.Write(Code);
                writer.Write(Unknown);
            }
        }

        public class ContinentName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string Name { get; set; } = reader.ReadString();
            public byte NameTerminator { get; set; } = reader.ReadByte();
            public string CodeName { get; set; } = reader.ReadString();
            public string Demonym { get; set; } = reader.ReadString();
            public byte DemonymTerminator { get; set; } = reader.ReadByte();

            public override string ToString() => Name;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(Name);
                writer.Write(NameTerminator);
                writer.Write(CodeName);
                writer.Write(Demonym);
                writer.Write(DemonymTerminator);
            }
        }

        public class CompetitionName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadString();
            public byte FullNameTerminator { get; set; } = reader.ReadByte();
            public string ShortName { get; set; } = reader.ReadString();
            public byte ShortNameTerminator { get; set; } = reader.ReadByte();
            public string CodeName { get; set; } = reader.ReadString();

            public override string ToString() => FullName;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(FullName);
                writer.Write(FullNameTerminator);
                writer.Write(ShortName);
                writer.Write(ShortNameTerminator);
                writer.Write(CodeName);
            }
        }

        public class StadiumName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadString();
            public string ShortName { get; set; } = reader.ReadString();
            public byte Terminator { get; set; } = reader.ReadByte();
            public byte Unknown { get; set; } = reader.ReadByte();

            public override string ToString() => FullName;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(FullName);
                writer.Write(ShortName);
                writer.Write(Terminator);
                writer.Write(Unknown);
            }
        }

        public class AwardName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string FullName { get; set; } = reader.ReadString();
            public byte FullNameTerminator { get; set; } = reader.ReadByte();
            public string ShortName { get; set; } = reader.ReadString();
            public byte ShortNameTerminator { get; set; } = reader.ReadByte();

            public override string ToString() => FullName;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(FullName);
                writer.Write(FullNameTerminator);
                writer.Write(ShortName);
                writer.Write(ShortNameTerminator);
            }
        }

        public class OtherName(BinaryReaderEx reader)
        {
            public int Uid { get; set; } = reader.ReadInt32();
            public string Name { get; set; } = reader.ReadString();
            public byte Terminator { get; set; } = reader.ReadByte();

            public override string ToString() => Name;

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Uid);
                writer.Write(Name);
                writer.Write(Terminator);
            }
        }
    }
}
