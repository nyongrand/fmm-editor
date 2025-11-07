// Utility functions to replicate the WPF converter functionality

// Boolean converter functions
export function booleanConverter(value: any, parameter: string = ''): boolean {
  switch (parameter) {
    case null:
    case undefined:
      return value === true || value === 1;
    case 'Invert':
      return value === null || value === false || value === 0;
    case 'Object':
      return value === null || value === undefined;
    case 'ObjectInvert':
      return value !== null && value !== undefined;
    case 'String':
      return value === null || value === undefined || value === '';
    case 'StringInvert':
      return value !== null && value !== undefined && value !== '';
    case 'Int':
      return value !== null && value !== undefined && value !== 0;
    case 'IntInvert':
      return value === null || value === undefined || value === 0;
    default:
      throw new Error(`Unknown parameter for booleanConverter: ${parameter}`);
  }
}

// Visibility converter functions - returns 'visible' or 'hidden' instead of WPF visibility states
export function visibilityConverter(value: any, parameter: string = ''): 'visible' | 'hidden' {
  switch (parameter) {
    case 'Object':
      return value === null || value === undefined ? 'hidden' : 'visible';
    case 'Boolean':
      return value === true || value === 1 ? 'visible' : 'hidden';
    case 'Inverter':
      return value === true || value === 1 ? 'hidden' : 'visible';
    case 'Text':
      return value === null || value === undefined || value === '' ? 'hidden' : 'visible';
    case 'TextInvert':
      return value === null || value === undefined || value === '' ? 'visible' : 'hidden';
    default:
      return value === true || value === 1 ? 'visible' : 'hidden';
  }
}

// Text converter functions
export function textConverter(value: any, parameter: string = ''): string {
  switch (parameter) {
    case 'EditMode':
      if (value === null || value === undefined) return '';
      return value ? 'Simpan Jadwal' : 'Edit Jadwal';
    
    case 'RowNumber':
      if (value === null || value === undefined) return '';
      return `${value}.`;
    
    case 'ScheduleSwap':
      if (value === null || value === undefined) return '';
      if (value === 0) {
        return 'Belum pernah tukar dinas bulan ini';
      } else {
        return `${value} kali tukar dinas bulan ini`;
      }
    
    case 'Approved':
      if (value === null || value === undefined) return '';
      return value ? 'Diterima' : 'Ditolak';
    
    default:
      return '';
  }
}

// Bytes converter function (converts byte array to hex string)
export function bytesConverter(value: any): string {
  if (Array.isArray(value) && value.every(v => typeof v === 'number' && v >= 0 && v <= 255)) {
    return Array.from(value as number[], byte => byte.toString(16).padStart(2, '0')).join('-');
  }
  return value?.toString() || 'NULL';
}