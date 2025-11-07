import { useMainStore, type Nation as NationType, type Competition as CompetitionType, type Club as ClubType, type Name as NameType } from '@/stores/main'
import { ref } from 'vue'
import { Nation, Competition, Club, Name } from './datSerializer'
import { DatParser } from './datParser'

// File handling utilities for browser-based file operations
export class FileHandler {
  // Load a database directory by using file inputs for .dat files
  static async loadDatabase() {
    return new Promise<{ nations: NationType[], competitions: CompetitionType[], clubs: ClubType[] }>(async (resolve, reject) => {
      // In a real implementation, this would show a folder picker
      // For browser compatibility, we'll use individual file inputs
      
      // Create file inputs for each .dat file type
      let nationFileInput = document.getElementById('nation-file-input') as HTMLInputElement
      if (!nationFileInput) {
        nationFileInput = document.createElement('input')
        nationFileInput.type = 'file'
        nationFileInput.id = 'nation-file-input'
        nationFileInput.style.display = 'none'
        nationFileInput.accept = '.dat'
        document.body.appendChild(nationFileInput)
      }

      let compFileInput = document.getElementById('comp-file-input') as HTMLInputElement
      if (!compFileInput) {
        compFileInput = document.createElement('input')
        compFileInput.type = 'file'
        compFileInput.id = 'comp-file-input'
        compFileInput.style.display = 'none'
        compFileInput.accept = '.dat'
        document.body.appendChild(compFileInput)
      }

      let clubFileInput = document.getElementById('club-file-input') as HTMLInputElement
      if (!clubFileInput) {
        clubFileInput = document.createElement('input')
        clubFileInput.type = 'file'
        clubFileInput.id = 'club-file-input'
        clubFileInput.style.display = 'none'
        clubFileInput.accept = '.dat'
        document.body.appendChild(clubFileInput)
      }
      
      // Track loaded data
      let nations: NationType[] = [];
      let competitions: CompetitionType[] = [];
      let clubs: ClubType[] = [];
      let filesLoaded = 0;
      const totalFiles = 3;
      
      // Handle nation file loading
      nationFileInput.onchange = async (event: any) => {
        const file = event.target.files[0]
        if (file) {
          try {
            const arrayBuffer = await file.arrayBuffer();
            const parsedNations = DatParser.parseNations(arrayBuffer);
            // Convert to our store types
            nations = parsedNations.map(n => ({
              ...n,
              // Add any additional properties that might not be in the parsed object
            } as unknown as NationType));
            filesLoaded++;
            if (filesLoaded === totalFiles) resolve({ nations, competitions, clubs });
          } catch (error) {
            console.error('Error parsing nations file:', error);
            reject(error);
          }
        } else {
          filesLoaded++;
          if (filesLoaded === totalFiles) resolve({ nations, competitions, clubs });
        }
      }
      
      // Handle competition file loading
      compFileInput.onchange = async (event: any) => {
        const file = event.target.files[0]
        if (file) {
          try {
            const arrayBuffer = await file.arrayBuffer();
            const parsedCompetitions = DatParser.parseCompetitions(arrayBuffer);
            // Convert to our store types
            competitions = parsedCompetitions.map(c => ({
              ...c,
              // Map nation name from nationId if needed
              nation: `Nation${c.nationId}` // This would be looked up in a real implementation
            } as unknown as CompetitionType));
            filesLoaded++;
            if (filesLoaded === totalFiles) resolve({ nations, competitions, clubs });
          } catch (error) {
            console.error('Error parsing competitions file:', error);
            reject(error);
          }
        } else {
          filesLoaded++;
          if (filesLoaded === totalFiles) resolve({ nations, competitions, clubs });
        }
      }
      
      // Handle club file loading
      clubFileInput.onchange = async (event: any) => {
        const file = event.target.files[0]
        if (file) {
          try {
            const arrayBuffer = await file.arrayBuffer();
            const parsedClubs = DatParser.parseClubs(arrayBuffer);
            // Convert to our store types
            clubs = parsedClubs.map(c => ({
              ...c,
              based: `City${c.basedId}`, // This would be looked up in a real implementation
              nation: `Nation${c.nationId}` // This would be looked up in a real implementation
            } as unknown as ClubType));
            filesLoaded++;
            if (filesLoaded === totalFiles) resolve({ nations, competitions, clubs });
          } catch (error) {
            console.error('Error parsing clubs file:', error);
            reject(error);
          }
        } else {
          filesLoaded++;
          if (filesLoaded === totalFiles) resolve({ nations, competitions, clubs });
        }
      }
      
      // Click the inputs to prompt user for files
      nationFileInput.click();
      compFileInput.click();
      clubFileInput.click();
    });
  }

  // Handle file input for name files
  static async loadNameFile(inputId: string = 'name-file-input'): Promise<NameType[]> {
    return new Promise(async (resolve, reject) => {
      // Create file input element if it doesn't exist
      let fileInput = document.getElementById(inputId) as HTMLInputElement
      if (!fileInput) {
        fileInput = document.createElement('input')
        fileInput.type = 'file'
        fileInput.id = inputId
        fileInput.style.display = 'none'
        fileInput.accept = '.dat'
        document.body.appendChild(fileInput)
      }

      fileInput.onchange = async (event: any) => {
        const file = event.target.files[0]
        if (file) {
          try {
            const arrayBuffer = await file.arrayBuffer();
            const parsedNames = DatParser.parseNames(arrayBuffer);
            // Convert to our store types
            const names = parsedNames.map(n => ({
              ...n,
            } as unknown as NameType));
            resolve(names);
          } catch (error) {
            console.error('Error parsing names file:', error);
            reject(error);
          }
        } else {
          resolve([])
        }
      }

      fileInput.click()
    });
  }

  // Save data as .dat files
  static async saveDataAsDat(nations: NationType[], competitions: CompetitionType[], clubs: ClubType[]) {
    // Create and download nation.dat
    const nationCountBuffer = new ArrayBuffer(4);
    const nationCountView = new DataView(nationCountBuffer);
    nationCountView.setInt32(0, nations.length, true); // little-endian
    
    const serializedNations = nations.map(n => {
      // Create a Nation instance from the data
      const nation = new Nation(n as Partial<Nation>);
      return nation.toBytes();
    });
    
    const nationDatBytes = new Uint8Array(
      4 + serializedNations.reduce((acc, bytes) => acc + bytes.length, 0)
    );
    nationDatBytes.set(new Uint8Array(nationCountBuffer), 0);
    
    let offset = 4;
    for (const nationBytes of serializedNations) {
      nationDatBytes.set(nationBytes, offset);
      offset += nationBytes.length;
    }
    
    this.downloadFile(nationDatBytes, 'nation.dat');
    
    // Create and download competition.dat
    const compCountBuffer = new ArrayBuffer(2); // competitions use short for count
    const compCountView = new DataView(compCountBuffer);
    compCountView.setInt16(0, competitions.length, true); // little-endian
    
    const serializedComps = competitions.map(c => {
      // Create a Competition instance from the data
      const comp = new Competition(c as Partial<Competition>);
      return comp.toBytes();
    });
    
    const compDatBytes = new Uint8Array(
      2 + serializedComps.reduce((acc, bytes) => acc + bytes.length, 0)
    );
    compDatBytes.set(new Uint8Array(compCountBuffer), 0);
    
    offset = 2;
    for (const compBytes of serializedComps) {
      compDatBytes.set(compBytes, offset);
      offset += compBytes.length;
    }
    
    this.downloadFile(compDatBytes, 'competition.dat');
    
    // Create and download club.dat
    const clubCountBuffer = new ArrayBuffer(4);
    const clubCountView = new DataView(clubCountBuffer);
    clubCountView.setInt32(0, clubs.length, true); // little-endian
    
    const serializedClubs = clubs.map(c => {
      // Create a Club instance from the data
      const club = new Club(c as Partial<Club>);
      return club.toBytes();
    });
    
    const clubDatBytes = new Uint8Array(
      4 + serializedClubs.reduce((acc, bytes) => acc + bytes.length, 0)
    );
    clubDatBytes.set(new Uint8Array(clubCountBuffer), 0);
    
    offset = 4;
    for (const clubBytes of serializedClubs) {
      clubDatBytes.set(clubBytes, offset);
      offset += clubBytes.length;
    }
    
    this.downloadFile(clubDatBytes, 'club.dat');
  }
  
  // Save individual name file
  static async saveNameFile(names: NameType[], filename: string = 'name.dat') {
    // Create and download name.dat
    const headerBuffer = new ArrayBuffer(8); // 4 bytes zeros + 4 bytes count
    const headerView = new DataView(headerBuffer);
    // First 4 bytes are zeros
    headerView.setInt32(4, names.length, true); // little-endian count at offset 4
    
    const serializedNames = names.map(n => {
      // Create a Name instance from the data
      const name = new Name(n as Partial<Name>);
      return name.toBytes();
    });
    
    const nameDatBytes = new Uint8Array(
      8 + serializedNames.reduce((acc, bytes) => acc + bytes.length, 0)
    );
    nameDatBytes.set(new Uint8Array(headerBuffer), 0);
    
    let offset = 8;
    for (const nameBytes of serializedNames) {
      nameDatBytes.set(nameBytes, offset);
      offset += nameBytes.length;
    }
    
    this.downloadFile(nameDatBytes, filename);
  }

  // Helper to download a file from bytes
  private static downloadFile(bytes: Uint8Array, filename: string) {
    const blob = new Blob([bytes], { type: 'application/octet-stream' });
    
    // Create a download link
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    
    // Clean up
    setTimeout(() => {
      document.body.removeChild(a);
      URL.revokeObjectURL(url);
    }, 100);
  }
}