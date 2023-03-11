namespace FMELibrary
{
    static class ListExtension
    {
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
