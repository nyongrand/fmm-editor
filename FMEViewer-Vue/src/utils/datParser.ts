import { BinaryConverter } from './binaryConverter';
import { Nation, Club, Competition, Name } from './datSerializer';

export class DatParser {
  // Parse a nation.dat file
  static parseNations(buffer: ArrayBuffer): Nation[] {
    const view = new DataView(buffer);
    const result: Nation[] = [];
    
    // The first 4 bytes should be the count
    const count = view.getInt32(0, true); // little-endian
    let offset = 4;
    
    for (let i = 0; i < count; i++) {
      try {
        const { nation, parseOffset } = this.parseSingleNation(view, offset);
        result.push(nation);
        offset = parseOffset; // Use offset from the parsed nation
      } catch (e) {
        console.error(`Error parsing nation at index ${i}:`, e);
        break; // Stop parsing if we encounter an error
      }
    }
    
    return result;
  }

  // Helper function to parse a single nation from buffer
  private static parseSingleNation(view: DataView, startOffset: number): {nation: Nation, parseOffset: number} {
    let offset = startOffset;
    
    // Read fields based on the original Nation structure
    const uid = view.getInt32(offset, true); offset += 4;
    const id = view.getInt16(offset, true); offset += 2;
    
    // Read name string (null-terminated)
    const { value: name, newOffset: nameEndOffset } = this.readString(view, offset);
    offset = nameEndOffset;
    
    const terminator1 = view.getUint8(offset); offset += 1;
    
    // Read nationality string (null-terminated)
    const { value: nationality, newOffset: nationalityEndOffset } = this.readString(view, offset);
    offset = nationalityEndOffset;
    
    const terminator2 = view.getUint8(offset); offset += 1;
    
    // Read code name string (null-terminated)
    const { value: codeName, newOffset: codeNameEndOffset } = this.readString(view, offset);
    offset = codeNameEndOffset;
    
    const continentId = view.getInt16(offset, true); offset += 2;
    const cityId = view.getInt16(offset, true); offset += 2;
    const stadiumId = view.getInt16(offset, true); offset += 2;
    
    const unknown1 = view.getInt32(offset, true); offset += 4;
    const unknown2 = view.getInt16(offset, true); offset += 2;
    const unknown3 = view.getUint8(offset); offset += 1;
    
    // Read languages array (byte count, then (short, byte) pairs)
    const languageCount = view.getUint8(offset); offset += 1;
    const languages: [number, number][] = [];
    for (let i = 0; i < languageCount; i++) {
      const langId = view.getInt16(offset, true); offset += 2;
      const proficiency = view.getUint8(offset); offset += 1;
      languages.push([langId, proficiency]);
    }
    
    const isActive = view.getUint8(offset) !== 0; offset += 1;
    
    // If active, read additional fields
    let color1: number | null = null, unknown5: number | null = null, color2: number | null = null;
    let unknown6: number | null = null, unknown7: number | null = null, unknown8: number | null = null;
    let isRanked: boolean | null = null, ranking: number | null = null, points: number | null = null;
    let unknown9: number | null = null;
    let coefficients1: number[] = [], unknown10: number[] = [];
    
    if (isActive) {
      color1 = view.getInt16(offset, true); offset += 2;
      unknown5 = view.getInt32(offset, true); offset += 4;
      color2 = view.getInt16(offset, true); offset += 2;
      
      unknown6 = view.getUint8(offset); offset += 1;
      unknown7 = view.getInt16(offset, true); offset += 2;
      unknown8 = view.getUint8(offset); offset += 1;
      
      isRanked = view.getUint8(offset) !== 0; offset += 1;
      ranking = view.getInt16(offset, true); offset += 2;
      points = view.getInt16(offset, true); offset += 2;
      unknown9 = view.getInt16(offset, true); offset += 2;
      
      const coeffCount = view.getUint8(offset); offset += 1;
      coefficients1 = [];
      for (let i = 0; i < coeffCount; i++) {
        coefficients1.push(view.getFloat32(offset, true));
        offset += 4;
      }
      
      // Read unknown10 (11 bytes)
      unknown10 = [];
      for (let i = 0; i < 11; i++) {
        unknown10.push(view.getUint8(offset));
        offset += 1;
      }
    }
    
    // Read hasCoefficient2 flag
    const hasCoefficient2 = view.getUint8(offset) !== 0; offset += 1;
    
    let unknown11: number[] = [], unknown12: number | null = null, unknown13: number | null = null;
    let coefficients2: number[] = [], unknown14: number[] = [];
    
    if (hasCoefficient2) {
      // Read unknown11 (16 bytes)
      unknown11 = [];
      for (let i = 0; i < 16; i++) {
        unknown11.push(view.getUint8(offset));
        offset += 1;
      }
      
      unknown12 = view.getUint8(offset); offset += 1;
      unknown13 = view.getInt16(offset, true); offset += 2;
      
      const coeff2Count = view.getUint8(offset); offset += 1;
      coefficients2 = [];
      for (let i = 0; i < coeff2Count; i++) {
        coefficients2.push(view.getFloat32(offset, true));
        offset += 4;
      }
      
      // Read unknown14 (11 bytes)
      unknown14 = [];
      for (let i = 0; i < 11; i++) {
        unknown14.push(view.getUint8(offset));
        offset += 1;
      }
    }
    
    const nation = new Nation({
      uid,
      id,
      name,
      terminator1,
      nationality,
      terminator2,
      codeName,
      continentId,
      cityId,
      stadiumId,
      unknown1,
      unknown2,
      unknown3,
      languages,
      isActive,
      color1,
      unknown5,
      color2,
      unknown6,
      unknown7,
      unknown8,
      isRanked,
      ranking,
      points,
      unknown9,
      coefficients1,
      unknown10,
      hasCoefficient2,
      unknown11,
      unknown12,
      unknown13,
      coefficients2,
      unknown14,
    });
    
    // We need to return the offset as well
    (nation as any).parseOffset = offset;
    
    return { nation, parseOffset: offset };
  }

  // Parse a club.dat file
  static parseClubs(buffer: ArrayBuffer): Club[] {
    const view = new DataView(buffer);
    const result: Club[] = [];
    
    // The first 4 bytes should be the count
    const count = view.getInt32(0, true); // little-endian
    let offset = 4;
    
    for (let i = 0; i < count; i++) {
      try {
        const { club, parseOffset } = this.parseSingleClub(view, offset);
        result.push(club);
        offset = parseOffset; // Use offset from the parsed club
      } catch (e) {
        console.error(`Error parsing club at index ${i}:`, e);
        break; // Stop parsing if we encounter an error
      }
    }
    
    return result;
  }

  // Helper function to parse a single club
  private static parseSingleClub(view: DataView, startOffset: number): {club: Club, parseOffset: number} {
    let offset = startOffset;
    
    // Read club fields based on the original structure
    const id = view.getInt32(offset, true); offset += 4;
    const uid = view.getInt32(offset, true); offset += 4;
    
    // Read strings (null-terminated)
    const { value: fullName, newOffset: fullNameEnd } = this.readString(view, offset);
    offset = fullNameEnd;
    
    const unknown0 = view.getUint8(offset); offset += 1;
    
    const { value: shortName, newOffset: shortNameEnd } = this.readString(view, offset);
    offset = shortNameEnd;
    
    const unknown1 = view.getUint8(offset); offset += 1;
    
    const { value: codeName1, newOffset: codeName1End } = this.readString(view, offset);
    offset = codeName1End;
    
    const { value: codeName2, newOffset: codeName2End } = this.readString(view, offset);
    offset = codeName2End;
    
    const basedId = view.getInt16(offset, true); offset += 2;
    const nationId = view.getInt16(offset, true); offset += 2;
    
    // Read 6 colors
    const colors: number[] = [];
    for (let i = 0; i < 6; i++) {
      colors.push(view.getInt16(offset, true));
      offset += 2;
    }
    
    // Simplified kit parsing (would be more complex in real implementation)
    // Skip 6 kit structures for now
    
    const status = view.getUint8(offset); offset += 1;
    const academy = view.getUint8(offset); offset += 1;
    const facilities = view.getUint8(offset); offset += 1;
    const attAvg = view.getInt16(offset, true); offset += 2;
    const attMin = view.getInt16(offset, true); offset += 2;
    const attMax = view.getInt16(offset, true); offset += 2;
    const reserves = view.getUint8(offset); offset += 1;
    const leagueId = view.getInt16(offset, true); offset += 2;
    
    const unknown2 = view.getInt16(offset, true); offset += 2;
    const unknown3 = view.getUint8(offset); offset += 1;
    const stadium = view.getInt16(offset, true); offset += 2;
    const lastLeague = view.getInt16(offset, true); offset += 2;
    
    const unknown4Flag = view.getUint8(offset) !== 0; offset += 1;
    
    let unknown4: number[] = [];
    if (unknown4Flag) {
      // Read the 68 bytes for unknown4
      unknown4 = [];
      for (let i = 0; i < 68; i++) {
        unknown4.push(view.getUint8(offset));
        offset += 1;
      }
    }
    
    // Read length of unknown5 then the array
    const unknown5Length = view.getInt32(offset, true); offset += 4;
    const unknown5: number[] = [];
    for (let i = 0; i < unknown5Length; i++) {
      unknown5.push(view.getUint8(offset));
      offset += 1;
    }
    
    const leaguePos = view.getUint8(offset); offset += 1;
    const reputation = view.getInt16(offset, true); offset += 2;
    
    // Read unknown6 (20 bytes)
    const unknown6: number[] = [];
    for (let i = 0; i < 20; i++) {
      unknown6.push(view.getUint8(offset));
      offset += 1;
    }
    
    // Read affiliates
    const affiliateCount = view.getInt16(offset, true); offset += 2;
    const affiliates: any[] = [];
    // Parse affiliate data would go here
    
    // Read players
    const playerCount = view.getInt16(offset, true); offset += 2;
    const players: number[] = [];
    for (let i = 0; i < playerCount; i++) {
      players.push(view.getInt32(offset, true));
      offset += 4;
    }
    
    // Read unknown7 (11 integers)
    const unknown7: number[] = [];
    for (let i = 0; i < 11; i++) {
      unknown7.push(view.getInt32(offset, true));
      offset += 4;
    }
    
    const mainClub = view.getInt32(offset, true); offset += 4;
    const isNational = view.getInt16(offset, true); offset += 2;
    
    // Read unknown8 (33 bytes)
    const unknown8: number[] = [];
    for (let i = 0; i < 33; i++) {
      unknown8.push(view.getUint8(offset));
      offset += 1;
    }
    
    // Read unknown9 (40 bytes)
    const unknown9: number[] = [];
    for (let i = 0; i < 40; i++) {
      unknown9.push(view.getUint8(offset));
      offset += 1;
    }
    
    // Read unknown10 (2 bytes)
    const unknown10: number[] = [];
    for (let i = 0; i < 2; i++) {
      unknown10.push(view.getUint8(offset));
      offset += 1;
    }
    
    const club = new Club({
      id,
      uid,
      fullName,
      unknown0,
      shortName,
      unknown1,
      codeName1,
      codeName2,
      basedId,
      nationId,
      colors,
      status,
      academy,
      facilities,
      attAvg,
      attMin,
      attMax,
      reserves,
      leagueId,
      unknown2,
      unknown3,
      stadium,
      lastLeague,
      unknown4Flag,
      unknown4,
      unknown5,
      leaguePos,
      reputation,
      unknown6,
      affiliates,
      players,
      unknown7,
      mainClub,
      isNational,
      unknown8,
      unknown9,
      unknown10,
    });
    
    // We need to return the offset as well
    (club as any).parseOffset = offset;
    
    return { club, parseOffset: offset };
  }

  // Parse a competition.dat file
  static parseCompetitions(buffer: ArrayBuffer): Competition[] {
    const view = new DataView(buffer);
    const result: Competition[] = [];
    
    // The first 2 bytes should be the count (short)
    const count = view.getInt16(0, true); // little-endian
    let offset = 2;
    
    for (let i = 0; i < count; i++) {
      try {
        const { competition, parseOffset } = this.parseSingleCompetition(view, offset);
        result.push(competition);
        offset = parseOffset; // Use offset from the parsed competition
      } catch (e) {
        console.error(`Error parsing competition at index ${i}:`, e);
        break; // Stop parsing if we encounter an error
      }
    }
    
    return result;
  }

  // Helper function to parse a single competition
  private static parseSingleCompetition(view: DataView, startOffset: number): {competition: Competition, parseOffset: number} {
    let offset = startOffset;
    
    const id = view.getInt16(offset, true); offset += 2;
    const uid = view.getInt32(offset, true); offset += 4;
    
    // Read strings (null-terminated)
    const { value: fullName, newOffset: fullNameEnd } = this.readString(view, offset);
    offset = fullNameEnd;
    
    const unknown1 = view.getUint8(offset); offset += 1;
    
    const { value: shortName, newOffset: shortNameEnd } = this.readString(view, offset);
    offset = shortNameEnd;
    
    const unknown2 = view.getUint8(offset); offset += 1;
    
    const { value: codeName, newOffset: codeNameEnd } = this.readString(view, offset);
    offset = codeNameEnd;
    
    const type = view.getUint8(offset); offset += 1;
    const continentId = view.getInt16(offset, true); offset += 2;
    const nationId = view.getInt16(offset, true); offset += 2;
    const color1 = view.getInt16(offset, true); offset += 2;
    const color2 = view.getInt16(offset, true); offset += 2;
    const reputation = view.getInt16(offset, true); offset += 2;
    const level = view.getUint8(offset); offset += 1;
    const mainComp = view.getInt16(offset, true); offset += 2;
    
    // Read qualifiers array
    const qualifierCount = view.getInt32(offset, true); offset += 4;
    const qualifiers: number[][] = [];
    for (let i = 0; i < qualifierCount; i++) {
      const qualifier: number[] = [];
      for (let j = 0; j < 8; j++) {  // Each qualifier is 8 bytes
        qualifier.push(view.getUint8(offset));
        offset += 1;
      }
      qualifiers.push(qualifier);
    }
    
    const rank1 = view.getInt32(offset, true); offset += 4;
    const rank2 = view.getInt32(offset, true); offset += 4;
    const rank3 = view.getInt32(offset, true); offset += 4;
    const year1 = view.getInt16(offset, true); offset += 2;
    const year2 = view.getInt16(offset, true); offset += 2;
    const year3 = view.getInt16(offset, true); offset += 2;
    
    const unknown3 = view.getUint8(offset); offset += 1;
    const unknown4 = view.getUint8(offset); offset += 1;
    
    const competition = new Competition({
      id,
      uid,
      fullName,
      unknown1,
      shortName,
      unknown2,
      codeName,
      type,
      continentId,
      nationId,
      color1,
      color2,
      reputation,
      level,
      mainComp,
      qualifiers,
      rank1,
      rank2,
      rank3,
      year1,
      year2,
      year3,
      unknown3,
      unknown4,
    });
    
    // We need to return the offset as well
    (competition as any).parseOffset = offset;
    
    return { competition, parseOffset: offset };
  }

  // Parse a name.dat file
  static parseNames(buffer: ArrayBuffer): Name[] {
    const view = new DataView(buffer);
    const result: Name[] = [];
    
    // The first 4 bytes are zeros, next 4 are count
    const count = view.getInt32(4, true); // little-endian, after 4 zero bytes
    let offset = 8; // Start after the zeros and count
    
    for (let i = 0; i < count; i++) {
      try {
        const { name, parseOffset } = this.parseSingleName(view, offset);
        result.push(name);
        offset = parseOffset; // Use offset from the parsed name
      } catch (e) {
        console.error(`Error parsing name at index ${i}:`, e);
        break; // Stop parsing if we encounter an error
      }
    }
    
    return result;
  }

  // Helper function to parse a single name
  private static parseSingleName(view: DataView, startOffset: number): {name: Name, parseOffset: number} {
    let offset = startOffset;
    
    // Skip 4 bytes of zeros
    offset += 4;
    
    const id = view.getInt32(offset, true); offset += 4;
    
    // Read 4 bytes for nation (as hex)
    const nationBytes: number[] = [];
    for (let i = 0; i < 4; i++) {
      nationBytes.push(view.getUint8(offset));
      offset += 1;
    }
    const nation = nationBytes.map(b => b.toString(16).padStart(2, '0')).join('');
    
    // Read 4 bytes for others (as hex)
    const othersBytes: number[] = [];
    for (let i = 0; i < 4; i++) {
      othersBytes.push(view.getUint8(offset));
      offset += 1;
    }
    const others = othersBytes.map(b => b.toString(16).padStart(2, '0')).join('');
    
    // Read length of name
    const nameLength = view.getInt32(offset, true); offset += 4;
    
    // Read name string
    const nameBytes = new Uint8Array(view.buffer, view.byteOffset + offset, nameLength);
    const decoder = new TextDecoder();
    const value = decoder.decode(nameBytes);
    offset += nameLength;
    
    const name = new Name({
      id,
      nation,
      others,
      value,
    });
    
    // We need to return the offset as well
    (name as any).parseOffset = offset;
    
    return { name, parseOffset: offset };
  }

  // Helper function to read null-terminated string from DataView
  private static readString(view: DataView, startOffset: number): { value: string, newOffset: number } {
    let offset = startOffset;
    const bytes: number[] = [];
    
    // Read until we hit a null terminator (0x00)
    while (offset < view.byteLength && view.getUint8(offset) !== 0) {
      bytes.push(view.getUint8(offset));
      offset++;
    }
    
    // Skip the null terminator
    if (offset < view.byteLength && view.getUint8(offset) === 0) {
      offset++;
    }
    
    // Convert bytes to string
    const decoder = new TextDecoder();
    const value = decoder.decode(new Uint8Array(bytes));
    
    return { value, newOffset: offset };
  }
}