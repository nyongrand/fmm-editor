#!/usr/bin/env node

/**
 * FMEConsole - Console application for testing FMELibrary
 * 
 * This program tests the Parser by loading a .dat file,
 * parsing it, serializing it back, and comparing the bytes to ensure
 * data integrity.
 */

import { NationParser } from './NationParser';
import { promises as fs } from 'fs';
import * as path from 'path';

/**
 * Main entry point for the console application
 */
async function main(): Promise<void> {
  try {
    // Path to the .dat file (relative to the project root)
    const file = path.join(__dirname, '../../FMEConsole/db/db_archive_2603/nation.dat');

    console.log(`Loading items data from: ${file}`);

    // Load and parse the item data
    const parser = await NationParser.load(file);

    console.log(`Loaded items: ${parser.items.length}`);
    console.log(`Parser count: ${parser.count}`);

    // Check if count matches items length
    const match = parser.count === parser.items.length;
    console.log(`Count matches items length: ${match}\n`);

    // Convert back to bytes
    console.log('Serializing items back to bytes...');
    const bytes1 = parser.toBuffer();

    // Read original file
    console.log('Reading original file...\n');
    const bytes2 = await fs.readFile(file);

    // Compare byte sequences
    const equal = bytes1.equals(bytes2);
    console.log(`Bytes are equal: ${equal}`);
    console.log(`Original size  : ${bytes2.length} bytes`);
    console.log(`Serialized size: ${bytes1.length} bytes`);

    // Detailed byte comparison
    if (!equal) {
      console.log('\nPerforming detailed byte comparison...');
      const minLength = Math.min(bytes1.length, bytes2.length);

      for (let i = 0; i < minLength; i++) {
        if (bytes1[i] !== bytes2[i]) {
          throw new Error(
            `Byte mismatch at index ${i}: ${bytes1[i]} != ${bytes2[i]}`
          );
        }
      }

      // Check if one buffer is longer
      if (bytes1.length !== bytes2.length) {
        throw new Error(
          `Length mismatch: serialized=${bytes1.length}, original=${bytes2.length}`
        );
      }
    }

    console.log('\n✓ Test passed, all bytes match.');
    console.log('\nSample items:');

    // Display first 5 items as a sample
    const sampleSize = Math.min(5, parser.items.length);
    for (let i = 0; i < sampleSize; i++) {
      const item = parser.items[i];
      console.log(`  ${i + 1}. ${item.name} (ID: ${item.uid}, Code: ${item.codeName})`);
    }

    process.exit(0);

  } catch (error) {
    console.error('\n✗ Test failed!');
    if (error instanceof Error) {
      console.error(`Error: ${error.message}`);
      console.error(error.stack);
    } else {
      console.error(error);
    }
    process.exit(1);
  }
}

// Run the main function
main();
