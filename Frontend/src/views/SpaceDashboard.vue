<template>
  <NexusLayout>
    <div class="space-dashboard-page p-6">
      <header class="mb-6 flex justify-between items-center">
        <div>
          <h1 class="text-2xl font-bold text-[var(--color-text-primary)]">Dashboard</h1>
          <p class="text-[var(--color-text-muted)] text-sm mt-1">Project overview and quick insights</p>
        </div>
      </header>
      
      <div v-if="loading" class="dashboard-grid">
        <SprintaSkeleton height="88px" rounded="md" v-for="i in 4" :key="i" />
        <SprintaSkeleton class="col-span-2" height="200px" rounded="md" />
        <SprintaSkeleton class="col-span-2" height="200px" rounded="md" />
      </div>

      <div v-else class="dashboard-grid">
        <div class="stat-card">
          <div class="stat-icon bg-blue-500/10 text-blue-500"><ListTodo class="w-5 h-5" /></div>
          <div class="stat-info">
            <span class="stat-value">12</span>
            <span class="stat-label">Open Tasks</span>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon bg-green-500/10 text-green-500"><CheckCircle2 class="w-5 h-5" /></div>
          <div class="stat-info">
            <span class="stat-value">45</span>
            <span class="stat-label">Completed</span>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon bg-yellow-500/10 text-yellow-500"><Clock class="w-5 h-5" /></div>
          <div class="stat-info">
            <span class="stat-value">3</span>
            <span class="stat-label">In Progress</span>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon bg-red-500/10 text-red-500"><AlertTriangle class="w-5 h-5" /></div>
          <div class="stat-info">
            <span class="stat-value">2</span>
            <span class="stat-label">Blocked</span>
          </div>
        </div>
        
        <div class="dashboard-panel col-span-2">
          <h3 class="font-semibold text-lg mb-4 text-[var(--color-text-primary)]">Recent Tasks</h3>
          <SprintaEmptyState
            icon="Inbox"
            title="No recent tasks"
            description="Get started by creating a new task in your board or backlog."
            class="h-[180px]"
          />
        </div>

        <div class="dashboard-panel col-span-2">
          <h3 class="font-semibold text-lg mb-4 text-[var(--color-text-primary)]">Team Workload</h3>
          <SprintaEmptyState
            icon="Users"
            title="Workload distribution"
            description="Assign tasks to your team members to see their workload here."
            class="h-[180px]"
          />
        </div>
      </div>
    </div>
  </NexusLayout>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import NexusLayout from '@/components/layout/NexusLayout.vue'
import SprintaSkeleton from '@/components/ui/SprintaSkeleton.vue'
import SprintaEmptyState from '@/components/ui/SprintaEmptyState.vue'
import { ListTodo, CheckCircle2, Clock, AlertTriangle } from 'lucide-vue-next'

const loading = ref(false)

onMounted(() => {
  loading.value = true
  setTimeout(() => {
    loading.value = false
  }, 500)
})
</script>

<style scoped>
.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24px;
}
.col-span-2 {
  grid-column: span 2;
}
.stat-card {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  box-shadow: var(--shadow-sm);
}
.stat-icon {
  width: 48px;
  height: 48px;
  border-radius: var(--radius-md);
  display: flex;
  align-items: center;
  justify-content: center;
}
.stat-info {
  display: flex;
  flex-direction: column;
}
.stat-value {
  font-size: 24px;
  font-weight: 700;
  line-height: 1.2;
  color: var(--color-text-primary);
}
.stat-label {
  font-size: 13px;
  color: var(--color-text-muted);
}
.dashboard-panel {
  background: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: var(--radius-md);
  padding: 24px;
  box-shadow: var(--shadow-sm);
}
@media (max-width: 1024px) {
  .dashboard-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
@media (max-width: 640px) {
  .dashboard-grid {
    grid-template-columns: 1fr;
  }
  .col-span-2 {
    grid-column: span 1;
  }
}
</style>
