import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

// Define the interfaces based on the C# models
export interface Nation {
  uid: number
  id: number
  name: string
  nationality: string
  codeName: string
  continentId: number
  cityId: number
  stadiumId: number
  languages: [number, number][] // Array of (short, byte) tuples
  isActive: boolean
  hasCoefficient2: boolean
  // Add other properties as needed for completeness
}

export interface Competition {
  id: number
  uid: number
  fullName: string
  shortName: string
  codeName: string
  nationId: number
  nation: string
  // Add other properties from original model
  unknown1?: number
  unknown2?: number
  type?: number
  continentId?: number
  color1?: number
  color2?: number
  reputation?: number
  level?: number
  mainComp?: number
  qualifiers?: number[][]
  rank1?: number
  rank2?: number
  rank3?: number
  year1?: number
  year2?: number
  year3?: number
  unknown3?: number
  unknown4?: number
}

export interface Club {
  id: number
  uid: number
  fullName: string
  shortName: string
  codeName1: string
  codeName2: string
  basedId: number
  nationId: number
  leagueId: number
  reputation: number
  based: string
  nation: string
  // Add other properties from original model
  unknown0?: number
  unknown1?: number
  colors?: number[]
  status?: number
  academy?: number
  facilities?: number
  attAvg?: number
  attMin?: number
  attMax?: number
  reserves?: number
  unknown2?: number
  unknown3?: number
  stadium?: number
  lastLeague?: number
  unknown4Flag?: boolean
  unknown4?: number[]
  unknown5?: number[]
  leaguePos?: number
  unknown6?: number[]
  affiliates?: any[] // Would need Affiliate interface if fully implemented
  players?: number[]
  unknown7?: number[]
  mainClub?: number
  isNational?: number
  unknown8?: number[]
  unknown9?: number[]
  unknown10?: number[]
}

export interface Name {
  id: number
  nation: string
  others: string
  value: string
}

// Define parser interfaces
export interface NationParser {
  items: Nation[]
  // Add other properties as needed
}

export interface CompetitionParser {
  items: Competition[]
  count: number
  // Add other properties as needed
}

export interface ClubParser {
  items: Club[]
  count: number
  // Add other properties as needed
}

export interface NameParser {
  names: Name[]
  count: number
  // Add other properties as needed
}

export const useMainStore = defineStore('main', () => {
  // State - equivalent to properties in MainViewModel
  const searchQuery = ref('')
  const selectedCompetition = ref<Competition | null>(null)
  const selectedClub = ref<Club | null>(null)
  const showDialog = ref(false)
  const showMoveDialog = ref(false)
  const showSwitchDialog = ref(false)
  const filterSwitchNation = ref<Nation | null>(null)
  const switchedWithClub = ref<Club | null>(null)
  const folderPath = ref<string | null>(null)
  
  // Collections - equivalent to ObservableCollection in MainViewModel
  const nations = ref<Nation[]>([])
  const comps = ref<Competition[]>([])
  const clubs = ref<Club[]>([])
  const switches = ref<Club[]>([])
  
  // For NameViewModel functionality
  const filePath1 = ref<string | null>(null)
  const filePath2 = ref<string | null>(null)
  const names1 = ref<Name[]>([])
  const names2 = ref<Name[]>([])
  
  // Snackbar/message queue functionality (replacing WPF ISnackbarMessageQueue)
  const snackbarQueue = ref<Array<{text: string, color: string}>>([])

  // Computed - filters based on search
  const filteredComps = computed(() => {
    if (!searchQuery.value) return comps.value
    
    const query = searchQuery.value.toLowerCase()
    return comps.value.filter(comp => 
      comp.fullName.toLowerCase().includes(query) || 
      comp.nation.toLowerCase().includes(query)
    )
  })

  const filteredClubsForCompetition = computed(() => {
    if (!selectedCompetition.value) return clubs.value
    return clubs.value.filter(club => club.leagueId === selectedCompetition.value?.id)
  })

  const filteredSwitchClubs = computed(() => {
    if (!selectedCompetition.value || !filterSwitchNation.value) return switches.value
    return switches.value.filter(club => 
      club.leagueId !== selectedCompetition.value?.id && 
      club.nationId === filterSwitchNation.value?.id
    )
  })

  // Actions - equivalent to commands in MainViewModel
  const clearSearch = () => {
    searchQuery.value = ''
  }

  const loadDatabase = async () => {
    console.log('Loading database...')
    try {
      const result = await import('@/utils/fileHandler').then(mod => mod.FileHandler.loadDatabase())
      nations.value = result.nations
      comps.value = result.competitions
      clubs.value = result.clubs
      switches.value = [...clubs.value] // Initially all clubs can be switched
      folderPath.value = 'sample/database/path'
      showSnackbarMessage('Database loaded successfully', 'success')
    } catch (error) {
      console.error('Error loading database:', error)
      showSnackbarMessage(`Load error: ${(error as Error).message}`, 'error')
    }
  }

  const saveDatabase = async () => {
    console.log('Saving database...')
    try {
      await import('@/utils/fileHandler').then(mod => 
        mod.FileHandler.saveDataAsDat(nations.value, comps.value, clubs.value)
      )
      showSnackbarMessage('Database saved successfully', 'success')
    } catch (error) {
      console.error('Error saving database:', error)
      showSnackbarMessage(`Save error: ${(error as Error).message}`, 'error')
    }
  }

  const saveAsDatabase = async () => {
    console.log('Saving database as...')
    try {
      await import('@/utils/fileHandler').then(mod => 
        mod.FileHandler.saveDataAsDat(nations.value, comps.value, clubs.value)
      )
      showSnackbarMessage('Database saved successfully', 'success')
    } catch (error) {
      console.error('Error saving database as:', error)
      showSnackbarMessage(`Save error: ${(error as Error).message}`, 'error')
    }
  }

  // Club management actions
  const switchClub = () => {
    filterSwitchNation.value = null
    switchedWithClub.value = null
    showMoveDialog.value = false
    showSwitchDialog.value = true
  }

  const moveClub = () => {
    filterSwitchNation.value = null
    switchedWithClub.value = null
    showMoveDialog.value = true
    showSwitchDialog.value = false
  }

  const removeClub = () => {
    if (selectedClub.value) {
      selectedClub.value.basedId = selectedClub.value.nationId
      selectedClub.value.leagueId = -1
    }
  }

  const cancelSwitch = () => {
    filterSwitchNation.value = null
    switchedWithClub.value = null
    showMoveDialog.value = false
    showSwitchDialog.value = false
  }

  const confirmMove = () => {
    showMoveDialog.value = false

    if (selectedCompetition.value && switchedWithClub.value) {
      switchedWithClub.value.basedId = selectedCompetition.value.nationId
      switchedWithClub.value.leagueId = selectedCompetition.value.id
    }
  }

  const confirmSwitch = () => {
    showSwitchDialog.value = false

    if (selectedClub.value && switchedWithClub.value) {
      // Swap based and league IDs
      const tempBased = selectedClub.value.basedId
      const tempLeague = selectedClub.value.leagueId
      
      selectedClub.value.basedId = switchedWithClub.value.basedId
      selectedClub.value.leagueId = switchedWithClub.value.leagueId
      
      switchedWithClub.value.basedId = tempBased
      switchedWithClub.value.leagueId = tempLeague
    }
  }

  // Action to show message in snackbar
  const showSnackbarMessage = (text: string, color: string = 'info') => {
    snackbarQueue.value.push({ text, color })
    // In a real implementation, this would trigger the snackbar display
    console.log(`Snackbar: ${text} (${color})`)
  }

  return {
    // State
    searchQuery,
    selectedCompetition,
    selectedClub,
    showDialog,
    showMoveDialog,
    showSwitchDialog,
    filterSwitchNation,
    switchedWithClub,
    folderPath,
    nations,
    comps,
    clubs,
    switches,
    filePath1,
    filePath2,
    names1,
    names2,
    snackbarQueue,
    
    // Computed
    filteredComps,
    filteredClubsForCompetition,
    filteredSwitchClubs,
    
    // Actions
    clearSearch,
    loadDatabase,
    saveDatabase,
    saveAsDatabase,
    switchClub,
    moveClub,
    removeClub,
    cancelSwitch,
    confirmMove,
    confirmSwitch,
    showSnackbarMessage
  }
})