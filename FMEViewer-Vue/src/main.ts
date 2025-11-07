import { createApp } from 'vue'
import { createPinia } from 'pinia'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { aliases, mdi } from 'vuetify/iconsets/mdi'
import '@mdi/font/css/materialdesignicons.css'
import App from './App.vue'
import router from './router'

// Vuetify theme configuration
const myCustomLightTheme = {
  dark: false,
  colors: {
    background: '#f5f5f5',
    surface: '#ffffff',
    primary: '#4CAF50',     // Green (as in original WPF theme)
    'primary-darken-1': '#43A047',
    'primary-lighten-1': '#66BB6A',
    secondary: '#388E3C',
    accent: '#4CAF50',
    error: '#B71C1C',
    info: '#2196F3',
    success: '#4CAF50',
    warning: '#FF9800',
  }
}

const vuetify = createVuetify({
  components,
  directives,
  theme: {
    defaultTheme: 'myCustomLightTheme',
    themes: {
      myCustomLightTheme
    }
  },
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: {
      mdi
    }
  }
})

const app = createApp(App)

app.use(createPinia())
app.use(router)
app.use(vuetify)

app.mount('#app')