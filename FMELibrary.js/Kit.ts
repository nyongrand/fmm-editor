/**
 * Represents a football kit (uniform) with its colors and properties.
 */
export class Kit {
  /**
   * Gets or sets an unknown byte value.
   */
  unknown1: number = 0;

  /**
     * Gets or sets an unknown byte value.
     */
  unknown2: number = 0;

  /**
   * Gets or sets the array of 10 color values for the kit.
   */
  colors: number[] = [];

  /**
   * Initializes a new instance of the Kit class by reading from a buffer.
   * @param buffer - The buffer containing the kit data
   * @param offset - The current offset in the buffer
   * @returns An object containing the Kit instance and the new offset
   */
  static fromBuffer(buffer: Buffer, offset: number): { kit: Kit; offset: number } {
    const kit = new Kit();

    kit.unknown1 = buffer.readUInt8(offset);
    offset += 1;

    kit.unknown2 = buffer.readUInt8(offset);
    offset += 1;

    kit.colors = [];
    for (let i = 0; i < 10; i++) {
      kit.colors.push(buffer.readInt16LE(offset));
      offset += 2;
    }

    return { kit, offset };
  }

  /**
   * Writes the kit data to the specified buffer.
   * @param buffer - The buffer to write the kit data to
 * @param offset - The current offset in the buffer
   * @returns The new offset after writing
   */
  write(buffer: Buffer, offset: number): number {
    buffer.writeUInt8(this.unknown1, offset);
    offset += 1;

    buffer.writeUInt8(this.unknown2, offset);
    offset += 1;

    for (let i = 0; i < this.colors.length; i++) {
      buffer.writeInt16LE(this.colors[i], offset);
      offset += 2;
    }

    return offset;
  }

  /**
   * Calculates the size of the kit data in bytes.
   * @returns The size in bytes (22 bytes: 1 + 1 + 10*2)
   */
  static getSize(): number {
    return 1 + 1 + (10 * 2); // unknown1 + unknown2 + 10 colors
  }
}
