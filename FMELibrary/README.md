# FMELibrary - TypeScript

TypeScript library for parsing and managing Football Manager Editor data files. This is a TypeScript port of the C# FMELibrary, maintaining full compatibility with the binary file formats.

## Features

- **Binary Data Parsing**: Read and write custom binary formats used in Football Manager editor files
- **Entity Management**: Handle clubs, nations, competitions, kits, affiliates, and names
- **Type Safety**: Full TypeScript typing for all entities and operations
- **Async/Await**: Modern async file operations using Node.js fs.promises
- **Buffer-based**: Efficient binary data handling using Node.js Buffer API

## Installation

```bash
npm install
npm run build
```

## Usage

### Parsing Club Data

```typescript
import { ClubParser } from './ClubParser';

// Load clubs from file
const parser = await ClubParser.load('path/to/clubs.dat');

// Access clubs
console.log(`Loaded ${parser.items.length} clubs`);
for (const club of parser.items) {
    console.log(`${club.fullName} (${club.nation})`);
}

// Modify data
parser.items[0].reputation = 9999;

// Save back to file
await parser.save();
// Or save to a different file
await parser.save('path/to/output.dat');
```

### Parsing Nation Data

```typescript
import { NationParser } from './NationParser';

// Load nations from file
const parser = await NationParser.load('path/to/nations.dat');

// Access nations
for (const nation of parser.items) {
    console.log(`${nation.name} - Ranking: ${nation.ranking ?? 'N/A'}`);
}

// Save changes
await parser.save();
```

### Parsing Competition Data

```typescript
import { CompetitionParser } from './CompetitionParser';

// Load competitions
const parser = await CompetitionParser.load('path/to/competitions.dat');

// Access competitions
for (const comp of parser.items) {
    console.log(`${comp.fullName} - Type: ${comp.type}`);
}
```

### Parsing Name Data

```typescript
import { NameParser } from './NameParser';

// Create parser
const parser = new NameParser('path/to/names.dat');

// Parse the file
const names = await parser.parse();

// Access names
for (const name of names) {
    console.log(`${name.id}: ${name.value}`);
}

// Save changes
await parser.save();
```

### Working with Individual Entities

```typescript
import { Club } from './Club';
import { Nation } from './Nation';

// Parse from buffer
const buffer = Buffer.from(/* your data */);
const clubResult = Club.fromBuffer(buffer, 0);
const club = clubResult.club;
const nextOffset = clubResult.offset;

// Serialize to buffer
const clubBuffer = club.toBuffer();

// Modify properties
club.reputation = 8000;
club.fullName = 'New Club Name';
```

## API Reference

### Parsers

- **ClubParser**: Parse and manage club data files
  - `static load(path: string): Promise<ClubParser>`
  - `toBuffer(): Buffer`
  - `save(filepath?: string): Promise<void>`

- **NationParser**: Parse and manage nation data files
  - `static load(path: string): Promise<NationParser>`
  - `toBuffer(): Buffer`
  - `save(filepath?: string): Promise<void>`

- **CompetitionParser**: Parse and manage competition data files
  - `static load(path: string): Promise<CompetitionParser>`
  - `toBuffer(): Buffer`
  - `save(filepath?: string): Promise<void>`

- **NameParser**: Parse and manage name data files
  - `constructor(file: string)`
  - `parse(): Promise<Name[]>`
  - `save(filepath?: string): Promise<void>`

### Entities

- **Club**: Represents a football club with all properties
- **Nation**: Represents a nation with rankings, colors, and coefficients
- **Competition**: Represents a competition/league
- **Kit**: Represents a team kit/uniform
- **Affiliate**: Represents club affiliation relationships
- **Name**: Represents a name entry

### Utilities

- **BinaryParser**: Helper methods for reading/writing binary data
  - `readString(buffer, offset, length)`
  - `readStringEx(buffer, offset)` - Length-prefixed string
  - `readColor(buffer, offset)` - RGB565 color
  - `writeEx(value, buffer, offset)` - Auto-typed write
  - `writeByte(value, buffer, offset)`
  - `writeInt16(value, buffer, offset)`
  - `writeInt32(value, buffer, offset)`
  - `writeFloat(value, buffer, offset)`

- **ArrayExtension**: Array and Buffer slicing utilities
  - `slice<T>(array, length)` - Remove and return elements from start
  - `sliceWithIndex<T>(array, currentIndex, length)` - Extract with index tracking
  - `sliceBuffer(buffer, currentIndex, length)` - Buffer slicing with index

## TypeScript Equivalents

This library maintains feature parity with the C# version:

| C# Type | TypeScript Type | Notes |
|---------|----------------|-------|
| `byte` | `number` | 0-255 |
| `short` | `number` | 16-bit integer |
| `int` | `number` | 32-bit integer |
| `float` | `number` | 32-bit float |
| `byte[]` | `Buffer` | Binary data |
| `string` | `string` | UTF-8 encoded |
| `Color` | `{r, g, b}` | RGB object |
| `BinaryReader` | `Buffer + offset` | Read operations |
| `BinaryWriter` | `Buffer + offset` | Write operations |
| `async Task<T>` | `Promise<T>` | Async operations |

## Building

```bash
# Install dependencies
npm install

# Build TypeScript to JavaScript
npm run build

# Watch mode for development
npm run watch

# Clean build artifacts
npm run clean
```

## Requirements

- Node.js >= 18.0.0
- TypeScript >= 5.0.0

## License

MIT

## Notes

- All file I/O is asynchronous using `fs.promises`
- Binary data is handled using Node.js `Buffer` API
- Offsets are tracked manually when parsing (returned from `fromBuffer` methods)
- The library maintains binary compatibility with the C# version
- Optional properties (nullable types in C#) use TypeScript optional properties (`?`)
