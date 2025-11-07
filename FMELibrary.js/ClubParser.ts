import { promises as fs } from 'fs';
import { Club } from './Club';

/**
 * Parses and manages club data from binary files.
 */
export class ClubParser {
  /** File path */
    filePath: string = '';

    /** File header, 8 bytes */
    header: Buffer = Buffer.alloc(0);

    /** Item count */
    count: number = 0;

    /** List of all items */
    items: Club[] = [];

    /**
     * Initializes a new instance of the ClubParser class.
     * @param path - The file path of the source data
     * @param buffer - The buffer containing the club data
     */
  private constructor(path: string, buffer: Buffer) {
    this.filePath = path;
        this.header = buffer.slice(0, 8);
  this.count = buffer.readInt32LE(8);
   this.items = [];
    }

    /**
     * Asynchronously loads club data from the specified file path.
     * @param path - The file path to load club data from
     * @returns A promise that resolves to the loaded ClubParser instance
     */
    static async load(path: string): Promise<ClubParser> {
        const buffer = await fs.readFile(path);
        const parser = new ClubParser(path, buffer);

        let offset = 12; // Skip header (8) and count (4)
        
        while (offset < buffer.length) {
          const result = Club.fromBuffer(buffer, offset);
    parser.items.push(result.club);
            offset = result.offset;
    }

        return parser;
    }

    /**
     * Converts the club data to a buffer for serialization.
     * @returns A buffer representing the serialized club data
     */
    toBuffer(): Buffer {
        const buffers: Buffer[] = [];
 
        buffers.push(this.header);
        
        const countBuffer = Buffer.alloc(4);
   countBuffer.writeInt32LE(this.items.length, 0);
        buffers.push(countBuffer);

        for (const item of this.items) {
            buffers.push(item.toBuffer());
 }

        return Buffer.concat(buffers);
    }

    /**
     * Asynchronously saves data back to file path.
     * @param filepath - Optional file path. If null, saves to the original file path
     * @returns A promise that resolves when the save operation is complete
     */
    async save(filepath?: string): Promise<void> {
        const buffer = this.toBuffer();
      await fs.writeFile(filepath ?? this.filePath, buffer);
    }
}
