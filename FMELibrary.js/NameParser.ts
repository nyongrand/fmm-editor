import { promises as fs } from 'fs';
import { Name } from './Name';

/**
 * Parses and manages name data from binary files.
 */
export class NameParser {
  /** Gets or sets the file path of the source data. */
  filePath: string = '';

  /** Gets or sets the total count of name entries. */
  count: number = 0;

  /** Gets or sets the file header (8 bytes). */
  header: Buffer = Buffer.alloc(0);

  /** Private backing field for names */
  private _names: Name[] | null = null;

  /**
   * Gets or sets the list of name entries. Throws an exception if accessed before parse() is called.
   * @throws Error when accessed before parse() is called
   */
  get names(): Name[] {
    if (this._names === null) {
      throw new Error('parse() is not called');
    }
    return this._names;
  }

  set names(value: Name[]) {
    this._names = value;
  }

  /**
   * Initializes a new instance of the NameParser class.
   * @param file - The file path to parse
*/
  constructor(file: string) {
    this.filePath = file;
  }

  /**
   * Asynchronously parses the name data from the file.
   * @returns A promise that resolves to the list of parsed names
   */
  async parse(): Promise<Name[]> {
    const buffer = await fs.readFile(this.filePath);

    this.header = buffer.slice(0, 8);
    this.count = buffer.readInt32LE(8);
    this._names = [];

    let offset = 12;
    while (offset < buffer.length && this._names.length < this.count) {
      const result = Name.fromBuffer(buffer, offset);
      this._names.push(result.name);
      offset = result.offset;
    }

    return this._names;
  }

  /**
   * Asynchronously saves the name data to a file.
   * @param filepath - Optional file path. If null, saves to the original file path
   * @returns A promise that resolves when the save operation is complete
   */
  async save(filepath?: string): Promise<void> {
    if (this._names === null) {
      throw new Error('parse() is not called');
    }

    const buffers: Buffer[] = [];

    buffers.push(this.header);

    const countBuffer = Buffer.alloc(4);
    countBuffer.writeInt32LE(this._names.length, 0);
    buffers.push(countBuffer);

    for (const name of this._names) {
      buffers.push(name.toBuffer());
    }

    const buffer = Buffer.concat(buffers);
    await fs.writeFile(filepath ?? this.filePath, buffer);
  }
}
