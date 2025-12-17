namespace FMMLibrary
{
    /// <summary>
    /// Defines a contract for parser classes that load, manage, and persist data from binary files.
    /// </summary>
    /// <typeparam name="T">The type of items being parsed and managed.</typeparam>
    public interface IParser<T>
    {
        /// <summary>
        /// Gets or sets the file path of the source data.
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// Gets the file header (8 bytes).
        /// </summary>
        byte[] Header { get; set; }

        /// <summary>
        /// Gets the read-only list of all items.
        /// </summary>
        IReadOnlyList<T> Items { get; }

        /// <summary>
        /// Adds the specified item to the collection.
        /// </summary>
        /// <param name="item">The item to add to the collection.</param>
        void Add(T item);

        /// <summary>
        /// Converts the parser data to a byte array for serialization.
        /// </summary>
        /// <returns>A byte array representing the serialized data.</returns>
        byte[] ToBytes();

        /// <summary>
        /// Saves data back to the file path.
        /// </summary>
        /// <param name="filepath">Optional file path. If null, saves to the original file path.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task Save(string? filepath = null);
    }
}
