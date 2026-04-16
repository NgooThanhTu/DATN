import { defineStore } from 'pinia'
import axiosClient from '@/api/axiosClient'

export const useActivityStore = defineStore('activityStore', {
  state: () => ({
    activities: [],
    loading: false,
  }),
  actions: {
    async fetchRecentActivities() {
      this.loading = true
      try {
        // Fallback: If we don't have a direct /activities endpoint, 
        // we derive activities from /tasks/my-tasks for now to show real dynamic data.
        const res = await axiosClient.get('/tasks/my-tasks')
        const tasks = res.data?.data || []
        
        // Map tasks to simple activities for display
        const mappedLogs = tasks.slice(0, 5).map(t => {
          return {
            id: 'act-' + t.id,
            icon: 'fa-regular fa-bell',
            text: `You were assigned to`,
            bold: t.sequenceId || t.title,
            time: 'Recently'
          }
        })
        
        this.activities = mappedLogs
      } catch (err) {
        console.error('Failed to load activities', err)
      } finally {
        this.loading = false
      }
    }
  }
})
