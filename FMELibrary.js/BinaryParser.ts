/**
 * Provides utility functions for reading and writing binary data with custom serialization logic.
 */
export class BinaryParser {
  /**
* Reads a UTF-8 encoded string of the specified length from the buffer.
   * @param buffer - The buffer to read from
   * @param offset - The current offset in the buffer
   * @param length - The number of bytes to read
   * @returns An object containing the decoded string and the new offset
 */
  static readString(buffer: Buffer, offset: number, length: number): { value: string; offset: number } {
    const value = buffer.toString('utf-8', offset, offset + length);
    return { value, offset: offset + length };
  }

  /**
   * Reads a length-prefixed string from the buffer. The first 4 bytes represent the string length.
   * @param buffer - The buffer to read from
   * @param offset - The current offset in the buffer
   * @returns An object containing the decoded string and the new offset
   */
  static readStringEx(buffer: Buffer, offset: number): { value: string; offset: number } {
    const length = buffer.readInt32LE(offset);
    offset += 4;
    return this.readString(buffer, offset, length);
  }

  /**
   * Reads a 16-bit RGB565 color value and converts it to RGB components.
   * @param buffer - The buffer to read from
   * @param offset - The current offset in the buffer
* @returns An object containing the RGB color values and the new offset
   */
  static readColor(buffer: Buffer, offset: number): { r: number; g: number; b: number; offset: number } {
    const color = buffer.readInt16LE(offset);
    const red = color >> 10;
    const green = (color >> 5) & 0x1F;
    const blue = color & 0x1F;

    return {
      r: red << 3,
      g: green << 3,
      b: blue << 3,
      offset: offset + 2
    };
  }

  /**
   * Writes various data types to a buffer with automatic type detection and appropriate serialization.
* @param value - The value to write. Supported types: number (byte, short, int, float), Buffer, string, color object, null/undefined
   * @param buffer - The buffer to write to
   * @param offset - The current offset in the buffer
   * @returns The new offset after writing
   * @throws Error when the value type is not supported
   */
  static writeEx(value: any, buffer: Buffer, offset: number): number {
    if (value === null || value === undefined) {
      return offset;
    }

    if (typeof value === 'number') {
      // Determine size based on context - caller should specify type
      buffer.writeInt32LE(value, offset);
      return offset + 4;
    }

    if (Buffer.isBuffer(value)) {
      value.copy(buffer, offset);
      return offset + value.length;
    }

    if (typeof value === 'string') {
      const bytes = Buffer.from(value, 'utf-8');
      buffer.writeInt32LE(bytes.length, offset);
      offset += 4;
      bytes.copy(buffer, offset);
      return offset + bytes.length;
    }

    if (typeof value === 'object' && 'r' in value && 'g' in value && 'b' in value) {
      const red = value.r >> 3;
      const green = value.g >> 3;
      const blue = value.b >> 3;
      buffer.writeInt16LE((red << 10) | (green << 5) | blue, offset);
      return offset + 2;
    }

    throw new Error(`Unsupported type: ${typeof value}`);
  }

  /**
   * Writes a byte value to the buffer.
   */
  static writeByte(value: number, buffer: Buffer, offset: number): number {
    buffer.writeUInt8(value, offset);
    return offset + 1;
  }

  /**
     * Writes a short (16-bit integer) value to the buffer.
     */
  static writeInt16(value: number, buffer: Buffer, offset: number): number {
    buffer.writeInt16LE(value, offset);
    return offset + 2;
  }

  /**
   * Writes an int (32-bit integer) value to the buffer.
   */
  static writeInt32(value: number, buffer: Buffer, offset: number): number {
    buffer.writeInt32LE(value, offset);
    return offset + 4;
  }

  /**
   * Writes a float (32-bit floating point) value to the buffer.
   */
  static writeFloat(value: number, buffer: Buffer, offset: number): number {
    buffer.writeFloatLE(value, offset);
    return offset + 4;
  }
}
