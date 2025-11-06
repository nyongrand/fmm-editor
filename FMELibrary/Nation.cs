namespace FMELibrary
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
        /// Gets or sets the capital city identifier.
        /// </summary>
        public short CityId { get; set; }

        /// <summary>
        /// Gets or sets the main stadium identifier.
        /// </summary>
        public short StadiumId { get; set; }

        /// <summary>
        /// Gets or sets an unknown integer value.
        /// </summary>
        public int Unknown1 { get; set; }

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
        public (short, byte)[] Languages { get; set; }

        /// <summary>
        /// Gets or sets whether the nation is active in the game (1 = active, 0 = inactive).
        /// </summary>
        public byte IsActive { get; set; }

        /// <summary>
        /// Gets or sets the first national color (nullable, only if IsActive is 1).
        /// </summary>
        public short? Color1 { get; set; }

        /// <summary>
        /// Gets or sets an unknown integer value (nullable, only if IsActive is 1).
        /// </summary>
        public int? Unknown5 { get; set; }

        /// <summary>
        /// Gets or sets the second national color (nullable, only if IsActive is 1).
        /// </summary>
        public short? Color2 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value (nullable, only if IsActive is 1).
        /// </summary>
        public byte? Unknown6 { get; set; }

        /// <summary>
        /// Gets or sets an unknown short value (nullable, only if IsActive is 1).
        /// </summary>
        public short? Unknown7 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value (nullable, only if IsActive is 1).
        /// </summary>
        public byte? Unknown8 { get; set; }

        /// <summary>
        /// Gets or sets whether the nation has a FIFA/UEFA ranking (nullable, only if IsActive is 1).
        /// </summary>
        public byte? IsRanked { get; set; }

        /// <summary>
        /// Gets or sets the FIFA/UEFA ranking position (nullable, only if IsActive is 1).
        /// </summary>
        public short? Ranking { get; set; }

        /// <summary>
        /// Gets or sets the ranking points (nullable, only if IsActive is 1).
        /// </summary>
        public short? Points { get; set; }

        /// <summary>
        /// Gets or sets an unknown short value (nullable, only if IsActive is 1).
        /// </summary>
        public short? Unknown9 { get; set; }

        /// <summary>
        /// Gets or sets the first set of coefficient values (only if IsActive is 1).
        /// </summary>
        public float[] Coefficients1 { get; set; } = [];

        /// <summary>
        /// Gets or sets unknown data (11 bytes, only if IsActive is 1).
        /// </summary>
        public byte[] Unknown10 { get; set; } = [];

        /// <summary>
        /// Gets or sets whether the nation has a second coefficient set (1 = has coefficients, 0 = no coefficients).
        /// </summary>
        public byte HasCoefficient2 { get; set; }

        /// <summary>
        /// Gets or sets unknown data (16 bytes, only if HasCoefficient2 is 1).
        /// </summary>
        public byte[] Unknown11 { get; set; } = [];

        /// <summary>
        /// Gets or sets an unknown byte value (nullable, only if HasCoefficient2 is 1).
        /// </summary>
        public byte? Unknown12 { get; set; }

        /// <summary>
        /// Gets or sets an unknown short value (nullable, only if HasCoefficient2 is 1).
        /// </summary>
        public short? Unknown13 { get; set; }

        /// <summary>
        /// Gets or sets the second set of coefficient values (only if HasCoefficient2 is 1).
        /// </summary>
        public float[] Coefficients2 { get; set; } = [];

        /// <summary>
        /// Gets or sets unknown data (11 bytes, only if HasCoefficient2 is 1).
        /// </summary>
        public byte[] Unknown14 { get; set; } = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="Nation"/> class by reading from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader containing the nation data.</param>
        public Nation(BinaryReader reader)
        {
            Uid = reader.ReadInt32();
            Id = reader.ReadInt16();

            Name = reader.ReadStringEx();
            Terminator1 = reader.ReadByte();
            Nationality = reader.ReadStringEx();
            Terminator2 = reader.ReadByte();
            CodeName = reader.ReadStringEx();

            ContinentId = reader.ReadInt16();
            CityId = reader.ReadInt16();
            StadiumId = reader.ReadInt16();

            Unknown1 = reader.ReadInt32();
            Unknown2 = reader.ReadInt16();
            Unknown3 = reader.ReadByte();

            Languages = new (short, byte)[reader.ReadByte()];
            for (int i = 0; i < Languages.Length; i++)
                Languages[i] = (reader.ReadInt16(), reader.ReadByte());

            IsActive = reader.ReadByte();
            if (IsActive == 1)
            {
                Color1 = reader.ReadInt16();
                Unknown5 = reader.ReadInt32();
                Color2 = reader.ReadInt16();

                Unknown6 = reader.ReadByte();
                Unknown7 = reader.ReadInt16();
                Unknown8 = reader.ReadByte();

                IsRanked = reader.ReadByte();
                Ranking = reader.ReadInt16();
                Points = reader.ReadInt16();

                Unknown9 = reader.ReadInt16();

                Coefficients1 = new float[reader.ReadByte()];
                for (int i = 0; i < Coefficients1.Length; i++)
                    Coefficients1[i] = reader.ReadSingle();

                Unknown10 = reader.ReadBytes(11);
            }

            HasCoefficient2 = reader.ReadByte();
            if (HasCoefficient2 == 1)
            {
                Unknown11 = reader.ReadBytes(16);
                Unknown12 = reader.ReadByte();
                Unknown13 = reader.ReadInt16();

                Coefficients2 = new float[reader.ReadByte()];
                for (int i = 0; i < Coefficients2.Length; i++)
                    Coefficients2[i] = reader.ReadSingle();

                Unknown14 = reader.ReadBytes(11);
            }
        }

        /// <summary>
        /// Converts the nation data to a byte array.
        /// </summary>
        /// <returns>A byte array representing the serialized nation data.</returns>
        public byte[] ToBytes()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);
            Write(writer);
            return stream.ToArray();
        }

        /// <summary>
        /// Writes the nation data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the nation data to.</param>
        public void Write(BinaryWriter writer)
        {
            writer.WriteEx(Uid);
            writer.WriteEx(Id);

            writer.WriteEx(Name);
            writer.WriteEx(Terminator1);
            writer.WriteEx(Nationality);
            writer.WriteEx(Terminator2);
            writer.WriteEx(CodeName);

            writer.WriteEx(ContinentId);
            writer.WriteEx(CityId);
            writer.WriteEx(StadiumId);

            writer.WriteEx(Unknown1);
            writer.WriteEx(Unknown2);
            writer.WriteEx(Unknown3);

            writer.WriteEx((byte)Languages.Length);
            for (int i = 0; i < Languages.Length; i++)
            {
                writer.WriteEx(Languages[i].Item1);
                writer.WriteEx(Languages[i].Item2);
            }

            writer.WriteEx(IsActive);

            if (IsActive == 1)
            {
                writer.WriteEx(Color1);
                writer.WriteEx(Unknown5);
                writer.WriteEx(Color2);

                writer.WriteEx(Unknown6);
                writer.WriteEx(Unknown7);
                writer.WriteEx(Unknown8);

                writer.WriteEx(IsRanked);
                writer.WriteEx(Ranking);
                writer.WriteEx(Points);

                writer.WriteEx(Unknown9);
                writer.WriteEx((byte)Coefficients1.Length);
                for (int i = 0; i < Coefficients1.Length; i++)
                {
                    writer.WriteEx(Coefficients1[i]);
                }

                writer.WriteEx(Unknown10);
            }

            writer.WriteEx(HasCoefficient2);
            if (HasCoefficient2 == 1)
            {
                writer.WriteEx(Unknown11);
                writer.WriteEx(Unknown12);
                writer.WriteEx(Unknown13);

                writer.WriteEx((byte)Coefficients2.Length);
                for (int i = 0; i < Coefficients2.Length; i++)
                {
                    writer.WriteEx(Coefficients2[i]);
                }

                writer.WriteEx(Unknown14);
            }

            //writer.Write((byte)ExtraNames.Length);
            //for (int i = 0; i < ExtraNames.Length; i++)
            //{
            //    writer.WriteEx(ExtraNames[i].Item1);
            //    writer.Write(ExtraNames[i].Item2);
            //    writer.Write(ExtraNames[i].Item3);
            //}
        }
    }
}
