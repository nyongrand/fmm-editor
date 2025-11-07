// Utility functions to convert JavaScript/TypeScript values to byte arrays (similar to .NET binary serialization)
export class BinaryConverter {
  // Convert string to bytes using UTF-8 encoding with proper termination
  static stringToBytes(str: string): Uint8Array {
    if (!str) return new Uint8Array([0x00]);
    
    const encoder = new TextEncoder();
    const strBytes = encoder.encode(str);
    const result = new Uint8Array(strBytes.length + 1);
    result.set(strBytes, 0);
    result.set([0x00], strBytes.length); // null terminator
    return result;
  }

  // Convert string to bytes without null terminator
  static stringToBytesNoNull(str: string): Uint8Array {
    const encoder = new TextEncoder();
    return encoder.encode(str);
  }

  // Convert number to bytes (little-endian)
  static numberToBytes(value: number, byteCount: number): Uint8Array {
    const buffer = new ArrayBuffer(byteCount);
    const view = new DataView(buffer);
    
    if (byteCount === 1) {
      view.setUint8(0, value);
    } else if (byteCount === 2) {
      view.setInt16(0, value, true); // little-endian
    } else if (byteCount === 4) {
      view.setInt32(0, value, true); // little-endian
    }
    
    return new Uint8Array(buffer);
  }
  
  // Convert boolean to single byte
  static booleanToByte(value: boolean): Uint8Array {
    return new Uint8Array([value ? 1 : 0]);
  }
  
  // Convert float to bytes (4 bytes)
  static floatToBytes(value: number): Uint8Array {
    const buffer = new ArrayBuffer(4);
    const view = new DataView(buffer);
    view.setFloat32(0, value, true); // little-endian
    return new Uint8Array(buffer);
  }
  
  // Combine multiple byte arrays into one
  static combineBytes(...byteArrays: Uint8Array[]): Uint8Array {
    const totalLength = byteArrays.reduce((acc, arr) => acc + arr.length, 0);
    const result = new Uint8Array(totalLength);
    let offset = 0;
    
    for (const arr of byteArrays) {
      result.set(arr, offset);
      offset += arr.length;
    }
    
    return result;
  }
}