using System.Drawing;

namespace FMMLibrary
{
    /// <summary>
    /// Represents a nation with all its properties and attributes.
    /// </summary>
    public class Nation
    {
        /// <summary>
        /// Gets or sets the unique identifier for the nation.
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// Gets or sets the nation identifier.
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the nation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the terminator byte after the name.
        /// </summary>
        public byte Terminator1 { get; set; }

        /// <summary>
        /// Gets or sets the nationality descriptor.
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// Gets or sets the terminator byte after the nationality.
        /// </summary>
        public byte Terminator2 { get; set; }

        /// <summary>
        /// Gets or sets the code name of the nation.
        /// </summary>
        public string CodeName { get; set; }

        /// <summary>
        /// Gets or sets the continent identifier.
        /// </summary>
        public short ContinentId { get; set; }

        /// <summary>
        /// Capital city identifier.
        /// </summary>
        public short CapitalId { get; set; }

        /// <summary>
        /// Gets or sets the main stadium identifier.
        /// </summary>
        public short StadiumId { get; set; }

        /// <summary>
        /// The state of development of the nation.
        /// 1 - Developed
        /// 2 - Developing
        /// 3 - Third World
        /// </summary>
        public int StateOfDevelopment { get; set; }

        /// <summary>
        /// Gets or sets an unknown short value.
        /// </summary>
        public short Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown3 { get; set; }

        /// <summary>
        /// Gets or sets the array of languages spoken in the nation (language ID and proficiency level).
        /// </summary>
        public (short Id, byte Proficiency)[] Languages { get; set; }

        /// <summary>
        /// Gets or sets whether the nation is active in the game (1 = active, 0 = inactive).
        /// </summary>
        public bool HasMaleTeam { get; set; }

        public NationalTeam? MaleTeam { get; set; }

        /// <summary>
        /// Gets or sets whether the nation has a second coefficient set (1 = has coefficients, 0 = no coefficients).
        /// </summary>
        public bool HasFemaleTeam { get; set; }

        public NationalTeam? FemaleTeam { get; set; }

        public class NationalTeam
        {
            /// <summary>
            /// Gets or sets the first national color (nullable, only if IsActive is 1).
            /// </summary>
            public Color Color1 { get; set; }

            /// <summary>
            /// Gets or sets an unknown integer value (nullable, only if IsActive is 1).
            /// </summary>
            public Color Color2 { get; set; }

            /// <summary>
            /// Gets or sets an unknown integer value (nullable, only if IsActive is 1).
            /// </summary>
            public Color Color3 { get; set; }

            /// <summary>
            /// Gets or sets the second national color (nullable, only if IsActive is 1).
            /// </summary>
            public Color Color4 { get; set; }

            /// <summary>
            /// Gets or sets an unknown byte value (nullable, only if IsActive is 1).
            /// </summary>
            public byte Unknown6 { get; set; }

            /// <summary>
            /// Gets or sets an unknown short value (nullable, only if IsActive is 1).
            /// </summary>
            public short Unknown7 { get; set; }

            /// <summary>
            /// Gets or sets an unknown byte value (nullable, only if IsActive is 1).
            /// </summary>
            public byte Unknown8 { get; set; }

            /// <summary>
            /// Gets or sets whether the nation has a FIFA/UEFA ranking (nullable, only if IsActive is 1).
            /// </summary>
            public bool IsRanked { get; set; }

            /// <summary>
            /// Gets or sets the FIFA/UEFA ranking position (nullable, only if IsActive is 1).
            /// </summary>
            public short Ranking { get; set; }

            /// <summary>
            /// Gets or sets the ranking points (nullable, only if IsActive is 1).
            /// </summary>
            public short Points { get; set; }

            /// <summary>
            /// Gets or sets an unknown short value (nullable, only if IsActive is 1).
            /// </summary>
            public short Unknown9 { get; set; }

            /// <summary>
            /// Gets or sets the first set of coefficient values (only if IsActive is 1).
            /// </summary>
            public float[] MaleCoefficients { get; set; } = [];

            /// <summary>
            /// Gets or sets unknown data (11 bytes, only if IsActive is 1).
            /// </summary>
            public byte[] Unknown10 { get; set; } = [];

            public NationalTeam(BinaryReaderEx reader)
            {
                Color1 = reader.ReadColor();
                Color2 = reader.ReadColor();
                Color3 = reader.ReadColor();
                Color4 = reader.ReadColor();

                Unknown6 = reader.ReadByte();
                Unknown7 = reader.ReadInt16();
                Unknown8 = reader.ReadByte();

                IsRanked = reader.ReadBoolean();
                Ranking = reader.ReadInt16();
                Points = reader.ReadInt16();

                Unknown9 = reader.ReadInt16();

                MaleCoefficients = new float[reader.ReadByte()];
                for (int i = 0; i < MaleCoefficients.Length; i++)
                    MaleCoefficients[i] = reader.ReadSingle();

                Unknown10 = reader.ReadBytes(11);
            }

            public void Write(BinaryWriterEx writer)
            {
                writer.Write(Color1);
                writer.Write(Color2);
                writer.Write(Color3);
                writer.Write(Color4);

                writer.Write(Unknown6);
                writer.Write(Unknown7);
                writer.Write(Unknown8);

                writer.Write(IsRanked);
                writer.Write(Ranking);
                writer.Write(Points);

                writer.Write(Unknown9);
                writer.Write((byte)MaleCoefficients.Length);
                for (int i = 0; i < MaleCoefficients.Length; i++)
                    writer.Write(MaleCoefficients[i]);

                writer.Write(Unknown10);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nation"/> class by reading from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader containing the nation data.</param>
        public Nation(BinaryReaderEx reader)
        {
            Uid = reader.ReadInt32();
            Id = reader.ReadInt16();

            Name = reader.ReadString();
            Terminator1 = reader.ReadByte();
            Nationality = reader.ReadString();
            Terminator2 = reader.ReadByte();
            CodeName = reader.ReadString();

            ContinentId = reader.ReadInt16();
            CapitalId = reader.ReadInt16();
            StadiumId = reader.ReadInt16();

            StateOfDevelopment = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadByte();

            Languages = new (short Id, byte Proficiency)[reader.ReadByte()];
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = (reader.ReadInt16(), reader.ReadByte());

            HasMaleTeam = reader.ReadBoolean();
            MaleTeam = HasMaleTeam ? new NationalTeam(reader) : null;

            HasFemaleTeam = reader.ReadBoolean();
            FemaleTeam = HasFemaleTeam ? new NationalTeam(reader) : null;
        }

        /// <summary>
        /// Converts the nation data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized nation data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriterEx(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the nation data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the nation data to.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Uid);
            writer.Write(Id);

            writer.Write(Name);
            writer.Write(Terminator1);
            writer.Write(Nationality);
            writer.Write(Terminator2);
            writer.Write(CodeName);

            writer.Write(ContinentId);
            writer.Write(CapitalId);
            writer.Write(StadiumId);

            writer.Write(StateOfDevelopment);
            writer.Write(Unknown2);
            writer.Write(Unknown3);

            writer.Write((byte)Languages.Length);
            for (int i = 0; i < Languages.Length; i++)
            {
                writer.Write(Languages[i].Id);
                writer.Write(Languages[i].Proficiency);
            }

            writer.Write(HasMaleTeam);
            if (HasMaleTeam && MaleTeam != null)
                MaleTeam.Write(writer);

            writer.Write(HasFemaleTeam);
            if (HasFemaleTeam && FemaleTeam != null)
                FemaleTeam.Write(writer);
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
