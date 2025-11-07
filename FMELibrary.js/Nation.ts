import { BinaryParser } from './BinaryParser';

/**
 * Represents a nation with all its properties and attributes.
 */
export class Nation {
  /** Gets or sets the unique identifier for the nation. */
  uid: number = 0;

  /** Gets or sets the nation identifier. */
  id: number = 0;

  /** Gets or sets the name of the nation. */
  name: string = '';

  /** Gets or sets the terminator byte after the name. */
  terminator1: number = 0;

  /** Gets or sets the nationality descriptor. */
  nationality: string = '';

  /** Gets or sets the terminator byte after the nationality. */
  terminator2: number = 0;

  /** Gets or sets the code name of the nation. */
  codeName: string = '';

  /** Gets or sets the continent identifier. */
  continentId: number = 0;

  /** Gets or sets the capital city identifier. */
  cityId: number = 0;

  /** Gets or sets the main stadium identifier. */
  stadiumId: number = 0;

  /** Gets or sets an unknown integer value. */
  unknown1: number = 0;

  /** Gets or sets an unknown short value. */
  unknown2: number = 0;

  /** Gets or sets an unknown byte value. */
  unknown3: number = 0;

  /** Gets or sets the array of languages spoken in the nation (language ID and proficiency level). */
  languages: { languageId: number; proficiency: number }[] = [];

  /** Gets or sets whether the nation is active in the game (1 = active, 0 = inactive). */
  isActive: number = 0;

  /** Gets or sets the first national color (nullable, only if isActive is 1). */
  color1?: number;

  /** Gets or sets an unknown integer value (nullable, only if isActive is 1). */
  unknown5?: number;

  /** Gets or sets the second national color (nullable, only if isActive is 1). */
  color2?: number;

  /** Gets or sets an unknown byte value (nullable, only if isActive is 1). */
  unknown6?: number;

  /** Gets or sets an unknown short value (nullable, only if isActive is 1). */
  unknown7?: number;

  /** Gets or sets an unknown byte value (nullable, only if isActive is 1). */
  unknown8?: number;

  /** Gets or sets whether the nation has a FIFA/UEFA ranking (nullable, only if isActive is 1). */
  isRanked?: number;

  /** Gets or sets the FIFA/UEFA ranking position (nullable, only if isActive is 1). */
  ranking?: number;

  /** Gets or sets the ranking points (nullable, only if isActive is 1). */
  points?: number;

  /** Gets or sets an unknown short value (nullable, only if isActive is 1). */
  unknown9?: number;

  /** Gets or sets the first set of coefficient values (only if isActive is 1). */
  coefficients1: number[] = [];

  /** Gets or sets unknown data (11 bytes, only if isActive is 1). */
  unknown10: Buffer = Buffer.alloc(0);

  /** Gets or sets whether the nation has a second coefficient set (1 = has coefficients, 0 = no coefficients). */
  hasCoefficient2: number = 0;

  /** Gets or sets unknown data (16 bytes, only if hasCoefficient2 is 1). */
  unknown11: Buffer = Buffer.alloc(0);

  /** Gets or sets an unknown byte value (nullable, only if hasCoefficient2 is 1). */
  unknown12?: number;

  /** Gets or sets an unknown short value (nullable, only if hasCoefficient2 is 1). */
  unknown13?: number;

  /** Gets or sets the second set of coefficient values (only if hasCoefficient2 is 1). */
  coefficients2: number[] = [];

  /** Gets or sets unknown data (11 bytes, only if hasCoefficient2 is 1). */
  unknown14: Buffer = Buffer.alloc(0);

  /**
   * Initializes a new instance of the Nation class by reading from a buffer.
   * @param buffer - The buffer containing the nation data
   * @param offset - The current offset in the buffer
   * @returns An object containing the Nation instance and the new offset
   */
  static fromBuffer(buffer: Buffer, offset: number): { nation: Nation; offset: number } {
    const nation = new Nation();

    nation.uid = buffer.readInt32LE(offset);
    offset += 4;

    nation.id = buffer.readInt16LE(offset);
    offset += 2;

    const nameResult = BinaryParser.readStringEx(buffer, offset);
    nation.name = nameResult.value;
    offset = nameResult.offset;

    nation.terminator1 = buffer.readUInt8(offset);
    offset += 1;

    const nationalityResult = BinaryParser.readStringEx(buffer, offset);
    nation.nationality = nationalityResult.value;
    offset = nationalityResult.offset;

    nation.terminator2 = buffer.readUInt8(offset);
    offset += 1;

    const codeNameResult = BinaryParser.readStringEx(buffer, offset);
    nation.codeName = codeNameResult.value;
    offset = codeNameResult.offset;

    nation.continentId = buffer.readInt16LE(offset);
    offset += 2;

    nation.cityId = buffer.readInt16LE(offset);
    offset += 2;

    nation.stadiumId = buffer.readInt16LE(offset);
    offset += 2;

    nation.unknown1 = buffer.readInt32LE(offset);
    offset += 4;

    nation.unknown2 = buffer.readInt16LE(offset);
    offset += 2;

    nation.unknown3 = buffer.readUInt8(offset);
    offset += 1;

    const languagesCount = buffer.readUInt8(offset);
    offset += 1;

    nation.languages = [];
    for (let i = 0; i < languagesCount; i++) {
      const languageId = buffer.readInt16LE(offset);
      offset += 2;
      const proficiency = buffer.readUInt8(offset);
      offset += 1;
      nation.languages.push({ languageId, proficiency });
    }

    nation.isActive = buffer.readUInt8(offset);
    offset += 1;

    if (nation.isActive === 1) {
      nation.color1 = buffer.readInt16LE(offset);
      offset += 2;

      nation.unknown5 = buffer.readInt32LE(offset);
      offset += 4;

      nation.color2 = buffer.readInt16LE(offset);
      offset += 2;

      nation.unknown6 = buffer.readUInt8(offset);
      offset += 1;

      nation.unknown7 = buffer.readInt16LE(offset);
      offset += 2;

      nation.unknown8 = buffer.readUInt8(offset);
      offset += 1;

      nation.isRanked = buffer.readUInt8(offset);
      offset += 1;

      nation.ranking = buffer.readInt16LE(offset);
      offset += 2;

      nation.points = buffer.readInt16LE(offset);
      offset += 2;

      nation.unknown9 = buffer.readInt16LE(offset);
      offset += 2;

      const coefficients1Count = buffer.readUInt8(offset);
      offset += 1;

      nation.coefficients1 = [];
      for (let i = 0; i < coefficients1Count; i++) {
        nation.coefficients1.push(buffer.readFloatLE(offset));
        offset += 4;
      }

      nation.unknown10 = buffer.slice(offset, offset + 11);
      offset += 11;
    }

    nation.hasCoefficient2 = buffer.readUInt8(offset);
    offset += 1;

    if (nation.hasCoefficient2 === 1) {
      nation.unknown11 = buffer.slice(offset, offset + 16);
      offset += 16;

      nation.unknown12 = buffer.readUInt8(offset);
      offset += 1;

      nation.unknown13 = buffer.readInt16LE(offset);
      offset += 2;

      const coefficients2Count = buffer.readUInt8(offset);
      offset += 1;

      nation.coefficients2 = [];
      for (let i = 0; i < coefficients2Count; i++) {
        nation.coefficients2.push(buffer.readFloatLE(offset));
        offset += 4;
      }

      nation.unknown14 = buffer.slice(offset, offset + 11);
      offset += 11;
    }

    return { nation, offset };
  }

  /**
   * Converts the nation data to a buffer.
   * @returns A buffer representing the serialized nation data
   */
  toBuffer(): Buffer {
    let size = 4 + 2; // uid + id
    size += 4 + Buffer.byteLength(this.name, 'utf-8'); // name
    size += 1; // terminator1
    size += 4 + Buffer.byteLength(this.nationality, 'utf-8'); // nationality
    size += 1; // terminator2
    size += 4 + Buffer.byteLength(this.codeName, 'utf-8'); // codeName
    size += 2 + 2 + 2; // continentId, cityId, stadiumId
    size += 4 + 2 + 1; // unknown1, unknown2, unknown3
    size += 1 + (this.languages.length * 3); // languages
    size += 1; // isActive

    if (this.isActive === 1) {
      size += 2 + 4 + 2 + 1 + 2 + 1 + 1 + 2 + 2 + 2; // colors and unknowns
      size += 1 + (this.coefficients1.length * 4); // coefficients1
      size += 11; // unknown10
    }

    size += 1; // hasCoefficient2

    if (this.hasCoefficient2 === 1) {
      size += 16 + 1 + 2; // unknown11, unknown12, unknown13
      size += 1 + (this.coefficients2.length * 4); // coefficients2
      size += 11; // unknown14
    }

    const buffer = Buffer.alloc(size);
    let offset = 0;

    buffer.writeInt32LE(this.uid, offset);
    offset += 4;

    buffer.writeInt16LE(this.id, offset);
    offset += 2;

    offset = BinaryParser.writeEx(this.name, buffer, offset);
    offset = BinaryParser.writeByte(this.terminator1, buffer, offset);
    offset = BinaryParser.writeEx(this.nationality, buffer, offset);
    offset = BinaryParser.writeByte(this.terminator2, buffer, offset);
    offset = BinaryParser.writeEx(this.codeName, buffer, offset);

    buffer.writeInt16LE(this.continentId, offset);
    offset += 2;

    buffer.writeInt16LE(this.cityId, offset);
    offset += 2;

    buffer.writeInt16LE(this.stadiumId, offset);
    offset += 2;

    buffer.writeInt32LE(this.unknown1, offset);
    offset += 4;

    buffer.writeInt16LE(this.unknown2, offset);
    offset += 2;

    buffer.writeUInt8(this.unknown3, offset);
    offset += 1;

    buffer.writeUInt8(this.languages.length, offset);
    offset += 1;

    for (const lang of this.languages) {
      buffer.writeInt16LE(lang.languageId, offset);
      offset += 2;
      buffer.writeUInt8(lang.proficiency, offset);
      offset += 1;
    }

    buffer.writeUInt8(this.isActive, offset);
    offset += 1;

    if (this.isActive === 1) {
      buffer.writeInt16LE(this.color1!, offset);
      offset += 2;

      buffer.writeInt32LE(this.unknown5!, offset);
      offset += 4;

      buffer.writeInt16LE(this.color2!, offset);
      offset += 2;

      buffer.writeUInt8(this.unknown6!, offset);
      offset += 1;

      buffer.writeInt16LE(this.unknown7!, offset);
      offset += 2;

      buffer.writeUInt8(this.unknown8!, offset);
      offset += 1;

      buffer.writeUInt8(this.isRanked!, offset);
      offset += 1;

      buffer.writeInt16LE(this.ranking!, offset);
      offset += 2;

      buffer.writeInt16LE(this.points!, offset);
      offset += 2;

      buffer.writeInt16LE(this.unknown9!, offset);
      offset += 2;

      buffer.writeUInt8(this.coefficients1.length, offset);
      offset += 1;

      for (const coef of this.coefficients1) {
        buffer.writeFloatLE(coef, offset);
        offset += 4;
      }

      this.unknown10.copy(buffer, offset);
      offset += 11;
    }

    buffer.writeUInt8(this.hasCoefficient2, offset);
    offset += 1;

    if (this.hasCoefficient2 === 1) {
      this.unknown11.copy(buffer, offset);
      offset += 16;

      buffer.writeUInt8(this.unknown12!, offset);
      offset += 1;

      buffer.writeInt16LE(this.unknown13!, offset);
      offset += 2;

      buffer.writeUInt8(this.coefficients2.length, offset);
      offset += 1;

      for (const coef of this.coefficients2) {
        buffer.writeFloatLE(coef, offset);
        offset += 4;
      }

      this.unknown14.copy(buffer, offset);
      offset += 11;
    }

    return buffer.slice(0, offset);
  }
}
