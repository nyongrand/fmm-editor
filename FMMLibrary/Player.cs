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

        public Player(BinaryReader reader)
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

        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Id);
            writer.WriteEx(Uid);
            writer.WriteEx(Crossing);
            writer.WriteEx(Dribbling);
            writer.WriteEx(Tackling);
            writer.WriteEx(Finishing);
            writer.WriteEx(LongShot);
            writer.WriteEx(Heading);
            writer.WriteEx(Jumping);
            writer.WriteEx(Passing);
            writer.WriteEx(Decision);
            writer.WriteEx(Unselfishness);
            writer.WriteEx(Pace);
            writer.WriteEx(Strength);
            writer.WriteEx(Stamina);
            writer.WriteEx(Technique);
            writer.WriteEx(Consistency);
            writer.WriteEx(Aggression);
            writer.WriteEx(BigMatch);
            writer.WriteEx(InjuryProne);
            writer.WriteEx(Leadership);
            writer.WriteEx(Versatility);
            writer.WriteEx(SetPieces);
            writer.WriteEx(Penalty);
            writer.WriteEx(Creativity);
            writer.WriteEx(Movement);
            writer.WriteEx(Positioning);
            writer.WriteEx(WorkRate);
            writer.WriteEx(Flair);
            writer.WriteEx(Handling);
            writer.WriteEx(Kicking);
            writer.WriteEx(Agility);
            writer.WriteEx(Aerial);
            writer.WriteEx(Reflexes);
            writer.WriteEx(Communication);
            writer.WriteEx(Throwing);
            writer.WriteEx(GK);
            writer.WriteEx(LIB);
            writer.WriteEx(LB);
            writer.WriteEx(CB);
            writer.WriteEx(RB);
            writer.WriteEx(DM);
            writer.WriteEx(LM);
            writer.WriteEx(CM);
            writer.WriteEx(RM);
            writer.WriteEx(LW);
            writer.WriteEx(AM);
            writer.WriteEx(RW);
            writer.WriteEx(CF);
            writer.WriteEx(LWB);
            writer.WriteEx(RWB);
            writer.WriteEx(LeftFoot);
            writer.WriteEx(RightFoot);
            writer.WriteEx(CA);
            writer.WriteEx(PA);
            writer.WriteEx(HomeReputation);
            writer.WriteEx(CurrentReputation);
            writer.WriteEx(WorldReputation);
            writer.WriteEx(InternationalRetirement);
            writer.WriteEx(Unknown1);
            writer.WriteEx(SquadNumber);
            writer.WriteEx(PreferredSquadNumber);
            writer.WriteEx(Height);
            writer.WriteEx(Weight);
            writer.WriteEx(Unknown2);
        }

        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }
    }
}
