namespace FMMLibrary
{
    public class Relationship(BinaryReaderEx reader)
    {
        public byte Level { get; set; } = reader.ReadByte();
        public byte Type { get; set; } = reader.ReadByte();
        public byte Unknown { get; set; } = reader.ReadByte();
        public int Uid { get; set; } = reader.ReadInt32();
        public byte Reason { get; set; } = reader.ReadByte();

        /// <summary>
        /// Writes the current object's data to the specified binary writer in a predefined format.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriterEx"/> to which the object's data will be written. Cannot be null.</param>
        public void Write(BinaryWriterEx writer)
        {
            writer.Write(Level);
            writer.Write(Type);
            writer.Write(Unknown);
            writer.Write(Uid);
            writer.Write(Reason);
        }
    }
}
