import { DatRoundtripTester } from './testDatRoundtrip';

async function runDatTests() {
  console.log('🧪 Starting .dat file roundtrip tests...\n');
  
  let allTestsPassed = true;
  let testCount = 0;
  let passCount = 0;
  
  // Test 1: Competition.dat roundtrip with mock data
  console.log('Test 1: Competition.dat roundtrip with mock data');
  testCount++;
  try {
    const competitionResult = await DatRoundtripTester.testWithMockData();
    if (competitionResult) {
      console.log('✅ PASS: Competition.dat roundtrip test\n');
      passCount++;
    } else {
      console.log('❌ FAIL: Competition.dat roundtrip test\n');
      allTestsPassed = false;
    }
  } catch (error) {
    console.log('❌ ERROR: Competition.dat roundtrip test -', error, '\n');
    allTestsPassed = false;
  }
  
  // Summary
  console.log('📊 Test Summary:');
  console.log(`Total tests: ${testCount}`);
  console.log(`Passed: ${passCount}`);
  console.log(`Failed: ${testCount - passCount}`);
  console.log(`Success rate: ${Math.round((passCount / testCount) * 100)}%`);
  
  if (allTestsPassed) {
    console.log('\n🎉 All tests passed! .dat file functionality is working correctly.');
    process.exit(0);
  } else {
    console.log('\n💥 Some tests failed! Please check the implementation.');
    process.exit(1);
  }
}

// Run the tests
runDatTests().catch(error => {
  console.error('💥 Test suite failed with error:', error);
  process.exit(1);
});