import { BinaryParser } from './BinaryParser';
import { Kit } from './Kit';
import { Affiliate } from './Affiliate';

/**
 * Represents a football club with all its properties and attributes.
 */
export class Club {
    /** Gets or sets the club identifier. */
    id: number = 0;

    /** Gets or sets the unique identifier for the club. */
    uid: number = 0;

    /** Gets or sets the full name of the club. */
    fullName: string = '';

/** Gets or sets an unknown byte value. */
    unknown0: number = 0;

    /** Gets or sets the short name of the club. */
    shortName: string = '';

    /** Gets or sets an unknown byte value. */
    unknown1: number = 0;

    /** Gets or sets the first code name of the club. */
    codeName1: string = '';

    /** Gets or sets the second code name of the club. */
    codeName2: string = '';

    /** Gets or sets the identifier of the city where the club is based. */
    basedId: number = 0;

    /** Gets or sets the nation identifier of the club. */
    nationId: number = 0;

    /** Gets or sets the club's color values (6 colors). */
    colors: number[] = [];

    /** Gets or sets the club's kits (6 kits). */
    kits: Kit[] = [];

    /** Gets or sets the club's status. */
    status: number = 0;

    /** Gets or sets the academy rating. */
    academy: number = 0;

    /** Gets or sets the facilities rating. */
    facilities: number = 0;

    /** Gets or sets the average attendance. */
    attAvg: number = 0;

    /** Gets or sets the minimum attendance. */
    attMin: number = 0;

    /** Gets or sets the maximum attendance. */
    attMax: number = 0;

    /** Gets or sets the reserves status. */
    reserves: number = 0;

    /** Gets or sets the league identifier. */
    leagueId: number = 0;

    /** Gets or sets an unknown short value. */
  unknown2: number = 0;

    /** Gets or sets an unknown byte value. */
    unknown3: number = 0;

    /** Gets or sets the stadium identifier. */
    stadium: number = 0;

    /** Gets or sets the last league identifier. */
    lastLeague: number = 0;

  /** Gets or sets the type flag for unknown4 data. */
    unknown4Type: number = 0;

    /** Gets or sets conditional unknown data (68 bytes if unknown4Type is 1). */
    unknown4: Buffer = Buffer.alloc(0);

    /** Gets or sets variable-length unknown data. */
    unknown5: Buffer = Buffer.alloc(0);

    /** Gets or sets the current league position. */
    leaguePos: number = 0;

    /** Gets or sets the club's reputation. */
    reputation: number = 0;

    /** Gets or sets unknown data (20 bytes). */
    unknown6: Buffer = Buffer.alloc(0);

 /** Gets or sets the array of affiliated clubs. */
    affiliates: Affiliate[] = [];

    /** Gets or sets the array of player identifiers. */
    players: number[] = [];

    /** Gets or sets unknown data (11 integers). */
    unknown7: number[] = [];

    /** Gets or sets the main club identifier (for reserve/B teams). */
    mainClub: number = 0;

    /** Gets or sets whether this is a national team. */
    isNational: number = 0;

    /** Gets or sets unknown data (33 bytes). */
    unknown8: Buffer = Buffer.alloc(0);

    /** Gets or sets unknown data (40 bytes). */
    unknown9: Buffer = Buffer.alloc(0);

    /** Gets or sets unknown data (2 bytes). */
    unknown10: Buffer = Buffer.alloc(0);

    // Extra fields
    /** Gets or sets the name of the city where the club is based (extra/computed field). */
    based: string = '';

    /** Gets or sets the nation name (extra/computed field). */
    nation: string = '';

    /**
     * Initializes a new instance of the Club class by reading from a buffer.
     * @param buffer - The buffer containing the club data
     * @param offset - The current offset in the buffer
     * @returns An object containing the Club instance and the new offset
     */
    static fromBuffer(buffer: Buffer, offset: number): { club: Club; offset: number } {
        const club = new Club();

    club.id = buffer.readInt32LE(offset);
        offset += 4;

      club.uid = buffer.readInt32LE(offset);
        offset += 4;

  const fullNameResult = BinaryParser.readStringEx(buffer, offset);
        club.fullName = fullNameResult.value;
        offset = fullNameResult.offset;

   club.unknown0 = buffer.readUInt8(offset);
        offset += 1;

        const shortNameResult = BinaryParser.readStringEx(buffer, offset);
      club.shortName = shortNameResult.value;
   offset = shortNameResult.offset;

   club.unknown1 = buffer.readUInt8(offset);
      offset += 1;

  const codeName1Result = BinaryParser.readStringEx(buffer, offset);
        club.codeName1 = codeName1Result.value;
        offset = codeName1Result.offset;

      const codeName2Result = BinaryParser.readStringEx(buffer, offset);
      club.codeName2 = codeName2Result.value;
      offset = codeName2Result.offset;

        club.basedId = buffer.readInt16LE(offset);
        offset += 2;

        club.nationId = buffer.readInt16LE(offset);
        offset += 2;

        club.colors = [];
        for (let i = 0; i < 6; i++) {
   club.colors.push(buffer.readInt16LE(offset));
            offset += 2;
        }

        club.kits = [];
        for (let i = 0; i < 6; i++) {
         const kitResult = Kit.fromBuffer(buffer, offset);
 club.kits.push(kitResult.kit);
      offset = kitResult.offset;
        }

 club.status = buffer.readUInt8(offset);
        offset += 1;

        club.academy = buffer.readUInt8(offset);
      offset += 1;

      club.facilities = buffer.readUInt8(offset);
        offset += 1;

        club.attAvg = buffer.readInt16LE(offset);
        offset += 2;

     club.attMin = buffer.readInt16LE(offset);
        offset += 2;

        club.attMax = buffer.readInt16LE(offset);
        offset += 2;

        club.reserves = buffer.readUInt8(offset);
   offset += 1;

club.leagueId = buffer.readInt16LE(offset);
        offset += 2;

        club.unknown2 = buffer.readInt16LE(offset);
     offset += 2;

 club.unknown3 = buffer.readUInt8(offset);
        offset += 1;

        club.stadium = buffer.readInt16LE(offset);
        offset += 2;

club.lastLeague = buffer.readInt16LE(offset);
    offset += 2;

        club.unknown4Type = buffer.readUInt8(offset);
        offset += 1;

        if (club.unknown4Type === 1) {
            club.unknown4 = buffer.slice(offset, offset + 68);
      offset += 68;
        } else {
            club.unknown4 = Buffer.alloc(0);
        }

      const unknown5Length = buffer.readInt32LE(offset);
        offset += 4;

        club.unknown5 = buffer.slice(offset, offset + unknown5Length);
   offset += unknown5Length;

        club.leaguePos = buffer.readUInt8(offset);
        offset += 1;

     club.reputation = buffer.readInt16LE(offset);
        offset += 2;

      club.unknown6 = buffer.slice(offset, offset + 20);
   offset += 20;

const affiliatesCount = buffer.readInt16LE(offset);
        offset += 2;

        club.affiliates = [];
        for (let i = 0; i < affiliatesCount; i++) {
     const affiliateResult = Affiliate.fromBuffer(buffer, offset);
      club.affiliates.push(affiliateResult.affiliate);
  offset = affiliateResult.offset;
        }

        const playersCount = buffer.readInt16LE(offset);
        offset += 2;

        club.players = [];
        for (let i = 0; i < playersCount; i++) {
    club.players.push(buffer.readInt32LE(offset));
        offset += 4;
     }

        club.unknown7 = [];
        for (let i = 0; i < 11; i++) {
            club.unknown7.push(buffer.readInt32LE(offset));
   offset += 4;
        }

        club.mainClub = buffer.readInt32LE(offset);
        offset += 4;

   club.isNational = buffer.readInt16LE(offset);
        offset += 2;

        club.unknown8 = buffer.slice(offset, offset + 33);
        offset += 33;

    club.unknown9 = buffer.slice(offset, offset + 40);
        offset += 40;

  club.unknown10 = buffer.slice(offset, offset + 2);
        offset += 2;

        return { club, offset };
    }

    /**
  * Converts the club data to a buffer.
  * @returns A buffer representing the serialized club data
     */
    toBuffer(): Buffer {
        const buffers: Buffer[] = [];

        // Calculate approximate size
        let size = 4 + 4; // id + uid
        size += 4 + Buffer.byteLength(this.fullName, 'utf-8'); // fullName
        size += 1; // unknown0
        size += 4 + Buffer.byteLength(this.shortName, 'utf-8'); // shortName
    size += 1; // unknown1
        size += 4 + Buffer.byteLength(this.codeName1, 'utf-8'); // codeName1
    size += 4 + Buffer.byteLength(this.codeName2, 'utf-8'); // codeName2
      size += 2 + 2; // basedId + nationId
  size += 6 * 2; // colors
        size += 6 * Kit.getSize(); // kits
 size += 1 + 1 + 1 + 2 + 2 + 2 + 1 + 2; // status to leagueId
        size += 2 + 1 + 2 + 2; // unknown2 to lastLeague
        size += 1; // unknown4Type
  if (this.unknown4Type === 1) size += 68;
        size += 4 + this.unknown5.length;
  size += 1 + 2 + 20; // leaguePos, reputation, unknown6
        size += 2 + (this.affiliates.length * Affiliate.getSize());
        size += 2 + (this.players.length * 4);
   size += 11 * 4; // unknown7
        size += 4 + 2; // mainClub + isNational
        size += 33 + 40 + 2; // unknown8, unknown9, unknown10

        const buffer = Buffer.alloc(size);
        let offset = 0;

buffer.writeInt32LE(this.id, offset);
        offset += 4;

        buffer.writeInt32LE(this.uid, offset);
        offset += 4;

   offset = BinaryParser.writeEx(this.fullName, buffer, offset);
        offset = BinaryParser.writeByte(this.unknown0, buffer, offset);
        offset = BinaryParser.writeEx(this.shortName, buffer, offset);
        offset = BinaryParser.writeByte(this.unknown1, buffer, offset);
      offset = BinaryParser.writeEx(this.codeName1, buffer, offset);
  offset = BinaryParser.writeEx(this.codeName2, buffer, offset);

        buffer.writeInt16LE(this.basedId, offset);
        offset += 2;

        buffer.writeInt16LE(this.nationId, offset);
 offset += 2;

    for (const color of this.colors) {
 buffer.writeInt16LE(color, offset);
            offset += 2;
    }

        for (const kit of this.kits) {
   offset = kit.write(buffer, offset);
   }

        buffer.writeUInt8(this.status, offset);
        offset += 1;

        buffer.writeUInt8(this.academy, offset);
offset += 1;

        buffer.writeUInt8(this.facilities, offset);
        offset += 1;

        buffer.writeInt16LE(this.attAvg, offset);
        offset += 2;

    buffer.writeInt16LE(this.attMin, offset);
        offset += 2;

        buffer.writeInt16LE(this.attMax, offset);
        offset += 2;

buffer.writeUInt8(this.reserves, offset);
        offset += 1;

     buffer.writeInt16LE(this.leagueId, offset);
        offset += 2;

        buffer.writeInt16LE(this.unknown2, offset);
        offset += 2;

     buffer.writeUInt8(this.unknown3, offset);
        offset += 1;

    buffer.writeInt16LE(this.stadium, offset);
        offset += 2;

    buffer.writeInt16LE(this.lastLeague, offset);
        offset += 2;

        buffer.writeUInt8(this.unknown4Type, offset);
        offset += 1;

      if (this.unknown4Type === 1) {
 this.unknown4.copy(buffer, offset);
            offset += 68;
      }

   buffer.writeInt32LE(this.unknown5.length, offset);
        offset += 4;

  this.unknown5.copy(buffer, offset);
        offset += this.unknown5.length;

 buffer.writeUInt8(this.leaguePos, offset);
        offset += 1;

        buffer.writeInt16LE(this.reputation, offset);
        offset += 2;

        this.unknown6.copy(buffer, offset);
        offset += 20;

        buffer.writeInt16LE(this.affiliates.length, offset);
        offset += 2;

for (const affiliate of this.affiliates) {
offset = affiliate.write(buffer, offset);
     }

        buffer.writeInt16LE(this.players.length, offset);
        offset += 2;

      for (const player of this.players) {
            buffer.writeInt32LE(player, offset);
            offset += 4;
        }

    for (const val of this.unknown7) {
      buffer.writeInt32LE(val, offset);
 offset += 4;
        }

      buffer.writeInt32LE(this.mainClub, offset);
        offset += 4;

        buffer.writeInt16LE(this.isNational, offset);
        offset += 2;

        this.unknown8.copy(buffer, offset);
     offset += 33;

        this.unknown9.copy(buffer, offset);
   offset += 40;

     this.unknown10.copy(buffer, offset);
        offset += 2;

   return buffer.slice(0, offset);
    }

    /**
     * Returns a string representation of the club.
     * @returns A string containing the club's UID and full name
     */
    toString(): string {
        return `${this.uid} ${this.fullName}`;
    }
}
