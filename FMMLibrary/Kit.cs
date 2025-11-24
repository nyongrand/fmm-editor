namespace FMMLibrary
{
    /// <summary>
    /// Represents a football kit (uniform) with its colors and properties.
    /// </summary>
    public class Kit
    {
        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown1 { get; set; }

        /// <summary>
        /// Gets or sets an unknown byte value.
        /// </summary>
        public byte Unknown2 { get; set; }

        /// <summary>
        /// Gets or sets the array of 10 color values for the kit.
        /// </summary>
        public short[] Colors { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Kit"/> class by reading from a binary reader.
        /// </summary>
        /// <param name="reader">The binary reader containing the kit data.</param>
        public Kit(BinaryReader reader)
        {
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();

            Colors = new short[10];
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i] = reader.ReadInt16();
            }
        }

        /// <summary>
        /// Writes the kit data to the specified binary writer.
        /// </summary>
        /// <param name="writer">The binary writer to write the kit data to.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(Unknown1);
            writer.Write(Unknown2);

            for (int i = 0; i < Colors.Length; i++)
                writer.Write(Colors[i]);
        }
    }
}
