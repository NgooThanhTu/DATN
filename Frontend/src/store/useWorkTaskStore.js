import { defineStore } from 'pinia'
import axiosClient from '@/api/axiosClient'

export const useWorkTaskStore = defineStore('workTask', {
  state: () => ({
    tasks: [],
    loading: false,
    error: null,
  }),
  getters: {
    backlogTasks: (state) => (state.tasks || []).filter(t => {
       const s = (t.statusName || '').toUpperCase().trim();
       return s === 'BACKLOG' || s === '';
    }).sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)),
    
    todoTasks: (state) => (state.tasks || []).filter(t => {
       const s = (t.statusName || '').toUpperCase().trim();
       return s === 'TODO' || s === 'TO DO';
    }).sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)),
    
    inProgressTasks: (state) => (state.tasks || []).filter(t => {
       const s = (t.statusName || '').toUpperCase().trim();
       return s === 'IN PROGRESS' || s === 'INPROGRESS';
    }).sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)),
    
    reviewTasks: (state) => (state.tasks || []).filter(t => {
       const s = (t.statusName || '').toUpperCase().trim();
       return s === 'IN REVIEW' || s === 'REVIEW';
    }).sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)),

    doneTasks: (state) => (state.tasks || []).filter(t => {
       const s = (t.statusName || '').toUpperCase().trim();
       return s === 'DONE';
    }).sort((a, b) => (a.sortOrder || 0) - (b.sortOrder || 0)),
  },
  actions: {
    async fetchTasks(projectId) {
      if (!projectId) return;
      this.loading = true;
      try {
        const res = await axiosClient.get(`/projects/${projectId}/WorkTasks`);
        this.tasks = res.data?.data || [];
      } catch (err) {
        this.error = err.message;
        console.error('Failed to fetch tasks:', err);
      } finally {
        this.loading = false;
      }
    },
    async createTask(projectId, payload) {
      try {
        const res = await axiosClient.post(`/projects/${projectId}/WorkTasks`, payload);
        await this.fetchTasks(projectId);
        return res.data?.data;
      } catch (err) {
        console.error('Error creating task:', err);
        throw err;
      }
    },
    async updateTaskStatus(projectId, taskId, statusName) {
      try {
        await axiosClient.put(`/projects/${projectId}/WorkTasks/${taskId}/status`, { statusName });
        await this.fetchTasks(projectId);
      } catch (err) {
        throw err;
      }
    },
    async reorderTask(projectId, taskId, sortOrder, newStatusName) {
      try {
        await axiosClient.put(`/projects/${projectId}/WorkTasks/${taskId}/reorder`, { sortOrder, newStatusName });
        await this.fetchTasks(projectId);
      } catch (err) {
        throw err;
      }
    }
  }
})
