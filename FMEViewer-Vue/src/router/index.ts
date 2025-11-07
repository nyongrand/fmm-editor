import { createRouter, createWebHistory } from 'vue-router'
import MainView from '@/views/MainView.vue'
import NameView from '@/views/NameView.vue'
import TestView from '@/views/TestView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'main',
      component: MainView
    },
    {
      path: '/name',
      name: 'name',
      component: NameView
    },
    {
      path: '/test',
      name: 'test',
      component: TestView
    }
  ]
})

export default router