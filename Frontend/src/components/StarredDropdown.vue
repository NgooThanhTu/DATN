<template>
  <div class="jd-content">
    <div class="jd-header">
      <h3>Starred</h3>
    </div>

    <div class="jd-body">
      <div v-if="starredItems.length === 0" class="jd-empty-starred">
        <img src="https://jira-frontend-bifrost.prod-east.frontend.public.atl-paas.net/assets/starred-empty.svg" alt="Empty Starred" onerror="this.style.display='none'" />
        <h4>You haven't starred anything yet</h4>
        <p>Mark items that are important to you with a star to quickly access them here.</p>
      </div>

      <div v-else class="jd-list">
        <div 
          v-for="item in starredItems" 
          :key="item.id" 
          class="jd-item"
          @click="goToItem(item)"
        >
          <div class="jd-item-icon">
            <i class="fa-solid fa-square-check text-blue-500"></i>
          </div>
          <div class="jd-item-content">
            <div class="jd-item-title">{{ item.title || 'Task' }}</div>
            <div class="jd-item-subtitle">{{ item.projectName || 'Project' }}</div>
          </div>
          <div class="jd-item-action" @click.stop="unstarItem(item)">
            <i class="fa-solid fa-star text-yellow-400"></i>
          </div>
        </div>
      </div>
    </div>

    <div class="jd-footer" v-if="starredItems.length > 0">
      <button @click="viewAllStarred">View all starred items</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useWorkTaskStore } from '@/store/useWorkTaskStore'

const emit = defineEmits(['close'])
const router = useRouter()
const workTaskStore = useWorkTaskStore()
const starredItems = ref([])

const loadStarredItems = () => {
  const starredIds = workTaskStore.starredTaskIds
  if (!starredIds.length) {
    starredItems.value = []
    return
  }

  try {
    const data = localStorage.getItem('recently_viewed_tasks')
    const recent = data ? JSON.parse(data) : []
    
    starredItems.value = starredIds.map(id => {
      const cached = recent.find(r => r.id === id)
      return cached || { id, title: 'Starred Task ' + id.substring(0,6), projectId: '1' }
    })
  } catch (e) {
    starredItems.value = []
  }
}

// Expose the load method
defineExpose({
  loadStarredItems
})

const unstarItem = (item) => {
  workTaskStore.toggleTaskStar(item.id)
  starredItems.value = starredItems.value.filter(i => i.id !== item.id)
}

const goToItem = (item) => {
  emit('close')
  router.push(`/space/${item.projectId}?task=${item.id}`)
}

const viewAllStarred = () => {
  emit('close')
  router.push('/dashboard?tab=starred')
}

onMounted(() => {
  loadStarredItems()
})
</script>

<style scoped>
.jd-content {
  display: flex;
  flex-direction: column;
  background: var(--color-surface, #ffffff);
  color: var(--color-text-primary, #172b4d);
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
}

.jd-header {
  padding: 12px 16px 8px;
}
.jd-header h3 {
  margin: 0;
  font-size: 11px;
  text-transform: uppercase;
  color: var(--color-text-muted, #6b778c);
  font-weight: 700;
  letter-spacing: 0.5px;
}

.jd-body {
  flex: 1;
  overflow-y: auto;
  max-height: 320px;
}

.jd-empty-starred {
  text-align: center;
  padding: 24px 16px;
}
.jd-empty-starred img {
  width: 100px;
  margin: 0 auto 16px;
  opacity: 0.7;
}
.jd-empty-starred h4 {
  font-size: 14px;
  font-weight: 600;
  margin: 0 0 6px 0;
  color: var(--color-text-primary, #172b4d);
}
.jd-empty-starred p {
  font-size: 12px;
  color: var(--color-text-muted, #6b778c);
  margin: 0;
  line-height: 1.4;
}

.jd-list {
  padding-bottom: 8px;
}

.jd-item {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  cursor: pointer;
  transition: background 0.1s;
}
.jd-item:hover {
  background: var(--color-surface-hover, #f4f5f7);
}

.jd-item-icon {
  margin-right: 12px;
  font-size: 16px;
  display: flex;
  align-items: center;
}

.jd-item-content {
  flex: 1;
  min-width: 0;
}

.jd-item-title {
  font-size: 13px;
  font-weight: 500;
  color: var(--color-text-primary, #172b4d);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.jd-item-subtitle {
  font-size: 11px;
  color: var(--color-text-muted, #6b778c);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.jd-item-action {
  margin-left: 12px;
  font-size: 14px;
  padding: 4px;
  border-radius: 3px;
  display: flex;
  align-items: center;
}
.jd-item-action:hover {
  background: rgba(9, 30, 66, 0.08);
}

.jd-footer {
  padding: 8px 16px;
  border-top: 1px solid var(--color-border, #ebecf0);
}

.jd-footer button {
  width: 100%;
  text-align: left;
  background: transparent;
  border: none;
  color: var(--color-accent, #0c66e4);
  font-size: 13px;
  font-weight: 500;
  padding: 8px;
  border-radius: 3px;
  cursor: pointer;
}
.jd-footer button:hover {
  background: var(--color-surface-hover, #f4f5f7);
  text-decoration: underline;
}
</style>
