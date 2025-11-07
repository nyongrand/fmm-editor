import { BinaryConverter } from './binaryConverter';

// Base class for all serializable entities
export abstract class Serializable {
  abstract toBytes(): Uint8Array;
}

// Nation class matching the original format
export class Nation extends Serializable {
  uid: number = 0;
  id: number = 0;
  name: string = '';
  terminator1: number = 0;
  nationality: string = '';
  terminator2: number = 0;
  codeName: string = '';
  continentId: number = 0;
  cityId: number = 0;
  stadiumId: number = 0;
  unknown1: number = 0;
  unknown2: number = 0;
  unknown3: number = 0;
  languages: [number, number][] = []; // Array of (short, byte) tuples
  isActive: boolean = false;
  color1: number | null = null;
  unknown5: number | null = null;
  color2: number | null = null;
  unknown6: number | null = null;
  unknown7: number | null = null;
  unknown8: number | null = null;
  isRanked: boolean | null = null;
  ranking: number | null = null;
  points: number | null = null;
  unknown9: number | null = null;
  coefficients1: number[] = [];
  unknown10: number[] = [];
  hasCoefficient2: boolean = false;
  unknown11: number[] = [];
  unknown12: number | null = null;
  unknown13: number | null = null;
  coefficients2: number[] = [];
  unknown14: number[] = [];

  constructor(data: Partial<Nation> = {}) {
    super();
    Object.assign(this, data);
  }

  toBytes(): Uint8Array {
    const bytes = [
      BinaryConverter.numberToBytes(this.uid, 4),
      BinaryConverter.numberToBytes(this.id, 2),
      BinaryConverter.stringToBytesNoNull(this.name),
      BinaryConverter.numberToBytes(this.terminator1, 1),
      BinaryConverter.stringToBytesNoNull(this.nationality),
      BinaryConverter.numberToBytes(this.terminator2, 1),
      BinaryConverter.stringToBytesNoNull(this.codeName),
      BinaryConverter.numberToBytes(this.continentId, 2),
      BinaryConverter.numberToBytes(this.cityId, 2),
      BinaryConverter.numberToBytes(this.stadiumId, 2),
      BinaryConverter.numberToBytes(this.unknown1, 4),
      BinaryConverter.numberToBytes(this.unknown2, 2),
      BinaryConverter.numberToBytes(this.unknown3, 1),
      BinaryConverter.numberToBytes(this.languages.length, 1),
      ...this.languages.map(([langId, proficiency]) => [
        BinaryConverter.numberToBytes(langId, 2),
        BinaryConverter.numberToBytes(proficiency, 1)
      ]).flat(),
      BinaryConverter.booleanToByte(this.isActive),
    ];

    if (this.isActive) {
      if (this.color1 !== null) bytes.push(BinaryConverter.numberToBytes(this.color1, 2));
      if (this.unknown5 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown5, 4));
      if (this.color2 !== null) bytes.push(BinaryConverter.numberToBytes(this.color2, 2));
      if (this.unknown6 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown6, 1));
      if (this.unknown7 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown7, 2));
      if (this.unknown8 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown8, 1));
      if (this.isRanked !== null) bytes.push(BinaryConverter.booleanToByte(this.isRanked));
      if (this.ranking !== null) bytes.push(BinaryConverter.numberToBytes(this.ranking, 2));
      if (this.points !== null) bytes.push(BinaryConverter.numberToBytes(this.points, 2));
      if (this.unknown9 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown9, 2));
      
      bytes.push(BinaryConverter.numberToBytes(this.coefficients1.length, 1));
      for (const coeff of this.coefficients1) {
        bytes.push(BinaryConverter.floatToBytes(coeff));
      }
      
      bytes.push(new Uint8Array(this.unknown10));
    }

    bytes.push(BinaryConverter.booleanToByte(this.hasCoefficient2));
    
    if (this.hasCoefficient2) {
      bytes.push(new Uint8Array(this.unknown11));
      if (this.unknown12 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown12, 1));
      if (this.unknown13 !== null) bytes.push(BinaryConverter.numberToBytes(this.unknown13, 2));
      
      bytes.push(BinaryConverter.numberToBytes(this.coefficients2.length, 1));
      for (const coeff of this.coefficients2) {
        bytes.push(BinaryConverter.floatToBytes(coeff));
      }
      
      bytes.push(new Uint8Array(this.unknown14));
    }

    return BinaryConverter.combineBytes(...bytes);
  }
}

// Club class matching the original format
export class Club extends Serializable {
  id: number = 0;
  uid: number = 0;
  fullName: string = '';
  unknown0: number = 0;
  shortName: string = '';
  unknown1: number = 0;
  codeName1: string = '';
  codeName2: string = '';
  basedId: number = 0;
  nationId: number = 0;
  colors: number[] = Array(6).fill(0);
  status: number = 0;
  academy: number = 0;
  facilities: number = 0;
  attAvg: number = 0;
  attMin: number = 0;
  attMax: number = 0;
  reserves: number = 0;
  leagueId: number = 0;
  unknown2: number = 0;
  unknown3: number = 0;
  stadium: number = 0;
  lastLeague: number = 0;
  unknown4Flag: boolean = false;
  unknown4: number[] = [];
  unknown5: number[] = [];
  leaguePos: number = 0;
  reputation: number = 0;
  unknown6: number[] = Array(20).fill(0);
  affiliates: any[] = []; // Would need Affiliate class if fully implemented
  players: number[] = [];
  unknown7: number[] = Array(11).fill(0);
  mainClub: number = 0;
  isNational: number = 0;
  unknown8: number[] = Array(33).fill(0);
  unknown9: number[] = Array(40).fill(0);
  unknown10: number[] = Array(2).fill(0);

  constructor(data: Partial<Club> = {}) {
    super();
    Object.assign(this, data);
  }

  toBytes(): Uint8Array {
    // Combine all the club data into a byte array following the original format
    const bytes = [
      BinaryConverter.numberToBytes(this.id, 4),
      BinaryConverter.numberToBytes(this.uid, 4),
      BinaryConverter.stringToBytes(this.fullName),
      BinaryConverter.numberToBytes(this.unknown0, 1),
      BinaryConverter.stringToBytes(this.shortName),
      BinaryConverter.numberToBytes(this.unknown1, 1),
      BinaryConverter.stringToBytes(this.codeName1),
      BinaryConverter.stringToBytes(this.codeName2),
      BinaryConverter.numberToBytes(this.basedId, 2),
      BinaryConverter.numberToBytes(this.nationId, 2),
      ...this.colors.map(color => BinaryConverter.numberToBytes(color, 2)), // 6 colors as shorts
      // Assuming a simplified kit structure as the original is complex
      // In a real implementation, we'd serialize kit data here
      BinaryConverter.numberToBytes(this.status, 1),
      BinaryConverter.numberToBytes(this.academy, 1),
      BinaryConverter.numberToBytes(this.facilities, 1),
      BinaryConverter.numberToBytes(this.attAvg, 2),
      BinaryConverter.numberToBytes(this.attMin, 2),
      BinaryConverter.numberToBytes(this.attMax, 2),
      BinaryConverter.numberToBytes(this.reserves, 1),
      BinaryConverter.numberToBytes(this.leagueId, 2),
      BinaryConverter.numberToBytes(this.unknown2, 2),
      BinaryConverter.numberToBytes(this.unknown3, 1),
      BinaryConverter.numberToBytes(this.stadium, 2),
      BinaryConverter.numberToBytes(this.lastLeague, 2),
      BinaryConverter.booleanToByte(this.unknown4Flag),
      ...(this.unknown4Flag ? [new Uint8Array(this.unknown4)] : []),
      BinaryConverter.numberToBytes(this.unknown5.length, 4),
      new Uint8Array(this.unknown5),
      BinaryConverter.numberToBytes(this.leaguePos, 1),
      BinaryConverter.numberToBytes(this.reputation, 2),
      new Uint8Array(this.unknown6), // 20 bytes
      BinaryConverter.numberToBytes(this.affiliates.length, 2), // affiliate count
      // Affiliates serialization would go here
      BinaryConverter.numberToBytes(this.players.length, 2), // player count
      ...this.players.map(playerId => BinaryConverter.numberToBytes(playerId, 4)),
      ...this.unknown7.map(unk => BinaryConverter.numberToBytes(unk, 4)), // 11 unknown ints
      BinaryConverter.numberToBytes(this.mainClub, 4),
      BinaryConverter.numberToBytes(this.isNational, 2),
      new Uint8Array(this.unknown8), // 33 bytes
      new Uint8Array(this.unknown9), // 40 bytes
      new Uint8Array(this.unknown10), // 2 bytes
    ];

    return BinaryConverter.combineBytes(...bytes);
  }
}

// Competition class matching the original format
export class Competition extends Serializable {
  id: number = 0;
  uid: number = 0;
  fullName: string = '';
  unknown1: number = 0;
  shortName: string = '';
  unknown2: number = 0;
  codeName: string = '';
  type: number = 0;
  continentId: number = 0;
  nationId: number = 0;
  color1: number = 0;
  color2: number = 0;
  reputation: number = 0;
  level: number = 0;
  mainComp: number = 0;
  qualifiers: number[][] = [];
  rank1: number = 0;
  rank2: number = 0;
  rank3: number = 0;
  year1: number = 0;
  year2: number = 0;
  year3: number = 0;
  unknown3: number = 0;
  unknown4: number = 0;

  constructor(data: Partial<Competition> = {}) {
    super();
    Object.assign(this, data);
  }

  toBytes(): Uint8Array {
    const bytes = [
      BinaryConverter.numberToBytes(this.id, 2),
      BinaryConverter.numberToBytes(this.uid, 4),
      BinaryConverter.stringToBytes(this.fullName),
      BinaryConverter.numberToBytes(this.unknown1, 1),
      BinaryConverter.stringToBytes(this.shortName),
      BinaryConverter.numberToBytes(this.unknown2, 1),
      BinaryConverter.stringToBytes(this.codeName),
      BinaryConverter.numberToBytes(this.type, 1),
      BinaryConverter.numberToBytes(this.continentId, 2),
      BinaryConverter.numberToBytes(this.nationId, 2),
      BinaryConverter.numberToBytes(this.color1, 2),
      BinaryConverter.numberToBytes(this.color2, 2),
      BinaryConverter.numberToBytes(this.reputation, 2),
      BinaryConverter.numberToBytes(this.level, 1),
      BinaryConverter.numberToBytes(this.mainComp, 2),
      BinaryConverter.numberToBytes(this.qualifiers.length, 4),
      ...this.qualifiers.map(qual => new Uint8Array(qual)), // Each qualifier is 8 bytes
      BinaryConverter.numberToBytes(this.rank1, 4),
      BinaryConverter.numberToBytes(this.rank2, 4),
      BinaryConverter.numberToBytes(this.rank3, 4),
      BinaryConverter.numberToBytes(this.year1, 2),
      BinaryConverter.numberToBytes(this.year2, 2),
      BinaryConverter.numberToBytes(this.year3, 2),
      BinaryConverter.numberToBytes(this.unknown3, 1),
      BinaryConverter.numberToBytes(this.unknown4, 1),
    ];

    return BinaryConverter.combineBytes(...bytes);
  }
}

// Name class matching the original format
export class Name extends Serializable {
  id: number = 0;
  nation: string = ''; // Hex string
  others: string = ''; // Hex string
  value: string = '';

  constructor(data: Partial<Name> = {}) {
    super();
    Object.assign(this, data);
  }

  toBytes(): Uint8Array {
    // The Name format is: 4 bytes of zeros, then ID (4 bytes), then nation (4 bytes as hex), 
    // others (4 bytes as hex), length of name (4 bytes), then the name bytes
    const nameBytes = new TextEncoder().encode(this.value);
    
    const bytes = [
      new Uint8Array(4), // 4 bytes of zeros
      BinaryConverter.numberToBytes(this.id, 4),
      new Uint8Array(this.nation.match(/.{1,2}/g)?.map(byte => parseInt(byte, 16)) || []),
      new Uint8Array(this.others.match(/.{1,2}/g)?.map(byte => parseInt(byte, 16)) || []),
      BinaryConverter.numberToBytes(nameBytes.length, 4),
      new Uint8Array(nameBytes),
    ];

    return BinaryConverter.combineBytes(...bytes);
  }
}