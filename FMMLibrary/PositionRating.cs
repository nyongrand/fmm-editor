namespace FMMLibrary
{
    public class PositionRating
    {
        public byte GK { get; set; }
        public byte LIB { get; set; }
        public byte DL { get; set; }
        public byte DC { get; set; }
        public byte DR { get; set; }
        public byte DMC { get; set; }
        public byte LM { get; set; }
        public byte CM { get; set; }
        public byte RM { get; set; }
        public byte LW { get; set; }
        public byte AM { get; set; }
        public byte RW { get; set; }
        public byte CF { get; set; }
        public byte LWB { get; set; }
        public byte RWB { get; set; }

        public PositionRating()
        {
            GK = 0;
            LIB = 0;
            DL = 0;
            DC = 0;
            DR = 0;
            DMC = 0;
            LM = 0;
            CM = 0;
            RM = 0;
            LW = 0;
            AM = 0;
            RW = 0;
            CF = 0;
            LWB = 0;
            RWB = 0;
        }

        public PositionRating(BinaryReader reader)
        {
            GK = reader.ReadByte();
            LIB = reader.ReadByte();
            DL = reader.ReadByte();
            DC = reader.ReadByte();
            DR = reader.ReadByte();
            DMC = reader.ReadByte();
            LM = reader.ReadByte();
            CM = reader.ReadByte();
            RM = reader.ReadByte();
            LW = reader.ReadByte();
            AM = reader.ReadByte();
            RW = reader.ReadByte();
            CF = reader.ReadByte();
            LWB = reader.ReadByte();
            RWB = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(GK);
            writer.WriteEx(LIB);

            writer.WriteEx(DL);
            writer.WriteEx(DC);
            writer.WriteEx(DR);

            writer.WriteEx(DMC);

            writer.WriteEx(LM);
            writer.WriteEx(CM);
            writer.WriteEx(RM);

            writer.WriteEx(LW);
            writer.WriteEx(AM);
            writer.WriteEx(RW);

            writer.WriteEx(CF);

            writer.WriteEx(LWB);
            writer.WriteEx(RWB);
        }

        public override string ToString()
        {
            var position = "";
            if (GK >= 18)
                position += "GK";

            if (DL >= 18 || DC >= 18 || DR >= 18)
            {
                position += "D";
                if (DL >= 18)
                    position += "L";
                if (DR >= 18)
                    position += "R";
                if (DC >= 18)
                    position += "C";
            }

            if (DL >= 18 || DC >= 18 || DR >= 18)
            {
                position += "D";
                if (DL >= 18)
                    position += "L";
                if (DR >= 18)
                    position += "R";
                if (DC >= 18)
                    position += "C";
            }

            if (DL >= 18 || DC >= 18 || DR >= 18)
            {
                position += "D";
                if (DL >= 18)
                    position += "L";
                if (DR >= 18)
                    position += "R";
                if (DC >= 18)
                    position += "C";
            }

            return position;
        }
    }
}
