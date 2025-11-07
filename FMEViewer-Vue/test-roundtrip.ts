import { DatRoundtripTester } from './src/utils/testDatRoundtrip';

// Run the mock data test
async function runTest() {
  console.log('Starting competition.dat roundtrip test with mock data...');
  
  const result = await DatRoundtripTester.testWithMockData();
  
  if (result) {
    console.log('✅ Test PASSED: Reading and writing competition.dat produces identical bytes!');
  } else {
    console.log('❌ Test FAILED: Bytes are not identical after roundtrip');
  }
  
  return result;
}

// Execute the test
runTest().then(success => {
  console.log(`Test completed with result: ${success ? 'SUCCESS' : 'FAILURE'}`);
}).catch(error => {
  console.error('Test failed with error:', error);
});