<script setup>
import { computed, ref } from 'vue'

const props = defineProps({
  filters: {
    type: Array,
    default: () => []
  }
})

const emit = defineEmits(['remove', 'clear', 'add', 'add-filter', 'apply', 'update:filters'])

const showBuilder = ref(false)
const draft = ref({ field: 'status', operator: 'is', value: '' })

const filterFields = [
  { key: 'status', label: 'Status', icon: 'fa-regular fa-circle-dot', values: ['BACKLOG', 'TO DO', 'IN PROGRESS', 'IN REVIEW', 'DONE'] },
  { key: 'assignee', label: 'Assignee', icon: 'fa-regular fa-user', values: ['Unassigned'] },
  { key: 'creator', label: 'Creator', icon: 'fa-regular fa-user', values: ['Me'] },
  { key: 'priority', label: 'Priority', icon: 'fa-solid fa-signal', values: ['Urgent', 'High', 'Normal', 'Low', 'None'] },
  { key: 'label', label: 'Label', icon: 'fa-solid fa-tag', values: ['No label'] },
  { key: 'startDate', label: 'Start date', icon: 'fa-regular fa-calendar-plus', values: ['Today', 'This week', 'Empty'] },
  { key: 'dueDate', label: 'Due date', icon: 'fa-regular fa-calendar', values: ['Today', 'This week', 'Overdue', 'Empty'] },
  { key: 'cycle', label: 'Cycle', icon: 'fa-solid fa-arrows-spin', values: ['No cycle'] },
  { key: 'module', label: 'Module', icon: 'fa-solid fa-table-cells-large', values: ['No module'] },
  { key: 'createdAt', label: 'Created at', icon: 'fa-regular fa-calendar', values: ['Today', 'This week'] },
  { key: 'updatedAt', label: 'Updated at', icon: 'fa-regular fa-calendar', values: ['Today', 'This week'] }
]

const operatorsByField = {
  status: ['is', 'is not', 'in', 'not in'],
  assignee: ['is', 'is not', 'empty', 'not empty'],
  creator: ['is', 'is not'],
  priority: ['is', 'is not', 'in'],
  label: ['includes', 'not includes', 'empty'],
  startDate: ['before', 'after', 'between', 'empty'],
  dueDate: ['before', 'after', 'between', 'empty', 'overdue'],
  cycle: ['is', 'is not', 'empty'],
  module: ['is', 'is not', 'empty'],
  createdAt: ['before', 'after', 'between'],
  updatedAt: ['before', 'after', 'between']
}

const selectedField = computed(() => filterFields.find(field => field.key === draft.value.field) || filterFields[0])
const availableOperators = computed(() => operatorsByField[draft.value.field] || ['is'])
const valueRequired = computed(() => !['empty', 'not empty', 'overdue'].includes(draft.value.operator))

const removeFilter = (id) => {
  const next = props.filters.filter(filter => filter.id !== id)
  emit('update:filters', next)
  emit('remove', id)
  emit('apply', next)
}

const clearAll = () => {
  emit('update:filters', [])
  emit('clear')
  emit('apply', [])
}

const addFilter = () => {
  emit('add')
  showBuilder.value = !showBuilder.value
}

const applyFilter = () => {
  if (valueRequired.value && !draft.value.value) return

  const filter = {
    id: `${draft.value.field}-${Date.now()}`,
    field: draft.value.field,
    operator: draft.value.operator,
    value: valueRequired.value ? draft.value.value : '',
    label: selectedField.value.label,
    condition: draft.value.operator,
    displayValue: valueRequired.value ? draft.value.value : draft.value.operator,
    icon: selectedField.value.icon
  }

  const next = [...props.filters, filter]
  emit('update:filters', next)
  emit('add', filter)
  emit('add-filter', filter)
  emit('apply', next)
  showBuilder.value = false
  draft.value = { field: 'status', operator: 'is', value: '' }
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
          {{ filter.displayValue || filter.value || '--' }}
        </div>
        <button class="chip-segment remove-sec" type="button" @click="removeFilter(filter.id)" aria-label="Remove filter">
          <i class="fa-solid fa-xmark"></i>
        </button>
      </div>

      <span v-if="filters.length === 0" class="empty-filter-copy">No filters applied</span>
      
      <div class="add-filter-wrapper">
        <button class="add-filter-icon-btn" type="button" @click="addFilter" :class="{ active: showBuilder }" :aria-expanded="showBuilder">
          <i class="fa-solid fa-filter-circle-plus"></i>
        </button>

        <div class="filter-builder" v-if="showBuilder" @click.stop>
          <label>
            <span>Field</span>
            <select v-model="draft.field" @change="draft.operator = availableOperators[0]; draft.value = ''">
              <option v-for="field in filterFields" :key="field.key" :value="field.key">{{ field.label }}</option>
            </select>
          </label>

          <label>
            <span>Operator</span>
            <select v-model="draft.operator" @change="draft.value = ''">
              <option v-for="operator in availableOperators" :key="operator" :value="operator">{{ operator }}</option>
            </select>
          </label>

          <label v-if="valueRequired">
            <span>Value</span>
            <select v-model="draft.value">
              <option value="" disabled>Select value</option>
              <option v-for="value in selectedField.values" :key="value" :value="value">{{ value }}</option>
            </select>
          </label>

          <button class="apply-filter-btn" type="button" @click="applyFilter" :disabled="valueRequired && !draft.value">Apply filter</button>
        </div>
      </div>
    </div>

    <div class="filter-bar-actions" v-if="filters.length > 0">
        <div class="v-divider"></div>
        <button class="clear-all-btn" type="button" @click="clearAll">Clear all</button>
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

.empty-filter-copy {
  color: #71717A;
  font-size: 13px;
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
  border-top: 0;
  border-bottom: 0;
  border-left: 0;
  cursor: pointer;
  color: #71717A;
  padding: 0 8px;
  background: transparent;
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

.add-filter-icon-btn.active {
  border-color: #38BDF8;
  color: #fff;
}

.add-filter-wrapper {
  position: relative;
}

.filter-builder {
  position: absolute;
  top: calc(100% + 8px);
  left: 0;
  z-index: 20;
  width: 260px;
  background: #1B1C20;
  border: 1px solid #2D2F36;
  border-radius: 8px;
  padding: 12px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.35);
}

.filter-builder label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  color: #A1A1AA;
  font-size: 12px;
}

.filter-builder select {
  background: #111315;
  border: 1px solid #2D2F36;
  border-radius: 6px;
  color: #E4E4E7;
  padding: 8px;
  outline: none;
}

.apply-filter-btn {
  background: #0EA5E9;
  border: none;
  border-radius: 6px;
  color: #fff;
  cursor: pointer;
  font-size: 13px;
  font-weight: 600;
  padding: 8px 10px;
}

.apply-filter-btn:disabled {
  cursor: not-allowed;
  opacity: 0.55;
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
