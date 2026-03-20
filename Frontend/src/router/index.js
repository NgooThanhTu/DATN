import { createRouter, createWebHistory } from 'vue-router'
import homeRoutes from './homeRoutes'
import authRoutes from './authRoutes'
import dashboardRoutes from './dashboardRoutes'
import spaceRoutes from './spaceRoutes'
import aiRoutes from './aiRoutes'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    ...homeRoutes,
    ...authRoutes,
    ...dashboardRoutes,
    ...spaceRoutes,
    ...aiRoutes
  ]
})

export default router
