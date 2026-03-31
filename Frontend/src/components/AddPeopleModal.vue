<template>
  <el-dialog
    v-model="dialogVisible"
    width="460px"
    class="add-people-dialog"
    :show-close="false"
    append-to-body>
    
    <template #header="{ close }">
      <div class="custom-header">
        <h2 class="dialog-title">Add people</h2>
        <div class="header-actions">
          <div class="icon-action" title="More options"><i class="fa-solid fa-ellipsis"></i></div>
          <div class="icon-action" @click="close" title="Close"><i class="fa-solid fa-xmark"></i></div>
        </div>
      </div>
    </template>

    <div class="dialog-body">
      <div class="form-group">
        <label>Names or emails <span class="required">*</span></label>
        <el-input 
          v-model="emailInput" 
          placeholder="e.g., Maria, maria@company.com" 
          class="clean-input" />
      </div>

      <div class="form-group">
        <label>or add from</label>
        <div class="integration-buttons">
          <button class="integration-btn">
            <svg viewBox="0 0 24 24" width="18" height="18" xmlns="http://www.w3.org/2000/svg">
              <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
              <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
              <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
              <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
              <path d="M1 1h22v22H1z" fill="none"/>
            </svg>
            <span>Google</span>
          </button>
          
          <button class="integration-btn">
            <svg viewBox="0 0 24 24" width="18" height="18" xmlns="http://www.w3.org/2000/svg">
              <path d="M5.042 15.165a2.528 2.528 0 0 1-2.52 2.523A2.528 2.528 0 0 1 0 15.165a2.527 2.527 0 0 1 2.522-2.52h2.52v2.52z" fill="#E01E5A"/>
              <path d="M6.313 15.165a2.527 2.527 0 0 1 2.521-2.52 2.527 2.527 0 0 1 2.521 2.52v6.313A2.528 2.528 0 0 1 8.834 24a2.528 2.528 0 0 1-2.521-2.522v-6.313z" fill="#E01E5A"/>
              <path d="M8.834 5.042a2.528 2.528 0 0 1-2.521-2.52A2.528 2.528 0 0 1 8.834 0a2.528 2.528 0 0 1 2.521 2.522v2.52H8.834z" fill="#36C5F0"/>
              <path d="M8.834 6.313a2.528 2.528 0 0 1 2.521 2.521 2.528 2.528 0 0 1-2.521 2.521H2.522A2.528 2.528 0 0 1 0 8.834a2.528 2.528 0 0 1 2.522-2.521h6.312z" fill="#36C5F0"/>
              <path d="M18.956 8.834a2.528 2.528 0 0 1 2.522-2.521A2.528 2.528 0 0 1 24 8.834a2.528 2.528 0 0 1-2.522 2.521h-2.522V8.834z" fill="#2EB67D"/>
              <path d="M17.688 8.834a2.528 2.528 0 0 1-2.523 2.521 2.527 2.527 0 0 1-2.52-2.521V2.522A2.527 2.527 0 0 1 15.165 0a2.528 2.528 0 0 1 2.523 2.522v6.312z" fill="#2EB67D"/>
              <path d="M15.165 18.956a2.528 2.528 0 0 1 2.523 2.522A2.528 2.528 0 0 1 15.165 24a2.527 2.527 0 0 1-2.52-2.522v-2.522h2.52z" fill="#ECB22E"/>
              <path d="M15.165 17.688a2.527 2.527 0 0 1-2.52-2.523 2.526 2.526 0 0 1 2.52-2.52h6.313A2.527 2.527 0 0 1 24 15.165a2.528 2.528 0 0 1-2.522 2.523h-6.313z" fill="#ECB22E"/>
            </svg>
            <span>Slack</span>
          </button>
          
          <button class="integration-btn">
            <svg viewBox="0 0 24 24" width="18" height="18" xmlns="http://www.w3.org/2000/svg">
              <path fill="#f25022" d="M1.385 1.385h10.154v10.154H1.385z"/>
              <path fill="#7fba00" d="M12.461 1.385h10.154v10.154H12.461z"/>
              <path fill="#00a4ef" d="M1.385 12.462h10.154v10.154H1.385z"/>
              <path fill="#ffb900" d="M12.461 12.462h10.154v10.154H12.461z"/>
            </svg>
            <span>Microsoft</span>
          </button>
        </div>
      </div>

      <div class="form-group role-group">
        <label>Role <span class="required">*</span></label>
        <div class="select-wrapper">
          <select v-model="selectedRole" class="block-select">
            <option value="Member">Member</option>
            <option value="Admin">Admin</option>
            <option value="Guest">Guest</option>
          </select>
          <i class="fa-solid fa-chevron-down select-chevron"></i>
        </div>
      </div>

      <div class="recaptcha-text">
        This site is protected by reCAPTCHA and the Google 
        <a href="#" target="_blank" class="external-link">Privacy Policy <i class="fa-solid fa-arrow-up-right-from-square small-icon"></i></a> and 
        <a href="#" target="_blank" class="external-link">Terms of Service <i class="fa-solid fa-arrow-up-right-from-square small-icon"></i></a> apply.
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <div class="footer-left">
          <button class="link-btn" @click="copyLink">
            <i class="fa-solid fa-link"></i> Copy link
          </button>
        </div>
        <div class="footer-right">
          <button class="cancel-btn" @click="closeDialog">Cancel</button>
          <button class="add-btn" @click="submitAdd">Add</button>
        </div>
      </div>
    </template>
  </el-dialog>
</template>

<script setup>
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:visible', 'added'])

const dialogVisible = ref(props.visible)
const emailInput = ref('')
const selectedRole = ref('Member')

// Watch for external prop changes
import { watch } from 'vue'
watch(() => props.visible, (newVal) => {
  dialogVisible.value = newVal
  if (newVal) {
    emailInput.value = ''
    selectedRole.value = 'Member'
  }
})

// Sync internal visibility to external
watch(dialogVisible, (newVal) => {
  emit('update:visible', newVal)
})

const closeDialog = () => {
  dialogVisible.value = false
}

const submitAdd = () => {
  if (!emailInput.value.trim()) {
    return
  }
  emit('added', { 
    emails: emailInput.value, 
    role: selectedRole.value 
  })
  ElMessage.success('People added successfully')
  closeDialog()
}

const copyLink = () => {
  navigator.clipboard.writeText(window.location.href)
  ElMessage.success('Link copied to clipboard')
}
</script>

<style scoped>
.add-people-dialog :deep(.el-dialog) {
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0,0,0,0.15);
}

.add-people-dialog :deep(.el-dialog__header) {
  padding: 0;
  margin: 0;
}

.add-people-dialog :deep(.el-dialog__body) {
  padding: 0;
}

.add-people-dialog :deep(.el-dialog__footer) {
  padding: 0;
}

.custom-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24px 24px 16px;
}

.dialog-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #1c1c1c;
  font-family: 'Inter', -apple-system, sans-serif;
}

.header-actions {
  display: flex;
  gap: 8px;
}

.icon-action {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  cursor: pointer;
  color: #555;
  transition: background-color 0.2s;
  font-size: 16px;
}

.icon-action:hover {
  background-color: #f1f2f4;
  color: #172b4d;
}

.dialog-body {
  padding: 0 24px;
}

.form-group {
  margin-bottom: 24px;
}

.form-group label {
  display: block;
  font-size: 14px;
  font-weight: 600;
  color: #5e6c84;
  margin-bottom: 8px;
}

.required {
  color: #de350b;
}

.clean-input :deep(.el-input__wrapper) {
  box-shadow: inset 0 0 0 1px #dfe1e6 !important;
  border-radius: 4px;
  padding: 0 12px;
}

.clean-input :deep(.el-input__wrapper.is-focus) {
  box-shadow: inset 0 0 0 2px #2684FF !important;
}

.clean-input :deep(.el-input__inner) {
  height: 48px;
  line-height: 48px;
  font-size: 15px;
  color: #172b4d;
}

.clean-input :deep(.el-input__inner::placeholder) {
  color: #7a869a;
}

.integration-buttons {
  display: flex;
  gap: 12px;
}

.integration-btn {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  height: 40px;
  background: white;
  border: 1px solid #dfe1e6;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  color: #172b4d;
  transition: background-color 0.2s;
}

.integration-btn:hover {
  background-color: #fafbfc;
}

.role-group {
  margin-bottom: 16px;
}

.select-wrapper {
  position: relative;
}

.block-select {
  width: 100%;
  height: 48px;
  padding: 0 12px;
  font-size: 15px;
  color: #172b4d;
  background-color: white;
  border: 1px solid #dfe1e6;
  border-radius: 4px;
  appearance: none;
  cursor: pointer;
  outline: none;
}

.block-select:focus {
  border-color: #2684FF;
  box-shadow: 0 0 0 1px #2684FF;
}

.select-chevron {
  position: absolute;
  right: 16px;
  top: 50%;
  transform: translateY(-50%);
  color: #5e6c84;
  pointer-events: none;
  font-size: 12px;
}

.recaptcha-text {
  font-size: 13px;
  color: #5e6c84;
  line-height: 1.5;
  margin-bottom: 24px;
}

.external-link {
  color: #0052cc;
  text-decoration: none;
}

.external-link:hover {
  text-decoration: underline;
}

.small-icon {
  font-size: 10px;
  margin-left: 2px;
}

.dialog-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16px 24px 24px;
  border-top: none;
}

.link-btn {
  background: transparent;
  border: none;
  color: #42526e;
  font-size: 14px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  padding: 8px;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.link-btn:hover {
  background: #f4f5f7;
}

.link-btn i {
  font-size: 16px;
}

.footer-right {
  display: flex;
  gap: 12px;
}

.cancel-btn {
  background: transparent;
  border: none;
  color: #42526e;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 4px;
}

.cancel-btn:hover {
  background: #f4f5f7;
}

.add-btn {
  background: #0052cc;
  color: white;
  border: none;
  font-size: 14px;
  font-weight: 600;
  padding: 0 24px;
  height: 36px;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.add-btn:hover {
  background: #0047b3;
}
</style>
