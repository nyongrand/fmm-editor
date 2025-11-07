/**
 * Provides extension methods for Array slicing operations.
 */
export class ArrayExtension {
  /**
   * Removes and returns the specified number of elements from the beginning of the array.
   * @param array - The source array to slice from
* @param length - The number of elements to remove and return
   * @returns An array containing the removed elements
   * @throws Error when length is negative or greater than the array length
   */
  static slice<T>(array: T[], length: number): T[] {
    if (length < 0) {
      throw new RangeError('Number of elements to remove must be non-negative.');
    }

    if (length > array.length) {
      throw new RangeError(
        'Number of elements to remove cannot be greater than the number of elements in the array.'
      );
    }

    return array.splice(0, length);
  }

  /**
   * Extracts a slice of the specified length from the array starting at the current index and advances the index.
   * @param originalArray - The source array to slice from
   * @param currentIndex - An object containing the current index position, which will be incremented by the length
   * @param length - The number of elements to extract
   * @returns An array containing the extracted elements
   * @throws Error when length is negative or would exceed array bounds
*/
  static sliceWithIndex<T>(
    originalArray: T[],
    currentIndex: { value: number },
    length: number
  ): T[] {
    if (length < 0) {
      throw new RangeError('Number of elements to remove must be non-negative.');
    }

    if (currentIndex.value + length > originalArray.length) {
      throw new RangeError(
        'Number of elements to remove cannot be greater than the number of elements in the array.'
      );
    }

    const result = originalArray.slice(currentIndex.value, currentIndex.value + length);
    currentIndex.value += length;
    return result;
  }

  /**
* Extracts a slice from a Buffer starting at the current index and advances the index.
   * @param buffer - The source buffer to slice from
   * @param currentIndex - An object containing the current index position, which will be incremented by the length
   * @param length - The number of bytes to extract
   * @returns A buffer containing the extracted bytes
   * @throws Error when length is negative or would exceed buffer bounds
   */
  static sliceBuffer(
    buffer: Buffer,
    currentIndex: { value: number },
    length: number
  ): Buffer {
    if (length < 0) {
      throw new RangeError('Number of bytes to extract must be non-negative.');
    }

    if (currentIndex.value + length > buffer.length) {
      throw new RangeError(
        'Number of bytes to extract cannot be greater than the buffer length.'
      );
    }

    const result = buffer.slice(currentIndex.value, currentIndex.value + length);
    currentIndex.value += length;
    return result;
  }
}
