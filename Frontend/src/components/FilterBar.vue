<script setup>
import { ref } from 'vue'

const props = defineProps({
  filters: {
    type: Array,
    default: () => [
      { id: 1, label: 'Assignees', condition: 'is', value: '--', icon: 'fa-solid fa-user-group' },
      { id: 2, label: '@Mentions', condition: 'is', value: '--', icon: 'fa-solid fa-at' },
      { id: 3, label: 'Start date', condition: 'is', value: '--', icon: 'fa-regular fa-calendar-plus' },
      { id: 4, label: 'Start date', condition: 'is', value: '--', icon: 'fa-regular fa-calendar-plus' }
    ]
  }
})

const emit = defineEmits(['remove', 'clear', 'add'])

const removeFilter = (id) => {
  emit('remove', id)
}

const clearAll = () => {
  emit('clear')
}

const addFilter = () => {
  emit('add')
}
</script>

<template>
  <div class="filter-bar-container">
    <div class="filters-scroll-area">
      <div v-for="filter in filters" :key="filter.id" class="filter-chip">
        <div class="chip-segment label-sec">
          <i v-if="filter.icon" :class="filter.icon" class="mr-2"></i>
          <span>{{ filter.label }}</span>
        </div>
        <div class="chip-segment condition-sec">
          {{ filter.condition }}
        </div>
        <div class="chip-segment value-sec">
          {{ filter.value }}
        </div>
        <div class="chip-segment remove-sec" @click="removeFilter(filter.id)">
          <i class="fa-solid fa-xmark"></i>
        </div>
      </div>
      
      <button class="add-filter-icon-btn" @click="addFilter">
        <i class="fa-solid fa-filter-circle-plus"></i>
      </button>
    </div>

    <div class="filter-bar-actions" v-if="filters.length > 0">
        <div class="v-divider"></div>
        <button class="clear-all-btn" @click="clearAll">Clear all</button>
    </div>
  </div>
</template>

<style scoped>
.filter-bar-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #16181D;
  border: 1px solid #1E2025;
  border-radius: 12px;
  padding: 8px 12px;
  width: 100%;
  min-height: 48px;
}

.filters-scroll-area {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 10px;
  flex: 1;
}

.filter-chip {
  display: flex;
  align-items: stretch;
  background: #1B1C20;
  border: 1px solid #2D2F36;
  border-radius: 6px;
  overflow: hidden;
  height: 32px;
  transition: all 0.2s;
}

.filter-chip:hover {
  border-color: #3F3F46;
}

.chip-segment {
  display: flex;
  align-items: center;
  padding: 0 10px;
  font-size: 13px;
  border-right: 1px solid #2D2F36;
  white-space: nowrap;
}

.label-sec {
  color: #A1A1AA;
}
.label-sec i {
  font-size: 11px;
}

.condition-sec {
  color: #71717A;
  background: rgba(255, 255, 255, 0.02);
}

.value-sec {
  color: #E4E4E7;
  font-weight: 500;
}

.remove-sec {
  border-right: none;
  cursor: pointer;
  color: #71717A;
  padding: 0 8px;
}

.remove-sec:hover {
  background: rgba(255, 60, 60, 0.1);
  color: #EF4444;
}

.add-filter-icon-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #1B1C20;
  border: 1px solid #2D2F36;
  border-radius: 6px;
  color: #71717A;
  cursor: pointer;
  transition: all 0.2s;
}

.add-filter-icon-btn:hover {
  border-color: #3F3F46;
  color: #fff;
}

.filter-bar-actions {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-left: 12px;
}

.v-divider {
  width: 1px;
  height: 20px;
  background: #2D2F36;
}

.clear-all-btn {
  background: #1B1C20;
  border: 1px solid #2D2F36;
  border-radius: 6px;
  color: #D4D4D8;
  padding: 6px 14px;
  font-size: 13px;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
  transition: all 0.2s;
}

.clear-all-btn:hover {
  background: #27272A;
  border-color: #3F3F46;
  color: #fff;
}
</style>
