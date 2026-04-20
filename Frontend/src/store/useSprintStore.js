import { defineStore } from 'pinia'
import axiosClient from '@/api/axiosClient'

export const useSprintStore = defineStore('sprint', {
  state: () => ({
    sprints: [],
    activeSprint: null,
    loading: false,
    error: null
  }),
  actions: {
    async fetchSprints(projectId) {
      if (!projectId) return;
      this.loading = true;
      try {
        const res = await axiosClient.get(`/projects/${projectId}/sprints`);
        this.sprints = res.data?.data || [];
        this.activeSprint = this.sprints.find(s => (s.state || '').toLowerCase() === 'active') || null;
      } catch (err) {
        this.error = err.message;
        console.error('Failed to fetch sprints:', err);
      } finally {
        this.loading = false;
      }
    },
    async toggleFavorite(projectId, sprintId) {
      if(!projectId || !sprintId) return;
      try {
        const res = await axiosClient.patch(`/projects/${projectId}/sprints/${sprintId}/favorite`);
        const idx = this.sprints.findIndex(s => s.id === sprintId);
        if (idx !== -1) {
           this.sprints[idx].isFavorite = res.data?.data?.isFavorite;
        }
      } catch (err) {
        console.error('Failed to toggle sprint favorite:', err);
      }
    }
  }
})
