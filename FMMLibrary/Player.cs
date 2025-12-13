namespace FMMLibrary
{
    public class Player
    {
        public int Id { get; set; }
        public int Uid { get; set; }

        public byte Crossing { get; set; }
        public byte Dribbling { get; set; }
        public byte Tackling { get; set; }
        public byte Finishing { get; set; }
        public byte LongShot { get; set; }
        public byte Heading { get; set; }
        public byte Jumping { get; set; }
        public byte Passing { get; set; }
        public byte Decision { get; set; }
        public byte Unselfishness { get; set; }
        public byte Pace { get; set; }
        public byte Strength { get; set; }
        public byte Stamina { get; set; }
        public byte Technique { get; set; }
        public byte Consistency { get; set; }
        public byte Aggression { get; set; }
        public byte BigMatch { get; set; }
        public byte InjuryProne { get; set; }
        public byte Leadership { get; set; }
        public byte Versatility { get; set; }
        public byte SetPieces { get; set; }
        public byte Penalty { get; set; }
        public byte Creativity { get; set; }
        public byte Movement { get; set; }
        public byte Positioning { get; set; }
        public byte WorkRate { get; set; }
        public byte Flair { get; set; }

        public byte Handling { get; set; }
        public byte Kicking { get; set; }
        public byte Agility { get; set; }
        public byte Aerial { get; set; }
        public byte Reflexes { get; set; }
        public byte Communication { get; set; }
        public byte Throwing { get; set; }

        public byte GK { get; set; }
        public byte LIB { get; set; }
        public byte LB { get; set; }
        public byte CB { get; set; }
        public byte RB { get; set; }
        public byte DM { get; set; }
        public byte LM { get; set; }
        public byte CM { get; set; }
        public byte RM { get; set; }
        public byte LW { get; set; }
        public byte AM { get; set; }
        public byte RW { get; set; }
        public byte CF { get; set; }
        public byte LWB { get; set; }
        public byte RWB { get; set; }

        public byte LeftFoot { get; set; }
        public byte RightFoot { get; set; }
        public short CA { get; set; }
        public short PA { get; set; }

        public short HomeReputation { get; set; }
        public short CurrentReputation { get; set; }
        public short WorldReputation { get; set; }

        public byte InternationalRetirement { get; set; }

        /// <summary>
        /// Always 0x0000
        /// </summary>
        public short Unknown1 { get; set; }
        public byte SquadNumber { get; set; }
        public byte PreferredSquadNumber { get; set; }

        public short Height { get; set; }
        public short Weight { get; set; }

        /// <summary>
        /// Always 0x00000000
        /// </summary>
        public int Unknown2 { get; set; }

        /// <summary>
        /// Parameterless constructor for creating new players.
        /// </summary>
        public Player()
        {
        }

        public Player(BinaryReaderEx reader)
        {
            Id = reader.ReadInt32();
            Uid = reader.ReadInt32();

            Crossing = reader.ReadByte();
            Dribbling = reader.ReadByte();
            Tackling = reader.ReadByte();
            Finishing = reader.ReadByte();
            LongShot = reader.ReadByte();
            Heading = reader.ReadByte();
            Jumping = reader.ReadByte();
            Passing = reader.ReadByte();
            Decision = reader.ReadByte();
            Unselfishness = reader.ReadByte();
            Pace = reader.ReadByte();
            Strength = reader.ReadByte();
            Stamina = reader.ReadByte();
            Technique = reader.ReadByte();
            Consistency = reader.ReadByte();
            Aggression = reader.ReadByte();
            BigMatch = reader.ReadByte();
            InjuryProne = reader.ReadByte();
            Leadership = reader.ReadByte();
            Versatility = reader.ReadByte();
            SetPieces = reader.ReadByte();
            Penalty = reader.ReadByte();
            Creativity = reader.ReadByte();
            Movement = reader.ReadByte();
            Positioning = reader.ReadByte();
            WorkRate = reader.ReadByte();
            Flair = reader.ReadByte();

            Handling = reader.ReadByte();
            Kicking = reader.ReadByte();
            Agility = reader.ReadByte();
            Aerial = reader.ReadByte();
            Reflexes = reader.ReadByte();
            Communication = reader.ReadByte();
            Throwing = reader.ReadByte();

            GK = reader.ReadByte();
            LIB = reader.ReadByte();
            LB = reader.ReadByte();
            CB = reader.ReadByte();
            RB = reader.ReadByte();
            DM = reader.ReadByte();
            LM = reader.ReadByte();
            CM = reader.ReadByte();
            RM = reader.ReadByte();
            LW = reader.ReadByte();
            AM = reader.ReadByte();
            RW = reader.ReadByte();
            CF = reader.ReadByte();
            LWB = reader.ReadByte();
            RWB = reader.ReadByte();

            LeftFoot = reader.ReadByte();
            RightFoot = reader.ReadByte();

            CA = reader.ReadInt16();
            PA = reader.ReadInt16();

            HomeReputation = reader.ReadInt16();
            CurrentReputation = reader.ReadInt16();
            WorldReputation = reader.ReadInt16();
            InternationalRetirement = reader.ReadByte();
            Unknown1 = reader.ReadInt16();
            SquadNumber = reader.ReadByte();
            PreferredSquadNumber = reader.ReadByte();
            Height = reader.ReadInt16();
            Weight = reader.ReadInt16();
            Unknown2 = reader.ReadInt32();
        }

        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Id);
            writer.Write(Uid);
            writer.Write(Crossing);
            writer.Write(Dribbling);
            writer.Write(Tackling);
            writer.Write(Finishing);
            writer.Write(LongShot);
            writer.Write(Heading);
            writer.Write(Jumping);
            writer.Write(Passing);
            writer.Write(Decision);
            writer.Write(Unselfishness);
            writer.Write(Pace);
            writer.Write(Strength);
            writer.Write(Stamina);
            writer.Write(Technique);
            writer.Write(Consistency);
            writer.Write(Aggression);
            writer.Write(BigMatch);
            writer.Write(InjuryProne);
            writer.Write(Leadership);
            writer.Write(Versatility);
            writer.Write(SetPieces);
            writer.Write(Penalty);
            writer.Write(Creativity);
            writer.Write(Movement);
            writer.Write(Positioning);
            writer.Write(WorkRate);
            writer.Write(Flair);
            writer.Write(Handling);
            writer.Write(Kicking);
            writer.Write(Agility);
            writer.Write(Aerial);
            writer.Write(Reflexes);
            writer.Write(Communication);
            writer.Write(Throwing);
            writer.Write(GK);
            writer.Write(LIB);
            writer.Write(LB);
            writer.Write(CB);
            writer.Write(RB);
            writer.Write(DM);
            writer.Write(LM);
            writer.Write(CM);
            writer.Write(RM);
            writer.Write(LW);
            writer.Write(AM);
            writer.Write(RW);
            writer.Write(CF);
            writer.Write(LWB);
            writer.Write(RWB);
            writer.Write(LeftFoot);
            writer.Write(RightFoot);
            writer.Write(CA);
            writer.Write(PA);
            writer.Write(HomeReputation);
            writer.Write(CurrentReputation);
            writer.Write(WorldReputation);
            writer.Write(InternationalRetirement);
            writer.Write(Unknown1);
            writer.Write(SquadNumber);
            writer.Write(PreferredSquadNumber);
            writer.Write(Height);
            writer.Write(Weight);
            writer.Write(Unknown2);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }
    }
}
