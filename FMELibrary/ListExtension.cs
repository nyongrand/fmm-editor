namespace FMELibrary
{
    /// <summary>
    /// Provides extension methods for List and Array slicing operations.
    /// </summary>
    static class ListExtension
    {
        /// <summary>
        /// Removes and returns the specified number of elements from the beginning of the list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="original">The source list to slice from.</param>
        /// <param name="length">The number of elements to remove and return.</param>
        /// <returns>An array containing the removed elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when length is negative or greater than the list count.</exception>
        public static T[] Slice<T>(this List<T> original, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Number of elements to remove must be non-negative.");
            }

            if (length > original.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Number of elements to remove cannot be greater than the number of elements in the list.");
            }

            var result = original.Take(length).ToArray();
            original.RemoveRange(0, length);
            return result;
        }

        /// <summary>
        /// Extracts a slice of the specified length from the array starting at the current index and advances the index.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="originalArray">The source array to slice from.</param>
        /// <param name="currentIndex">The current index position, which will be incremented by the length.</param>
        /// <param name="length">The number of elements to extract.</param>
        /// <returns>An array containing the extracted elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when length is negative or would exceed array bounds.</exception>
        public static T[] Slice<T>(this T[] originalArray, ref int currentIndex, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Number of elements to remove must be non-negative.");
            }

            if (currentIndex + length > originalArray.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(length),
                    "Number of elements to remove cannot be greater than the number of elements in the list.");
            }

            T[] result = new T[length];
            Array.Copy(originalArray, currentIndex, result, 0, length);
            currentIndex += length;
            return result;
        }
    }
}
