import { createRouter, createWebHistory } from 'vue-router'
import axiosClient from '@/api/axiosClient'
import { getStoredUser, hasSystemAdminAccess } from '@/utils/permissions'
import homeRoutes from './homeRoutes'
import authRoutes from './authRoutes'
import dashboardRoutes from './dashboardRoutes'
import spaceRoutes from './spaceRoutes'
import aiRoutes from './aiRoutes'
import logRoutes from './logRoutes'
import adminRoutes from './adminRoutes'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    ...homeRoutes,
    ...authRoutes,
    ...dashboardRoutes,
    ...spaceRoutes,
    ...aiRoutes,
    ...logRoutes,
    ...adminRoutes
  ]
})

router.beforeEach(async (to, from, next) => {
  const token = localStorage.getItem('accessToken')
  const publicPages = ['/login', '/register', '/', '/auth/github/callback', '/accept-invite']
  const authRequired = !publicPages.includes(to.path)

  if (authRequired && !token) {
    return next({
      path: '/login',
      query: { redirect: to.fullPath }
    })
  }

  const user = getStoredUser()

  if (to.meta.requiresSystemAdminAccess && !hasSystemAdminAccess(user)) {
    return next({
      path: '/dashboard',
      query: { denied: to.fullPath }
    })
  }

  if (to.meta.requiresProjectSettingsAccess) {
    try {
      const projectId = String(to.params.id || '')
      await axiosClient.get(`/projects/${projectId}/settings`)
    } catch (error) {
      return next({
        path: '/dashboard',
        query: { denied: to.fullPath }
      })
    }
  }

  if (to.meta.requiredRoles && to.meta.requiredRoles.length > 0) {
    try {
      const userRoles = (user.systemRoles || []).map(role => String(role).trim().toLowerCase())
      const requiredRoles = to.meta.requiredRoles.map(role => String(role).trim().toLowerCase())
      const hasPermission = requiredRoles.some(role => userRoles.includes(role))

      if (!hasPermission) {
        return next({
          path: '/dashboard',
          query: { denied: to.fullPath }
        })
      }
    } catch (error) {
      return next({ path: '/dashboard' })
    }
  }

  next()
})

export default router
