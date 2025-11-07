<template>
  <div class="main-view">
    <!-- Search Bar -->
    <div class="search-container">
      <v-text-field
        v-model="searchQuery"
        label="Search Competition Name / Nation"
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
      <!-- Competitions Grid -->
      <v-data-table
        :headers="competitionHeaders"
        :items="filteredComps"
        :items-per-page="20"
        class="competitions-table"
        item-value="id"
        @click:row="onCompetitionClick"
        :show-select="false"
        :item-selected="isSelectedCompetition"
      >
        <template #item.nation="{ item }">
          <div class="text-center">{{ item.nation }}</div>
        </template>
        <template #item.fullName="{ item }">
          {{ item.fullName }}
        </template>
      </v-data-table>

      <!-- Clubs Grid -->
      <v-data-table
        :headers="clubHeaders"
        :items="filteredClubsForCompetition"
        :items-per-page="20"
        class="clubs-table"
        item-value="id"
        @click:row="onClubClick"
        :show-select="false"
        :item-selected="isSelectedClub"
      >
        <template #item.nation="{ item }">
          <div class="text-center">{{ item.nation }}</div>
        </template>
        <template #item.reputation="{ item }">
          <div class="text-center">{{ item.reputation }}</div>
        </template>
        <template #item.fullName="{ item }">
          {{ item.fullName }}
        </template>
        <template #item.data-table-actions="{ item }">
          <v-menu>
            <template #activator="{ props }">
              <v-btn
                icon="mdi-dots-vertical"
                variant="text"
                v-bind="props"
                density="comfortable"
              ></v-btn>
            </template>
            <v-list density="compact">
              <v-list-item @click="moveClubAction(item)" :title="`Move club to ${selectedCompetition?.shortName || 'selected competition'}`"></v-list-item>
              <v-list-item @click="removeClubAction(item)" :title="`Remove club from ${selectedCompetition?.shortName || 'competition'}`"></v-list-item>
              <v-list-item @click="switchClubAction(item)" :title="`Switch ${item.shortName} with...`"></v-list-item>
            </v-list>
          </v-menu>
        </template>
      </v-data-table>
    </div>

    <!-- Dialogs -->
    <!-- Move Club Dialog -->
    <v-dialog v-model="showMoveDialog" max-width="400">
      <v-card>
        <v-card-title>MOVE CLUB</v-card-title>
        <v-card-text>
          <div class="text-center mb-4">to</div>
          <div class="text-center text-h6 primary--text mb-6">{{ selectedCompetition?.fullName }}</div>
          
          <v-select
            v-model="filterSwitchNation"
            :items="nations"
            item-title="name"
            item-value="id"
            label="Filter club by nation"
            clearable
          ></v-select>
          
          <v-select
            v-model="switchedWithClub"
            :items="filteredSwitchClubs"
            item-title="fullName"
            item-value="id"
            label="Club to switch"
            clearable
          ></v-select>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn @click="cancelSwitch">CANCEL</v-btn>
          <v-btn color="primary" @click="confirmMove">MOVE</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <!-- Switch Club Dialog -->
    <v-dialog v-model="showSwitchDialog" max-width="400">
      <v-card>
        <v-card-title>SWITCH CLUB</v-card-title>
        <v-card-text>
          <div class="text-center text-h6 primary--text mb-2">{{ selectedClub?.fullName }}</div>
          <div class="text-center mb-4">with</div>
          
          <v-select
            v-model="filterSwitchNation"
            :items="nations"
            item-title="name"
            item-value="id"
            label="Filter club by nation"
            clearable
          ></v-select>
          
          <v-select
            v-model="switchedWithClub"
            :items="filteredSwitchClubs"
            item-title="fullName"
            item-value="id"
            label="Club to switch"
            clearable
          ></v-select>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn @click="cancelSwitch">CANCEL</v-btn>
          <v-btn color="primary" @click="confirmSwitch">SWITCH</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    
    <!-- Snackbar for messages (replacing the WPF Snackbar) -->
    <v-snackbar
      v-model="snackbar.show"
      :timeout="snackbar.timeout"
      :color="snackbar.color"
    >
      {{ snackbar.text }}
      <template #actions>
        <v-btn
          variant="text"
          @click="snackbar.show = false"
        >
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script setup lang="ts">
import { useMainStore } from '@/stores/main'
import { computed, ref, type Ref } from 'vue'
import type { Competition, Club, Nation } from '@/stores/main'

const store = useMainStore()

// Snackbar state
const snackbar = ref({
  show: false,
  text: '',
  color: 'info',
  timeout: 3000
})

// Destructure the store properties
const {
  searchQuery,
  selectedCompetition,
  selectedClub,
  showMoveDialog,
  showSwitchDialog,
  filterSwitchNation,
  switchedWithClub,
  nations,
  filteredComps,
  filteredClubsForCompetition,
  filteredSwitchClubs,
  clearSearch,
  switchClub,
  moveClub,
  removeClub,
  cancelSwitch,
  confirmMove,
  confirmSwitch
} = store

// Competition table headers
const competitionHeaders = [
  { title: 'Nation', key: 'nation', align: 'center' as const },
  { title: 'Competition Name', key: 'fullName' }
]

// Club table headers
const clubHeaders = [
  { title: 'Nation', key: 'nation', align: 'center' as const },
  { title: 'Reputation', key: 'reputation', align: 'center' as const },
  { title: 'Name', key: 'fullName' },
  { title: 'Actions', key: 'actions', sortable: false }
]

// Helper functions to check if an item is selected
const isSelectedCompetition = (item: Competition) => {
  return store.selectedCompetition !== null && store.selectedCompetition.id === item.id
}

const isSelectedClub = (item: Club) => {
  return store.selectedClub !== null && store.selectedClub.id === item.id
}

// Event handlers
const onCompetitionClick = (e: any, { item }: { item: Competition }) => {
  store.selectedCompetition = item
}

const onClubClick = (e: any, { item }: { item: Club }) => {
  store.selectedClub = item
}

const moveClubAction = (club: Club) => {
  store.selectedClub = club
  moveClub()
}

const removeClubAction = (club: Club) => {
  store.selectedClub = club
  removeClub()
}

const switchClubAction = (club: Club) => {
  store.selectedClub = club
  switchClub()
}
</script>

<style scoped>
.main-view {
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
  grid-template-columns: 1fr 2fr;
  gap: 16px;
  height: calc(100vh - 180px); /* Adjust based on header/footer height */
}

.competitions-table, .clubs-table {
  height: 100%;
  overflow-y: auto;
}

@media (max-width: 960px) {
  .content-grid {
    grid-template-columns: 1fr;
    height: auto;
  }
}
</style>