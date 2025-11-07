/**
 * Represents a name entry with associated metadata.
 */
export class Name {
  /** Gets or sets the identifier for this name entry. */
  id: number = 0;

  /** Gets or sets the nation identifier (stored as hexadecimal string). */
  nation: string = '';

  /** Gets or sets other associated data (stored as hexadecimal string). */
  others: string = '';

  /** Gets or sets the actual name value. */
  value: string = '';

  /**
   * Returns a string representation of the name entry.
   * @returns A string containing the ID and name value
*/
  toString(): string {
    return `${this.id} ${this.value}`;
  }

  /**
   * Converts the name entry to a buffer for serialization.
* @returns A buffer representing the serialized name data
   */
  toBuffer(): Buffer {
    const nameBytes = Buffer.from(this.value, 'utf-8');
    const buffers: Buffer[] = [];

    // 4 zero bytes
    buffers.push(Buffer.from([0x00, 0x00, 0x00, 0x00]));

    // ID (4 bytes)
    const idBuffer = Buffer.alloc(4);
    idBuffer.writeInt32LE(this.id, 0);
    buffers.push(idBuffer);

    // Nation (4 bytes from hex string)
    buffers.push(Buffer.from(this.nation, 'hex'));

    // Others (4 bytes from hex string)
    buffers.push(Buffer.from(this.others, 'hex'));

    // Name length (4 bytes)
    const lengthBuffer = Buffer.alloc(4);
    lengthBuffer.writeInt32LE(nameBytes.length, 0);
    buffers.push(lengthBuffer);

    // Name value
    buffers.push(nameBytes);

    return Buffer.concat(buffers);
  }

  /**
   * Creates a Name instance from a buffer.
   * @param buffer - The buffer containing the name data
   * @param offset - The current offset in the buffer
   * @returns An object containing the Name instance and the new offset
   */
  static fromBuffer(buffer: Buffer, offset: number): { name: Name; offset: number } {
    const name = new Name();

    // Skip 4 zero bytes
    offset += 4;

    name.id = buffer.readInt32LE(offset);
    offset += 4;

    name.nation = buffer.slice(offset, offset + 4).toString('hex').toUpperCase();
    offset += 4;

    name.others = buffer.slice(offset, offset + 4).toString('hex').toUpperCase();
    offset += 4;

    const length = buffer.readInt32LE(offset);
    offset += 4;

    name.value = buffer.toString('utf-8', offset, offset + length);
    offset += length;

    return { name, offset };
  }
}
