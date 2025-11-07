import { promises as fs } from 'fs';
import { Nation } from './Nation';

/**
 * Parses and manages nation data from binary files.
 */
export class NationParser {
  /** File path */
    filePath: string = '';

    /** File header, 8 bytes */
    header: Buffer = Buffer.alloc(0);

    /** Item count */
    count: number = 0;

    /** List of all items */
    items: Nation[] = [];

    /**
     * Initializes a new instance of the NationParser class.
     * @param path - The file path of the source data
     * @param buffer - The buffer containing the nation data
     */
    private constructor(path: string, buffer: Buffer) {
this.filePath = path;
  this.header = buffer.slice(0, 8);
        this.count = buffer.readInt16LE(8);
        this.items = [];
    }

    /**
     * Asynchronously loads nation data from the specified file path.
     * @param path - The file path to load nation data from
     * @returns A promise that resolves to the loaded NationParser instance
     */
    static async load(path: string): Promise<NationParser> {
        const buffer = await fs.readFile(path);
   const parser = new NationParser(path, buffer);

        let offset = 10; // Skip header (8) and count (2)

     while (offset < buffer.length) {
      const result = Nation.fromBuffer(buffer, offset);
            parser.items.push(result.nation);
            offset = result.offset;

         // Debug output
   console.log(`Loaded Nation: ${result.nation.name}`);
        }

        return parser;
    }

    /**
     * Converts the nation data to a buffer for serialization.
     * @returns A buffer representing the serialized nation data
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
