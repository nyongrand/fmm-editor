<template>
  <div class="test-container">
    <h1>.dat File Roundtrip Test</h1>
    <div class="controls">
      <button @click="runTest" :disabled="isTesting" class="test-btn">
        {{ isTesting ? 'Testing...' : 'Run Competition.dat Roundtrip Test' }}
      </button>
      <div v-if="testResult !== null" class="result" :class="{ success: testResult, failure: !testResult }">
        {{ testResult ? '✅ Test PASSED: Bytes are identical!' : '❌ Test FAILED: Bytes differ after roundtrip' }}
      </div>
    </div>
    
    <div class="file-input-section">
      <h3>Test with actual .dat file:</h3>
      <input 
        type="file" 
        ref="fileInput" 
        @change="onFileSelect" 
        accept=".dat" 
        :disabled="isTesting"
      />
      <div v-if="selectedFileName" class="file-info">
        Selected: {{ selectedFileName }}
      </div>
      <button 
        @click="runFileTest" 
        :disabled="!selectedFile || isTesting"
        class="test-btn"
      >
        Test Selected File
      </button>
    </div>
    
    <div v-if="testLog" class="log">
      <h3>Test Log:</h3>
      <pre>{{ testLog }}</pre>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { DatRoundtripTester } from '@/utils/testDatRoundtrip';

const isTesting = ref(false);
const testResult = ref<boolean | null>(null);
const fileInput = ref<HTMLInputElement | null>(null);
const selectedFile = ref<File | null>(null);
const selectedFileName = ref<string>('');
const testLog = ref<string>('');

const runTest = async () => {
  isTesting.value = true;
  testResult.value = null;
  testLog.value = 'Starting competition.dat roundtrip test with mock data...\n';
  
  try {
    const result = await DatRoundtripTester.testWithMockData();
    testResult.value = result;
    testLog.value += `Test completed: ${result ? 'PASSED' : 'FAILED'}\n`;
  } catch (error) {
    testResult.value = false;
    testLog.value += `Test failed with error: ${error}\n`;
    console.error('Test failed:', error);
  } finally {
    isTesting.value = false;
  }
};

const onFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement;
  if (target.files && target.files.length > 0) {
    selectedFile.value = target.files[0];
    selectedFileName.value = selectedFile.value.name;
    testLog.value += `Selected file: ${selectedFileName.value}\n`;
  }
};

const runFileTest = async () => {
  if (!selectedFile.value) return;
  
  isTesting.value = true;
  testResult.value = null;
  testLog.value += `Testing file: ${selectedFile.value.name}\n`;
  
  try {
    const result = await DatRoundtripTester.testWithFile(selectedFile.value);
    testResult.value = result;
    testLog.value += `File test completed: ${result ? 'PASSED' : 'FAILED'}\n`;
  } catch (error) {
    testResult.value = false;
    testLog.value += `File test failed with error: ${error}\n`;
    console.error('File test failed:', error);
  } finally {
    isTesting.value = false;
  }
};
</script>

<style scoped>
.test-container {
  padding: 20px;
  max-width: 800px;
  margin: 0 auto;
}

.controls {
  margin-bottom: 30px;
}

.test-btn {
  background-color: #4CAF50;
  color: white;
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}

.test-btn:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.result {
  margin-top: 15px;
  padding: 10px;
  border-radius: 4px;
  font-weight: bold;
}

.result.success {
  background-color: #dff0d8;
  color: #3c763d;
  border: 1px solid #d6e9c6;
}

.result.failure {
  background-color: #f2dede;
  color: #a94442;
  border: 1px solid #ebccd1;
}

.file-input-section {
  margin-top: 30px;
  padding-top: 20px;
  border-top: 1px solid #eee;
}

.file-info {
  margin: 10px 0;
  font-style: italic;
}

.log {
  margin-top: 30px;
  padding: 15px;
  background-color: #f5f5f5;
  border: 1px solid #ddd;
  border-radius: 4px;
  max-height: 300px;
  overflow-y: auto;
}

.log pre {
  margin: 0;
  white-space: pre-wrap;
  word-wrap: break-word;
  font-size: 14px;
}
</style>