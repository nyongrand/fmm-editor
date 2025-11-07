import { BinaryParser } from './BinaryParser';

/**
 * Represents a football competition (league, cup, etc.) with all its properties.
 */
export class Competition {
  /** Gets or sets the competition identifier. */
  id: number = 0;

  /** Gets or sets the unique identifier for the competition. */
  uid: number = 0;

  /** Gets or sets the full name of the competition. */
  fullName: string = '';

  /** Gets or sets an unknown byte value. */
  unknown1: number = 0;

  /** Gets or sets the short name of the competition. */
  shortName: string = '';

  /** Gets or sets an unknown byte value. */
  unknown2: number = 0;

  /** Gets or sets the code name of the competition. */
  codeName: string = '';

  /** Gets or sets the type of competition. */
  type: number = 0;

  /** Gets or sets the continent identifier. */
  continentId: number = 0;

  /** Gets or sets the nation identifier. */
  nationId: number = 0;

  /** Gets or sets the first color value. */
  color1: number = 0;

  /** Gets or sets the second color value. */
  color2: number = 0;

  /** Gets or sets the competition's reputation. */
  reputation: number = 0;

  /** Gets or sets the competition level. */
  level: number = 0;

  /** Gets or sets the main competition identifier. */
  mainComp: number = 0;

  /** Gets or sets the array of qualifier data (8 bytes each). */
  qualifiers: Buffer[] = [];

  /** Gets or sets the first ranking value. */
  rank1: number = 0;

  /** Gets or sets the second ranking value. */
  rank2: number = 0;

  /** Gets or sets the third ranking value. */
  rank3: number = 0;

  /** Gets or sets the first year value. */
  year1: number = 0;

  /** Gets or sets the second year value. */
  year2: number = 0;

  /** Gets or sets the third year value. */
  year3: number = 0;

  /** Gets or sets an unknown byte value. */
  unknown3: number = 0;

  /** Gets or sets an unknown byte value. */
  unknown4: number = 0;

  // Extra fields
  /** Gets or sets the nation name (extra/computed field). */
  nation: string = '';

  /**
   * Initializes a new instance of the Competition class by reading from a buffer.
   * @param buffer - The buffer containing the competition data
   * @param offset - The current offset in the buffer
* @returns An object containing the Competition instance and the new offset
   */
  static fromBuffer(buffer: Buffer, offset: number): { competition: Competition; offset: number } {
    const competition = new Competition();

    competition.id = buffer.readInt16LE(offset);
    offset += 2;

    competition.uid = buffer.readInt32LE(offset);
    offset += 4;

    const fullNameResult = BinaryParser.readStringEx(buffer, offset);
    competition.fullName = fullNameResult.value;
    offset = fullNameResult.offset;

    competition.unknown1 = buffer.readUInt8(offset);
    offset += 1;

    const shortNameResult = BinaryParser.readStringEx(buffer, offset);
    competition.shortName = shortNameResult.value;
    offset = shortNameResult.offset;

    competition.unknown2 = buffer.readUInt8(offset);
    offset += 1;

    const codeNameResult = BinaryParser.readStringEx(buffer, offset);
    competition.codeName = codeNameResult.value;
    offset = codeNameResult.offset;

    competition.type = buffer.readUInt8(offset);
    offset += 1;

    competition.continentId = buffer.readInt16LE(offset);
    offset += 2;

    competition.nationId = buffer.readInt16LE(offset);
    offset += 2;

    competition.color1 = buffer.readInt16LE(offset);
    offset += 2;

    competition.color2 = buffer.readInt16LE(offset);
    offset += 2;

    competition.reputation = buffer.readInt16LE(offset);
    offset += 2;

    competition.level = buffer.readUInt8(offset);
    offset += 1;

    competition.mainComp = buffer.readInt16LE(offset);
    offset += 2;

    const qualifiersCount = buffer.readInt32LE(offset);
    offset += 4;

    competition.qualifiers = [];
    for (let i = 0; i < qualifiersCount; i++) {
      competition.qualifiers.push(buffer.slice(offset, offset + 8));
      offset += 8;
    }

    competition.rank1 = buffer.readInt32LE(offset);
    offset += 4;

    competition.rank2 = buffer.readInt32LE(offset);
    offset += 4;

    competition.rank3 = buffer.readInt32LE(offset);
    offset += 4;

    competition.year1 = buffer.readInt16LE(offset);
    offset += 2;

    competition.year2 = buffer.readInt16LE(offset);
    offset += 2;

    competition.year3 = buffer.readInt16LE(offset);
    offset += 2;

    competition.unknown3 = buffer.readUInt8(offset);
    offset += 1;

    competition.unknown4 = buffer.readUInt8(offset);
    offset += 1;

    return { competition, offset };
  }

  /**
* Converts the competition data to a buffer.
   * @returns A buffer representing the serialized competition data
   */
  toBuffer(): Buffer {
    let size = 2 + 4; // id + uid
    size += 4 + Buffer.byteLength(this.fullName, 'utf-8'); // fullName
    size += 1; // unknown1
    size += 4 + Buffer.byteLength(this.shortName, 'utf-8'); // shortName
    size += 1; // unknown2
    size += 4 + Buffer.byteLength(this.codeName, 'utf-8'); // codeName
    size += 1 + 2 + 2 + 2 + 2 + 2 + 1 + 2; // type to mainComp
    size += 4 + (this.qualifiers.length * 8); // qualifiers
    size += 4 + 4 + 4 + 2 + 2 + 2 + 1 + 1; // ranks, years, unknowns

    const buffer = Buffer.alloc(size);
    let offset = 0;

    buffer.writeInt16LE(this.id, offset);
    offset += 2;

    buffer.writeInt32LE(this.uid, offset);
    offset += 4;

    offset = BinaryParser.writeEx(this.fullName, buffer, offset);
    offset = BinaryParser.writeByte(this.unknown1, buffer, offset);
    offset = BinaryParser.writeEx(this.shortName, buffer, offset);
    offset = BinaryParser.writeByte(this.unknown2, buffer, offset);
    offset = BinaryParser.writeEx(this.codeName, buffer, offset);

    buffer.writeUInt8(this.type, offset);
    offset += 1;

    buffer.writeInt16LE(this.continentId, offset);
    offset += 2;

    buffer.writeInt16LE(this.nationId, offset);
    offset += 2;

    buffer.writeInt16LE(this.color1, offset);
    offset += 2;

    buffer.writeInt16LE(this.color2, offset);
    offset += 2;

    buffer.writeInt16LE(this.reputation, offset);
    offset += 2;

    buffer.writeUInt8(this.level, offset);
    offset += 1;

    buffer.writeInt16LE(this.mainComp, offset);
    offset += 2;

    buffer.writeInt32LE(this.qualifiers.length, offset);
    offset += 4;

    for (const qualifier of this.qualifiers) {
      qualifier.copy(buffer, offset);
      offset += 8;
    }

    buffer.writeInt32LE(this.rank1, offset);
    offset += 4;

    buffer.writeInt32LE(this.rank2, offset);
    offset += 4;

    buffer.writeInt32LE(this.rank3, offset);
    offset += 4;

    buffer.writeInt16LE(this.year1, offset);
    offset += 2;

    buffer.writeInt16LE(this.year2, offset);
    offset += 2;

    buffer.writeInt16LE(this.year3, offset);
    offset += 2;

    buffer.writeUInt8(this.unknown3, offset);
    offset += 1;

    buffer.writeUInt8(this.unknown4, offset);
    offset += 1;

    return buffer.slice(0, offset);
  }

  /**
   * Returns a string representation of the competition.
   * @returns A string containing the competition's ID, UID, and full name
   */
  toString(): string {
    return `${this.id} - ${this.uid} - ${this.fullName}`;
  }
}
