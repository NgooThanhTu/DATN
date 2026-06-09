<template>
  <div class="settings-card email-card">
    <div class="card-header">
      <h2 class="card-title">{{ t('Email Addresses', 'Địa chỉ Email') }}</h2>
      <p class="card-subtitle">{{ t('Manage the email addresses associated with your account. Primary email is used for security alerts and notifications.', 'Quản lý các địa chỉ email được liên kết với tài khoản của bạn. Email chính được sử dụng cho các cảnh báo bảo mật và thông báo.') }}</p>
    </div>

    <div class="email-list">
      <div
        v-for="(item, index) in emails"
        :key="item.email"
        class="email-item-card"
        :class="{ 'is-primary': item.isPrimary }"
      >
        <div class="email-details">
          <div class="email-address-row">
            <span class="email-text">{{ item.email }}</span>
            <div class="badge-group">
              <el-tag v-if="item.isPrimary" type="success" effect="dark" size="small">{{ t('Primary', 'Chính') }}</el-tag>
              <span v-if="item.isVerified" class="verified-badge" :title="t('Verified', 'Đã xác minh')">
                <i class="fa-solid fa-circle-check text-blue"></i>
              </span>
              <el-tag v-else type="info" size="small">{{ t('Unverified', 'Chưa xác minh') }}</el-tag>
            </div>
          </div>
          <p class="email-added-date" v-if="item.isPrimary">
            {{ t('Used for account security, log in, and billing.', 'Sử dụng để bảo mật tài khoản, đăng nhập và thanh toán.') }}
          </p>
          <p class="email-added-date" v-else>
            {{ t('Added on', 'Đã thêm ngày') }} 12/04/2026
          </p>
        </div>

        <div class="email-actions">
          <el-button
            v-if="!item.isPrimary && item.isVerified"
            type="primary"
            plain
            size="small"
            @click="makePrimary(index)"
          >
            {{ t('Make primary', 'Đặt làm chính') }}
          </el-button>
          <el-button
            v-if="!item.isPrimary && !item.isVerified"
            type="success"
            plain
            size="small"
            @click="verifyEmail(index)"
          >
            {{ t('Verify', 'Xác minh') }}
          </el-button>
          <el-button
            v-if="!item.isPrimary"
            type="danger"
            plain
            size="small"
            @click="removeEmail(index)"
          >
            <i class="fa-solid fa-trash"></i>
          </el-button>
        </div>
      </div>
    </div>

    <div class="add-email-trigger">
      <el-button type="primary" @click="showAddDialog = true">
        <i class="fa-solid fa-plus mr-2"></i> {{ t('Add email address', 'Thêm địa chỉ email') }}
      </el-button>
    </div>

    <!-- Add Email Dialog -->
    <el-dialog
      v-model="showAddDialog"
      :title="t('Add email address', 'Thêm địa chỉ email')"
      width="450px"
      destroy-on-close
    >
      <div class="add-email-form">
        <label class="field-label">{{ t('New email address', 'Địa chỉ email mới') }}</label>
        <el-input
          v-model="newEmailInput"
          placeholder="example@workmail.com"
          type="email"
          @keyup.enter="handleAddEmail"
        />
        <p class="dialog-desc">
          {{ t('We will send a verification link to this email to confirm ownership.', 'Chúng tôi sẽ gửi một liên kết xác thực tới email này để xác nhận quyền sở hữu.') }}
        </p>
      </div>
      <template #footer>
        <div class="dialog-actions">
          <el-button @click="showAddDialog = false">{{ t('Cancel', 'Hủy') }}</el-button>
          <el-button type="primary" :disabled="!isValidEmail" @click="handleAddEmail">
            {{ t('Add', 'Thêm') }}
          </el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup>
import { ref, computed, watch, onMounted } from 'vue'
import { useLocale } from '@/composables/useLocale'
import { ElMessage } from 'element-plus'

const { t } = useLocale()

const props = defineProps({
  profileData: {
    type: Object,
    required: true
  }
})

const emails = ref([])
const showAddDialog = ref(false)
const newEmailInput = ref('')

const savedEmailsKey = computed(() => `profile_emails_${props.profileData.email || 'guest'}`)

// Load emails
const loadEmails = () => {
  const local = localStorage.getItem(savedEmailsKey.value)
  if (local) {
    try {
      emails.value = JSON.parse(local)
      // Ensure props email is in the list
      const hasMain = emails.value.some(e => e.email.toLowerCase() === props.profileData.email.toLowerCase())
      if (!hasMain && props.profileData.email) {
        emails.value.unshift({
          email: props.profileData.email,
          isPrimary: true,
          isVerified: true
        })
      }
    } catch (e) {
      initDefaults()
    }
  } else {
    initDefaults()
  }
}

const initDefaults = () => {
  emails.value = [
    {
      email: props.profileData.email || 'user@example.com',
      isPrimary: true,
      isVerified: true
    },
    {
      email: 'nguyen.kiet.dev@gmail.com',
      isPrimary: false,
      isVerified: true
    },
    {
      email: 'secondary.kiet@work.com',
      isPrimary: false,
      isVerified: false
    }
  ]
  saveToLocal()
}

const saveToLocal = () => {
  localStorage.setItem(savedEmailsKey.value, JSON.stringify(emails.value))
}

watch(() => props.profileData.email, (newVal) => {
  if (newVal) {
    loadEmails()
  }
}, { immediate: true })

onMounted(() => {
  if (props.profileData.email) {
    loadEmails()
  }
})

const isValidEmail = computed(() => {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return re.test(newEmailInput.value)
})

const handleAddEmail = () => {
  if (!isValidEmail.value) return
  const duplicate = emails.value.some(e => e.email.toLowerCase() === newEmailInput.value.toLowerCase().trim())
  if (duplicate) {
    ElMessage.warning(t('This email is already added.', 'Email này đã được thêm.'))
    return
  }

  emails.value.push({
    email: newEmailInput.value.trim(),
    isPrimary: false,
    isVerified: false
  })
  saveToLocal()
  ElMessage.success(t('Email address added. Please check inbox to verify.', 'Đã thêm địa chỉ email. Vui lòng kiểm tra hộp thư để xác minh.'))
  newEmailInput.value = ''
  showAddDialog.value = false
}

const makePrimary = (index) => {
  emails.value.forEach((e, i) => {
    e.isPrimary = (i === index)
  })
  saveToLocal()
  ElMessage.success(t('Primary email updated successfully.', 'Đã cập nhật email chính thành công.'))
}

const verifyEmail = (index) => {
  emails.value[index].isVerified = true
  saveToLocal()
  ElMessage.success(t('Email verified successfully (mock).', 'Đã xác minh email thành công (mock).'))
}

const removeEmail = (index) => {
  emails.value.splice(index, 1)
  saveToLocal()
  ElMessage.success(t('Email address removed.', 'Đã xóa địa chỉ email.'))
}
</script>

<style scoped>
.email-card {
  background-color: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: 12px !important;
  padding: 32px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.card-header {
  margin-bottom: 8px;
}

.card-title {
  font-size: 24px;
  font-weight: 600;
  color: var(--color-text-primary);
  margin-bottom: 8px;
}

.card-subtitle {
  font-size: 14px;
  color: var(--color-text-secondary);
  line-height: 1.5;
  margin: 0;
}

.email-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.email-item-card {
  border: 1px solid var(--color-border);
  border-radius: 8px;
  padding: 18px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: all 0.2s ease;
  background-color: var(--color-surface);
}

.email-item-card.is-primary {
  border-left: 4px solid #0052cc;
}

.email-details {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.email-address-row {
  display: flex;
  align-items: center;
  gap: 12px;
}

.email-text {
  font-size: 15px;
  font-weight: 600;
  color: var(--color-text-primary);
}

.badge-group {
  display: flex;
  align-items: center;
  gap: 8px;
}

.verified-badge {
  font-size: 14px;
  display: flex;
  align-items: center;
}

.text-blue {
  color: #0c66e4;
}

.email-added-date {
  font-size: 12px;
  color: var(--color-text-secondary);
  margin: 0;
}

.email-actions {
  display: flex;
  align-items: center;
  gap: 8px;
}

.add-email-trigger {
  margin-top: 12px;
}

.add-email-form {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.dialog-desc {
  font-size: 13px;
  color: var(--color-text-secondary);
  margin: 0;
  line-height: 1.5;
}

.mr-2 {
  margin-right: 8px;
}

:deep(.el-input__wrapper) {
  padding: 8px 12px !important;
  height: 40px !important;
  border-radius: 6px !important;
  background-color: var(--color-input-bg) !important;
  border: 1px solid var(--color-input-border) !important;
  box-shadow: none !important;
}

:deep(.el-input__inner) {
  border: none !important;
  background: transparent !important;
  padding: 0 !important;
  height: auto !important;
}

@media (max-width: 600px) {
  .email-item-card {
    flex-direction: column;
    align-items: flex-start;
    gap: 16px;
  }
  
  .email-actions {
    width: 100%;
    justify-content: flex-end;
  }
}
</style>
