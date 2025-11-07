import { Competition } from './datSerializer';
import { DatParser } from './datParser';

export class DatRoundtripTester {
  /**
   * Test that reading a competition.dat file and writing it back produces identical bytes
   * @param originalBytes The original competition.dat file as bytes
   * @returns true if roundtrip produces identical bytes, false otherwise
   */
  static async testCompetitionRoundtrip(originalBytes: Uint8Array): Promise<boolean> {
    try {
      // Convert Uint8Array to ArrayBuffer for the parser
      const arrayBuffer = originalBytes.buffer.slice(
        originalBytes.byteOffset, 
        originalBytes.byteOffset + originalBytes.byteLength
      );
      
      // Parse the original file
      console.log('Parsing original competition.dat file...');
      const originalCompetitions = DatParser.parseCompetitions(arrayBuffer);
      console.log(`Parsed ${originalCompetitions.length} competitions`);
      
      // Create bytes from the parsed data (serialization)
      console.log('Serializing parsed data back to bytes...');
      
      // Create count header (short = 2 bytes, little-endian)
      const countBuffer = new ArrayBuffer(2);
      const countView = new DataView(countBuffer);
      countView.setInt16(0, originalCompetitions.length, true); // little-endian
      
      // Serialize each competition
      const serializedCompetitions = originalCompetitions.map(c => {
        const comp = new Competition(c);
        return comp.toBytes();
      });
      
      // Combine header + all serialized competitions
      const totalSize = 2 + serializedCompetitions.reduce((acc, bytes) => acc + bytes.length, 0);
      const resultBytes = new Uint8Array(totalSize);
      
      // Add count header
      resultBytes.set(new Uint8Array(countBuffer), 0);
      
      // Add each serialized competition
      let offset = 2;
      for (const compBytes of serializedCompetitions) {
        resultBytes.set(compBytes, offset);
        offset += compBytes.length;
      }
      
      // Compare original and result bytes
      if (originalBytes.length !== resultBytes.length) {
        console.log(`Length mismatch: original ${originalBytes.length}, result ${resultBytes.length}`);
        return false;
      }
      
      for (let i = 0; i < originalBytes.length; i++) {
        if (originalBytes[i] !== resultBytes[i]) {
          console.log(`Byte mismatch at position ${i}: original ${originalBytes[i]}, result ${resultBytes[i]}`);
          return false;
        }
      }
      
      console.log('Roundtrip test PASSED: Bytes are identical!');
      return true;
    } catch (error) {
      console.error('Error during roundtrip test:', error);
      return false;
    }
  }
  
  /**
   * Create a test competition.dat file with mock data and test roundtrip
   * @returns true if roundtrip test passes, false otherwise
   */
  static async testWithMockData(): Promise<boolean> {
    try {
      // Create mock competition data
      const mockCompetitions: Competition[] = [
        new Competition({
          id: 1,
          uid: 101,
          fullName: 'Premier League',
          unknown1: 0,
          shortName: 'Prem',
          unknown2: 0,
          codeName: 'PL',
          type: 0,
          continentId: 1,
          nationId: 1,
          color1: 0x4CAF,
          color2: 0x5038,
          reputation: 95,
          level: 1,
          mainComp: 0,
          qualifiers: [],
          rank1: 0,
          rank2: 0,
          rank3: 0,
          year1: 2023,
          year2: 2024,
          year3: 0,
          unknown3: 0,
          unknown4: 0,
        }),
        new Competition({
          id: 2,
          uid: 102,
          fullName: 'La Liga',
          unknown1: 0,
          shortName: 'LL',
          unknown2: 0,
          codeName: 'LL',
          type: 0,
          continentId: 1,
          nationId: 2,
          color1: 0x388E,
          color2: 0x3C38,
          reputation: 92,
          level: 1,
          mainComp: 0,
          qualifiers: [],
          rank1: 0,
          rank2: 0,
          rank3: 0,
          year1: 2023,
          year2: 2024,
          year3: 0,
          unknown3: 0,
          unknown4: 0,
        }),
      ];
      
      // Serialize the mock data to bytes (simulate a competition.dat file)
      console.log('Creating mock competition.dat file...');
      
      const countBuffer = new ArrayBuffer(2);
      const countView = new DataView(countBuffer);
      countView.setInt16(0, mockCompetitions.length, true); // little-endian
      
      const serializedCompetitions = mockCompetitions.map(comp => comp.toBytes());
      
      const totalSize = 2 + serializedCompetitions.reduce((acc, bytes) => acc + bytes.length, 0);
      const originalBytes = new Uint8Array(totalSize);
      
      originalBytes.set(new Uint8Array(countBuffer), 0);
      
      let offset = 2;
      for (const compBytes of serializedCompetitions) {
        originalBytes.set(compBytes, offset);
        offset += compBytes.length;
      }
      
      console.log(`Created mock file with ${mockCompetitions.length} competitions, ${originalBytes.length} bytes`);
      
      // Now test the roundtrip
      return await this.testCompetitionRoundtrip(originalBytes);
    } catch (error) {
      console.error('Error during mock data test:', error);
      return false;
    }
  }
  
  /**
   * Test function that can be used to test with actual file input
   * @param file The competition.dat file from an input element
   * @returns Promise resolving to true if roundtrip test passes
   */
  static async testWithFile(file: File): Promise<boolean> {
    try {
      const arrayBuffer = await file.arrayBuffer();
      const originalBytes = new Uint8Array(arrayBuffer);
      
      console.log(`Testing with file: ${file.name}, ${originalBytes.length} bytes`);
      
      return await this.testCompetitionRoundtrip(originalBytes);
    } catch (error) {
      console.error('Error reading file:', error);
      return false;
    }
  }
}

// Example usage:
// For testing in a browser environment:
/*
document.addEventListener('DOMContentLoaded', () => {
  const input = document.createElement('input');
  input.type = 'file';
  input.accept = '.dat';
  input.onchange = async (e) => {
    const file = (e.target as HTMLInputElement).files?.[0];
    if (file && file.name.includes('competition')) {
      const result = await DatRoundtripTester.testWithFile(file);
      console.log('Roundtrip test result:', result);
    }
  };
  // To manually trigger: input.click();
});
*/