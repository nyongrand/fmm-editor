/**
 * FMELibrary - TypeScript library for parsing and managing Football Manager Editor data files
 * 
 * This library provides classes for reading and writing binary data files used in
 * Football Manager editor applications, including clubs, nations, competitions, and names.
 */

// Core parsers
export { BinaryParser } from './BinaryParser';
export { ArrayExtension } from './ListExtension';

// Entity classes
export { Affiliate } from './Affiliate';
export { Club } from './Club';
export { Competition } from './Competition';
export { Kit } from './Kit';
export { Name } from './Name';
export { Nation } from './Nation';

// Parser classes
export { ClubParser } from './ClubParser';
export { CompetitionParser } from './CompetitionParser';
export { NameParser } from './NameParser';
export { NationParser } from './NationParser';
