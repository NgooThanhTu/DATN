import { defineStore } from 'pinia'

export const useActivityStore = defineStore('activityStore', {
  state: () => ({
    activities: [],
    loading: false,
  }),
  actions: {
    fetchRecentActivities() {
      this.loading = true
      try {
        const stored = localStorage.getItem('nexus_activities')
        if (stored) {
          this.activities = JSON.parse(stored)
        } else {
          // Initialize mock start log
          this.activities = [
            { id: Date.now().toString() + '1', icon: 'fa-regular fa-bell', text: 'Welcome! You can track your activity here.', bold: '', time: new Date().toLocaleString() }
          ]
          localStorage.setItem('nexus_activities', JSON.stringify(this.activities))
        }
      } catch (err) {
        console.error('Failed to load activities', err)
      } finally {
        this.loading = false
      }
    },
    
    logActivity(text, bold, icon = 'fa-regular fa-bell') {
      const newAct = {
        id: Date.now().toString() + Math.floor(Math.random() * 1000),
        icon,
        text,
        bold,
        time: new Date().toLocaleString()
      }
      this.activities.unshift(newAct)
      if (this.activities.length > 50) this.activities.pop() // keep max 50 items
      localStorage.setItem('nexus_activities', JSON.stringify(this.activities))
    }
  }
})
