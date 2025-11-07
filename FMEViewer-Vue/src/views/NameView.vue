<template>
  <div class="name-view">
    <!-- Search Bar -->
    <div class="search-container">
      <v-text-field
        v-model="store.searchQuery"
        label="Search"
        prepend-inner-icons="mdi-magnify"
        variant="outlined"
        density="compact"
        hide-details
        clearable
        style="max-width: 350px; margin-right: auto;"
      ></v-text-field>
    </div>

    <!-- Main Content Grid -->
    <div class="content-grid">
      <!-- Names Grid 1 -->
      <v-data-table
        :headers="nameHeaders"
        :items="filteredNames1"
        :items-per-page="20"
        class="names-table"
        item-value="id"
      >
        <template #item.id="{ item }">
          <div class="text-center">{{ item.id }}</div>
        </template>
        <template #item.nation="{ item }">
          <div class="text-center">{{ item.nation }}</div>
        </template>
        <template #item.others="{ item }">
          <div class="text-center">{{ item.others }}</div>
        </template>
        <template #item.value="{ item }">
          {{ item.value }}
        </template>
      </v-data-table>

      <!-- Names Grid 2 -->
      <v-data-table
        :headers="nameHeaders"
        :items="filteredNames2"
        :items-per-page="20"
        class="names-table"
        item-value="id"
      >
        <template #item.id="{ item }">
          <div class="text-center">{{ item.id }}</div>
        </template>
        <template #item.nation="{ item }">
          <div class="text-center">{{ item.nation }}</div>
        </template>
        <template #item.others="{ item }">
          <div class="text-center">{{ item.others }}</div>
        </template>
        <template #item.value="{ item }">
          {{ item.value }}
        </template>
      </v-data-table>
    </div>

    <!-- Status Bar -->
    <v-footer app class="status-bar">
      <span>{{ store.filePath1 }}</span>
      <span> | Names: {{ store.names1.length }}</span>
      <v-spacer></v-spacer>
      <span>{{ store.filePath2 }}</span>
      <span> | Names: {{ store.names2.length }}</span>
    </v-footer>
  </div>
</template>

<script setup lang="ts">
import { useMainStore } from '@/stores/main'
import { computed } from 'vue'
import type { Name } from '@/stores/main'

const store = useMainStore()

// Name table headers
const nameHeaders = [
  { title: 'Id', key: 'id', align: 'center' as const },
  { title: 'Nation', key: 'nation', align: 'center' as const },
  { title: 'Unknown', key: 'others', align: 'center' as const },
  { title: 'Name', key: 'value' }
]

// Computed properties for filtering names
const filteredNames1 = computed(() => {
  if (!store.searchQuery) return store.names1;
  const query = store.searchQuery.toLowerCase();
  return store.names1.filter((name: Name) => 
    name.value?.toLowerCase().includes(query) || 
    name.nation?.toLowerCase().includes(query) || 
    name.others?.toLowerCase().includes(query)
  );
});

const filteredNames2 = computed(() => {
  if (!store.searchQuery) return store.names2;
  const query = store.searchQuery.toLowerCase();
  return store.names2.filter((name: Name) => 
    name.value?.toLowerCase().includes(query) || 
    name.nation?.toLowerCase().includes(query) || 
    name.others?.toLowerCase().includes(query)
  );
});
</script>

<style scoped>
.name-view {
  padding: 16px;
  height: 100%;
}

.search-container {
  margin-bottom: 16px;
  display: flex;
  align-items: center;
}

.content-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  height: calc(100vh - 180px); /* Adjust based on header/footer height */
}

.names-table {
  height: 100%;
  overflow-y: auto;
}

.status-bar {
  display: flex;
  justify-content: space-between;
  padding: 0 16px;
  font-size: 12px;
}

@media (max-width: 960px) {
  .content-grid {
    grid-template-columns: 1fr;
    height: auto;
  }
  
  .status-bar {
    flex-direction: column;
    align-items: flex-start;
    gap: 4px;
  }
}
</style>