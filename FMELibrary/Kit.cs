namespace FMELibrary
{
    public class Kit
    {
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }
        public short Color0 { get; set; }
        public short Color1 { get; set; }
        public short Color2 { get; set; }
        public short Color3 { get; set; }
        public short Color4 { get; set; }
        public short Color5 { get; set; }
        public short Color6 { get; set; }
        public short Color7 { get; set; }
        public short Color8 { get; set; }
        public short Color9 { get; set; }

        public Kit(BinaryReader reader)
        {
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            Color0 = reader.ReadInt16();
            Color1 = reader.ReadInt16();
            Color2 = reader.ReadInt16();
            Color3 = reader.ReadInt16();
            Color4 = reader.ReadInt16();
            Color5 = reader.ReadInt16();
            Color6 = reader.ReadInt16();
            Color7 = reader.ReadInt16();
            Color8 = reader.ReadInt16();
            Color9 = reader.ReadInt16();
        }
    }
}
