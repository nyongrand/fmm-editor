/**
 * Represents an affiliation relationship between two clubs.
 */
export class Affiliate {
  /**
   * Gets or sets an unknown integer value.
   */
  unknown1: number = 0;

  /**
   * Gets or sets the identifier of the first club in the affiliation.
   */
  club1Id: number = 0;

  /**
   * Gets or sets the identifier of the second club in the affiliation.
   */
  club2Id: number = 0;

  /**
   * Gets or sets the start day of the affiliation.
*/
  startDay: number = 0;

  /**
   * Gets or sets the start year of the affiliation.
 */
  startYear: number = 0;

  /**
   * Gets or sets the end day of the affiliation.
   */
  endDay: number = 0;

  /**
   * Gets or sets the end year of the affiliation.
   */
  endYear: number = 0;

  /**
       * Gets or sets an unknown byte value.
       */
  unknown2: number = 0;

  /**
* Initializes a new instance of the Affiliate class by reading from a buffer.
   * @param buffer - The buffer containing the affiliate data
   * @param offset - The current offset in the buffer
   * @returns An object containing the Affiliate instance and the new offset
   */
  static fromBuffer(buffer: Buffer, offset: number): { affiliate: Affiliate; offset: number } {
    const affiliate = new Affiliate();

    affiliate.unknown1 = buffer.readInt32LE(offset);
    offset += 4;

    affiliate.club1Id = buffer.readInt32LE(offset);
    offset += 4;

    affiliate.club2Id = buffer.readInt32LE(offset);
    offset += 4;

    affiliate.startDay = buffer.readInt16LE(offset);
    offset += 2;

    affiliate.startYear = buffer.readInt16LE(offset);
    offset += 2;

    affiliate.endDay = buffer.readInt16LE(offset);
    offset += 2;

    affiliate.endYear = buffer.readInt16LE(offset);
    offset += 2;

    affiliate.unknown2 = buffer.readUInt8(offset);
    offset += 1;

    return { affiliate, offset };
  }

  /**
   * Writes the affiliate data to the specified buffer.
   * @param buffer - The buffer to write the affiliate data to
   * @param offset - The current offset in the buffer
   * @returns The new offset after writing
   */
  write(buffer: Buffer, offset: number): number {
    buffer.writeInt32LE(this.unknown1, offset);
    offset += 4;

    buffer.writeInt32LE(this.club1Id, offset);
    offset += 4;

    buffer.writeInt32LE(this.club2Id, offset);
    offset += 4;

    buffer.writeInt16LE(this.startDay, offset);
    offset += 2;

    buffer.writeInt16LE(this.startYear, offset);
    offset += 2;

    buffer.writeInt16LE(this.endDay, offset);
    offset += 2;

    buffer.writeInt16LE(this.endYear, offset);
    offset += 2;

    buffer.writeUInt8(this.unknown2, offset);
    offset += 1;

    return offset;
  }

  /**
   * Calculates the size of the affiliate data in bytes.
   * @returns The size in bytes
*/
  static getSize(): number {
    return 4 + 4 + 4 + 2 + 2 + 2 + 2 + 1; // 21 bytes
  }
}
