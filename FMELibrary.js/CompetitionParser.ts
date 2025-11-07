import { promises as fs } from 'fs';
import { Competition } from './Competition';

/**
 * Parses and manages competition data from binary files.
 */
export class CompetitionParser {
    /** File path */
    filePath: string = '';

    /** File header, 8 bytes */
    header: Buffer = Buffer.alloc(0);

    /** Item count */
    count: number = 0;

    /** List of all items */
    items: Competition[] = [];

    /**
     * Initializes a new instance of the CompetitionParser class.
     * @param path - The file path of the source data
     * @param buffer - The buffer containing the competition data
     */
    private constructor(path: string, buffer: Buffer) {
        this.filePath = path;
        this.header = buffer.slice(0, 8);
     this.count = buffer.readInt16LE(8);
        this.items = [];
    }

    /**
 * Asynchronously loads competition data from the specified file path.
  * @param path - The file path to load competition data from
     * @returns A promise that resolves to the loaded CompetitionParser instance
  */
    static async load(path: string): Promise<CompetitionParser> {
      const buffer = await fs.readFile(path);
        const parser = new CompetitionParser(path, buffer);

        let offset = 10; // Skip header (8) and count (2)
        
        while (offset < buffer.length) {
      const result = Competition.fromBuffer(buffer, offset);
   parser.items.push(result.competition);
         offset = result.offset;
        }

        return parser;
    }

    /**
     * Converts the competition data to a buffer for serialization.
  * @returns A buffer representing the serialized competition data
     */
    toBuffer(): Buffer {
        const buffers: Buffer[] = [];
      
        buffers.push(this.header);
        
        const countBuffer = Buffer.alloc(2);
        countBuffer.writeInt16LE(this.items.length, 0);
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
