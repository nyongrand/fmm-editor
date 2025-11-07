<template>
  <v-app>
    <v-app-bar color="primary" density="compact">
      <v-app-bar-title>FM23 DB Editor</v-app-bar-title>
      <v-spacer></v-spacer>
      <v-menu>
        <template v-slot:activator="{ props }">
          <v-btn icon="mdi-menu" v-bind="props" size="small"></v-btn>
        </template>
        <v-list>
          <v-list-item @click="loadDatabase" title="Load Database"></v-list-item>
          <v-list-item @click="saveDatabase" title="Save"></v-list-item>
          <v-list-item @click="saveAsDatabase" title="Save As..."></v-list-item>
          <v-divider></v-divider>
          <v-list-item :to="{ name: 'main' }" title="Main Editor"></v-list-item>
          <v-list-item :to="{ name: 'name' }" title="Name Editor"></v-list-item>
          <v-list-item :to="{ name: 'test' }" title="Test .dat Roundtrip"></v-list-item>
          <v-divider></v-divider>
          <v-list-item @click="exitApp" title="Exit"></v-list-item>
        </v-list>
      </v-menu>
    </v-app-bar>

    <v-main>
      <router-view />
    </v-main>

    <v-footer app>
      <v-spacer></v-spacer>
      <span v-if="store.folderPath">{{ store.folderPath }}</span>
      <v-spacer></v-spacer>
      <span>Clubs: {{ store.clubs.length }}</span>
    </v-footer>
  </v-app>
</template>

<script setup lang="ts">
import { useMainStore } from '@/stores/main'
import { useRouter } from 'vue-router'
import { onMounted } from 'vue'

const store = useMainStore()
const router = useRouter()

onMounted(() => {
  // Initialize the store when app starts
})

const loadDatabase = () => {
  store.loadDatabase()
}

const saveDatabase = () => {
  store.saveDatabase()
}

const saveAsDatabase = () => {
  store.saveAsDatabase()
}

const exitApp = () => {
  // Handle app exit (in browser this would be just closing the tab)
  console.log('Exit clicked')
}
</script>