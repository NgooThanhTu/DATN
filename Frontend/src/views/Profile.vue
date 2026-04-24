<template>
  <NexusLayout class="profile-page">
    <input ref="avatarInput" type="file" style="display: none" accept="image/*" @change="uploadAvatar" />
    <input ref="coverInput" type="file" style="display: none" accept="image/*" @change="uploadCover" />

    <div class="profile-body-container">
      <div class="profile-container" v-loading="isLoading">
        <div class="profile-header-section sharp-card">
          <div class="header-image-box" :style="coverBannerStyle" @click="triggerCoverUpload" title="Click to change cover">
            <div class="avatar-inside-wrapper" @click.stop>
              <el-dropdown trigger="click" @command="handleAvatarCommand">
                <div class="large-profile-avatar" :style="avatarStyle">
                  {{ profileData.avatarUrl ? '' : getInitials(profileData.fullName) }}
                  <div class="avatar-hover-overlay">
                    <i class="fa-solid fa-camera"></i>
                  </div>
                </div>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item command="upload">
                      <i class="fa-solid fa-plus"></i> Add profile photo
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </div>

            <div class="banner-upload-prompt" v-if="!profileData.coverUrl">
              <i class="fa-regular fa-image"></i>
              <span>Add cover</span>
            </div>
          </div>

          <div class="header-footer">
            <div class="profile-name-block">
              <strong>{{ profileData.publicName || profileData.fullName || 'Member' }}</strong>
              <span>{{ profileData.jobTitle || 'Update your job title' }}</span>
            </div>
          </div>
        </div>

        <div class="profile-content-form">
          <h2 class="section-title">About you</h2>

          <div class="form-grid">
            <div class="form-row">
              <div class="field-label">Full name</div>
              <div class="field-input-wrapper">
                <el-input v-model="profileData.fullName" />
              </div>
            </div>

            <div class="form-row">
              <div class="field-label">Public name <i class="fa-solid fa-circle-info info-icon"></i></div>
              <div class="field-input-wrapper">
                <el-input v-model="profileData.publicName" />
              </div>
            </div>

            <div class="form-row">
              <div class="field-label">Job title</div>
              <div class="field-input-wrapper">
                <el-input v-model="profileData.jobTitle" placeholder="Your job title" />
              </div>
            </div>

            <div class="form-row">
              <div class="field-label">Department</div>
              <div class="field-input-wrapper">
                <el-input v-model="profileData.department" placeholder="Your department" />
              </div>
            </div>

            <div class="form-row">
              <div class="field-label">Organization</div>
              <div class="field-input-wrapper">
                <el-input v-model="profileData.organization" placeholder="Your organization" />
              </div>
            </div>

            <div class="form-row">
              <div class="field-label">Working with you</div>
              <div class="field-input-wrapper">
                <el-input
                  v-model="profileData.collaboration"
                  type="textarea"
                  :rows="3"
                  placeholder="Help teammates understand how to work with you better"
                />
              </div>
            </div>

            <div class="form-row">
              <div class="field-label"></div>
              <div class="field-input-wrapper save-row">
                <el-button type="primary" :loading="isSaving" @click="saveProfile">Save changes</el-button>
              </div>
            </div>
          </div>

          <h2 class="section-title mt-40">Contact</h2>
          <div class="contact-card">
            <div class="form-row">
              <div class="field-label">Email address</div>
              <div class="field-input-wrapper">
                <div class="email-value">{{ profileData.email }}</div>
              </div>
            </div>
          </div>

          <h2 class="section-title mt-40">Security</h2>
          <div class="password-card">
            <div v-if="passwordStep === 1" class="password-step">
              <div class="step-indicator">
                <div class="step-badge active">1</div>
                <div class="step-line"></div>
                <div class="step-badge">2</div>
              </div>

              <h3 class="step-title"><i class="fa-solid fa-envelope-circle-check"></i> Verify identity</h3>
              <p class="step-desc">Send an OTP code to your account email before changing your password.</p>
              <div v-if="isPasswordChangeLocked" class="cooldown-alert">
                <i class="fa-solid fa-shield-halved"></i>
                <div>
                  <strong>Password changes are limited to once every 7 days.</strong>
                  <p>You can change your password again after {{ passwordEligibleLabel }}. If this is urgent, send a request to the system admin.</p>
                </div>
              </div>

              <div class="form-row compact-password-row">
                <div class="field-label">Account email</div>
                <div class="field-input-wrapper">
                  <el-input :model-value="profileData.email" disabled>
                    <template #prefix>
                      <i class="fa-solid fa-at"></i>
                    </template>
                  </el-input>
                </div>
              </div>

              <div v-if="passwordOtpSent" class="form-row compact-password-row mt-16">
                <div class="field-label">OTP code</div>
                <div class="field-input-wrapper">
                  <el-input v-model="passwordForm.otpCode" maxlength="6" placeholder="Enter 6-digit OTP">
                    <template #prefix>
                      <i class="fa-solid fa-key"></i>
                    </template>
                  </el-input>
                  <div class="otp-hint">
                    <i class="fa-solid fa-circle-info"></i>
                    OTP is valid for 5 minutes.
                    <a v-if="!isSendingPasswordOtp && canResendPasswordOtp" href="#" @click.prevent="sendPasswordOtp">Resend code</a>
                    <span v-if="!canResendPasswordOtp">({{ passwordCountdownText }})</span>
                  </div>
                </div>
              </div>

              <div class="password-actions">
                <el-button
                  v-if="isPasswordChangeLocked"
                  type="primary"
                  :loading="isRequestingPasswordException"
                  @click="requestPasswordChangeException"
                >
                  <i class="fa-solid fa-envelope"></i>
                  Request admin support
                </el-button>
                <el-button
                  v-else-if="!passwordOtpSent"
                  type="primary"
                  :loading="isSendingPasswordOtp"
                  :disabled="!profileData.email"
                  @click="sendPasswordOtp"
                >
                  <i class="fa-solid fa-paper-plane"></i>
                  Send verification code
                </el-button>
                <el-button
                  v-else
                  type="primary"
                  :disabled="!passwordForm.otpCode || passwordForm.otpCode.length < 6"
                  @click="passwordStep = 2"
                >
                  <i class="fa-solid fa-arrow-right"></i>
                  Verify & continue
                </el-button>
              </div>
            </div>

            <div v-else-if="passwordStep === 2" class="password-step">
              <div class="step-indicator">
                <div class="step-badge done"><i class="fa-solid fa-check"></i></div>
                <div class="step-line done"></div>
                <div class="step-badge active">2</div>
              </div>

              <div class="verified-badge">
                <i class="fa-solid fa-circle-check"></i>
                <span>Email verified: <strong>{{ profileData.email }}</strong></span>
              </div>

              <h3 class="step-title mt-24"><i class="fa-solid fa-lock"></i> Create new password</h3>
              <p class="step-desc">Use a strong password that you do not use elsewhere.</p>

              <div class="form-row compact-password-row">
                <div class="field-label">New password</div>
                <div class="field-input-wrapper">
                  <el-input v-model="passwordForm.newPassword" type="password" show-password placeholder="Create a new password" @input="checkPasswordStrength" />
                  <div v-if="passwordForm.newPassword" class="password-strength">
                    <div class="strength-bars">
                      <div class="bar" :class="passwordStrength > 0 ? passwordStrengthClass : ''"></div>
                      <div class="bar" :class="passwordStrength > 1 ? passwordStrengthClass : ''"></div>
                      <div class="bar" :class="passwordStrength > 2 ? passwordStrengthClass : ''"></div>
                      <div class="bar" :class="passwordStrength > 3 ? passwordStrengthClass : ''"></div>
                    </div>
                    <span class="strength-text" :class="passwordStrengthClass">{{ passwordStrengthLabel }}</span>
                  </div>
                  <ul class="password-hints">
                    <li :class="{ passed: passwordHints.length }"><i class="fa-solid" :class="passwordHints.length ? 'fa-check' : 'fa-circle-dot'"></i> At least 8 characters</li>
                    <li :class="{ passed: passwordHints.uppercase }"><i class="fa-solid" :class="passwordHints.uppercase ? 'fa-check' : 'fa-circle-dot'"></i> One uppercase letter</li>
                    <li :class="{ passed: passwordHints.number }"><i class="fa-solid" :class="passwordHints.number ? 'fa-check' : 'fa-circle-dot'"></i> One number</li>
                    <li :class="{ passed: passwordHints.special }"><i class="fa-solid" :class="passwordHints.special ? 'fa-check' : 'fa-circle-dot'"></i> One special character</li>
                  </ul>
                </div>
              </div>

              <div class="form-row compact-password-row mt-16">
                <div class="field-label">Confirm password</div>
                <div class="field-input-wrapper">
                  <el-input v-model="passwordForm.confirmPassword" type="password" show-password placeholder="Re-enter your password" />
                  <div v-if="passwordForm.confirmPassword && passwordForm.newPassword !== passwordForm.confirmPassword" class="error-msg">
                    Passwords do not match.
                  </div>
                </div>
              </div>

              <div class="logout-option">
                <el-checkbox v-model="passwordForm.logoutOthers">
                  <span class="checkbox-title">Log out from all other devices</span>
                </el-checkbox>
              </div>

              <div class="password-actions between">
                <el-button @click="passwordStep = 1">
                  <i class="fa-solid fa-arrow-left"></i>
                  Go back
                </el-button>
                <el-button type="primary" :disabled="!canSubmitPassword" :loading="isChangingPassword" @click="changePassword">
                  <i class="fa-solid fa-floppy-disk"></i>
                  Update password
                </el-button>
              </div>
            </div>

            <div v-else class="password-step success-section">
              <div class="success-icon-wrapper">
                <i class="fa-solid fa-circle-check"></i>
              </div>
              <h3 class="success-title">Password changed successfully!</h3>
              <p class="success-desc">Your password has been updated securely.</p>
              <el-button type="primary" @click="resetPasswordFlow">
                <i class="fa-solid fa-rotate-left"></i>
                Change password again
              </el-button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </NexusLayout>
</template>

<script setup>
import { computed, onMounted, onUnmounted, reactive, ref } from 'vue'
import NexusLayout from '@/components/layout/NexusLayout.vue'
import axiosClient from '@/api/axiosClient'
import { ElMessage } from 'element-plus'

const isLoading = ref(false)
const isSaving = ref(false)
const avatarInput = ref(null)
const coverInput = ref(null)

const profileData = ref({
  fullName: '',
  publicName: '',
  jobTitle: '',
  department: '',
  organization: '',
  email: '',
  collaboration: '',
  avatarUrl: '',
  coverUrl: '',
  lastPasswordChangedAt: '',
  canChangePasswordAt: ''
})

const passwordStep = ref(1)
const passwordOtpSent = ref(false)
const isSendingPasswordOtp = ref(false)
const isChangingPassword = ref(false)
const isRequestingPasswordException = ref(false)
const canResendPasswordOtp = ref(true)
const passwordCountdown = ref(0)
const passwordStrength = ref(0)
const passwordStrengthClass = ref('')
const passwordStrengthLabel = ref('')
let passwordCountdownTimer = null

const passwordForm = reactive({
  otpCode: '',
  newPassword: '',
  confirmPassword: '',
  logoutOthers: true
})

const passwordHints = reactive({
  length: false,
  uppercase: false,
  number: false,
  special: false
})

const passwordCountdownText = computed(() => {
  const minutes = Math.floor(passwordCountdown.value / 60)
  const seconds = passwordCountdown.value % 60
  return `${minutes}:${seconds.toString().padStart(2, '0')}`
})

const canSubmitPassword = computed(() => (
  passwordStrength.value === 4 &&
  passwordForm.newPassword === passwordForm.confirmPassword &&
  passwordForm.otpCode.length >= 6
))

const isPasswordChangeLocked = computed(() => {
  if (!profileData.value.canChangePasswordAt) return false
  return new Date(profileData.value.canChangePasswordAt).getTime() > Date.now()
})

const passwordEligibleLabel = computed(() => {
  if (!profileData.value.canChangePasswordAt) return 'now'
  return new Date(profileData.value.canChangePasswordAt).toLocaleString()
})

const getBaseUrl = () => import.meta.env.VITE_API_BASE_URL?.replace('/api', '') || 'http://localhost:5136'

const avatarStyle = computed(() => {
  if (!profileData.value.avatarUrl) return {}
  return {
    backgroundImage: `url(${getBaseUrl()}${profileData.value.avatarUrl})`,
    backgroundSize: 'cover',
    backgroundPosition: 'center',
    color: 'transparent'
  }
})

const coverBannerStyle = computed(() => {
  if (!profileData.value.coverUrl) {
    return { background: 'var(--bg-tertiary)' }
  }
  return {
    backgroundImage: `url(${getBaseUrl()}${profileData.value.coverUrl})`,
    backgroundSize: 'cover',
    backgroundPosition: 'center'
  }
})

const getInitials = (name) => {
  if (!name) return '?'
  return name
    .split(' ')
    .filter(Boolean)
    .map((word) => word[0])
    .join('')
    .substring(0, 2)
    .toUpperCase()
}

const handleAvatarCommand = (command) => {
  if (command === 'upload') {
    triggerAvatarUpload()
  }
}

const triggerAvatarUpload = () => {
  avatarInput.value?.click()
}

const triggerCoverUpload = () => {
  coverInput.value?.click()
}

const uploadAvatar = async (event) => {
  const avatarFile = event.target.files?.[0]
  if (!avatarFile) return

  const formData = new FormData()
  formData.append('file', avatarFile)

  try {
    const res = await axiosClient.put('/users/avatar', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    profileData.value.avatarUrl = res.data?.data?.avatarUrl || ''
    ElMessage.success('Profile photo updated')
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Could not upload profile photo')
  } finally {
    event.target.value = ''
  }
}

const uploadCover = async (event) => {
  const coverFile = event.target.files?.[0]
  if (!coverFile) return

  const formData = new FormData()
  formData.append('file', coverFile)

  try {
    const res = await axiosClient.put('/users/cover', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })
    profileData.value.coverUrl = res.data?.data?.coverUrl || ''
    ElMessage.success('Cover updated')
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Could not upload cover')
  } finally {
    event.target.value = ''
  }
}

const fetchProfile = async () => {
  try {
    isLoading.value = true
    const response = await axiosClient.get('/users/me')
    const data = response.data?.data || {}
    profileData.value = {
      fullName: data.fullName || '',
      publicName: data.publicName || '',
      jobTitle: data.jobTitle || '',
      department: data.departmentName || '',
      organization: data.organizationName || '',
      email: data.email || '',
      collaboration: data.collaborationRules || '',
      avatarUrl: data.avatarUrl || '',
      coverUrl: data.coverUrl || '',
      lastPasswordChangedAt: data.lastPasswordChangedAt || '',
      canChangePasswordAt: data.canChangePasswordAt || ''
    }
  } catch (error) {
    console.error('Profile load failed', error)
    ElMessage.error('Could not load your profile.')
  } finally {
    isLoading.value = false
  }
}

const saveProfile = async () => {
  try {
    isSaving.value = true
    await axiosClient.put('/users/profile', {
      fullName: profileData.value.fullName,
      publicName: profileData.value.publicName,
      jobTitle: profileData.value.jobTitle,
      departmentName: profileData.value.department,
      organizationName: profileData.value.organization,
      collaborationRules: profileData.value.collaboration
    })
    ElMessage.success('Profile saved.')
    await fetchProfile()
  } catch (error) {
    console.error('Profile save failed', error)
    ElMessage.error(error.response?.data?.message || 'Could not save profile.')
  } finally {
    isSaving.value = false
  }
}

const startPasswordCountdown = () => {
  canResendPasswordOtp.value = false
  passwordCountdown.value = 60
  if (passwordCountdownTimer) clearInterval(passwordCountdownTimer)
  passwordCountdownTimer = setInterval(() => {
    passwordCountdown.value -= 1
    if (passwordCountdown.value <= 0) {
      clearInterval(passwordCountdownTimer)
      passwordCountdownTimer = null
      canResendPasswordOtp.value = true
    }
  }, 1000)
}

const sendPasswordOtp = async () => {
  if (isPasswordChangeLocked.value) {
    ElMessage.warning('Password changes are limited to once every 7 days. Please request admin support if this is urgent.')
    return
  }
  if (!profileData.value.email) {
    ElMessage.warning('Profile email is missing.')
    return
  }

  isSendingPasswordOtp.value = true
  try {
    const { data } = await axiosClient.post('/users/send-change-password-otp', {
      email: profileData.value.email
    })
    passwordOtpSent.value = true
    passwordForm.otpCode = ''
    startPasswordCountdown()
    ElMessage.success(data?.message || 'OTP code sent to your email.')
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Could not send OTP code.')
  } finally {
    isSendingPasswordOtp.value = false
  }
}

const requestPasswordChangeException = async () => {
  isRequestingPasswordException.value = true
  try {
    const { data } = await axiosClient.post('/users/request-password-change-exception')
    ElMessage.success(data?.message || 'Your request has been sent to the system admin.')
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Could not send request to system admin.')
  } finally {
    isRequestingPasswordException.value = false
  }
}

const checkPasswordStrength = () => {
  const password = passwordForm.newPassword
  passwordHints.length = password.length >= 8
  passwordHints.uppercase = /[A-Z]/.test(password)
  passwordHints.number = /[0-9]/.test(password)
  passwordHints.special = /[^A-Za-z0-9]/.test(password)

  let score = 0
  if (passwordHints.length) score += 1
  if (passwordHints.uppercase) score += 1
  if (passwordHints.number) score += 1
  if (passwordHints.special) score += 1
  passwordStrength.value = score

  if (score <= 1) {
    passwordStrengthClass.value = 'strength-weak'
    passwordStrengthLabel.value = 'Weak'
  } else if (score === 2) {
    passwordStrengthClass.value = 'strength-fair'
    passwordStrengthLabel.value = 'Fair'
  } else if (score === 3) {
    passwordStrengthClass.value = 'strength-good'
    passwordStrengthLabel.value = 'Good'
  } else {
    passwordStrengthClass.value = 'strength-strong'
    passwordStrengthLabel.value = 'Very strong'
  }
}

const changePassword = async () => {
  if (!canSubmitPassword.value) return

  isChangingPassword.value = true
  try {
    await axiosClient.put('/users/change-password', {
      otpCode: passwordForm.otpCode,
      newPassword: passwordForm.newPassword,
      logoutOthers: passwordForm.logoutOthers
    })
    passwordStep.value = 3
    await fetchProfile()
    ElMessage.success('Password changed successfully.')
  } catch (error) {
    ElMessage.error(error.response?.data?.message || 'Password change failed. Please check the OTP code.')
  } finally {
    isChangingPassword.value = false
  }
}

const resetPasswordFlow = () => {
  passwordStep.value = 1
  passwordOtpSent.value = false
  passwordForm.otpCode = ''
  passwordForm.newPassword = ''
  passwordForm.confirmPassword = ''
  passwordStrength.value = 0
  passwordStrengthClass.value = ''
  passwordStrengthLabel.value = ''
  passwordHints.length = false
  passwordHints.uppercase = false
  passwordHints.number = false
  passwordHints.special = false
}

onMounted(fetchProfile)

onUnmounted(() => {
  if (passwordCountdownTimer) clearInterval(passwordCountdownTimer)
})
</script>

<style scoped>
.profile-page {
  height: 100%;
}

.profile-body-container {
  height: 100%;
  overflow-y: auto;
  padding: 40px 0;
  display: flex;
  justify-content: center;
}

.profile-container {
  width: 100%;
  max-width: 800px;
  display: flex;
  flex-direction: column;
}

.profile-header-section {
  position: relative;
  background-color: var(--bg-secondary);
  border-radius: 2px;
  overflow: visible;
  border: 1px solid var(--border-color);
  margin-bottom: 80px;
  width: 100%;
}

.header-image-box {
  width: 100%;
  height: 180px;
  display: flex;
  align-items: center;
  position: relative;
  cursor: pointer;
  border-radius: 2px 2px 0 0;
  transition: filter 0.2s;
}

.header-image-box:hover {
  filter: brightness(0.88);
}

.avatar-inside-wrapper {
  position: absolute;
  left: 30px;
  bottom: -50px;
  z-index: 10;
}

.large-profile-avatar {
  height: 120px;
  width: 120px;
  background-color: var(--color-accent);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 40px;
  font-weight: 700;
  color: #ffffff;
  border: 4px solid var(--bg-primary);
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: filter 0.2s;
}

.large-profile-avatar:hover {
  filter: brightness(0.8);
}

.avatar-hover-overlay {
  position: absolute;
  inset: 0;
  background: rgba(0, 0, 0, 0);
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background 0.2s;
  color: white;
  font-size: 22px;
  opacity: 0;
}

.large-profile-avatar:hover .avatar-hover-overlay {
  background: rgba(0, 0, 0, 0.4);
  opacity: 1;
}

.banner-upload-prompt {
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: rgba(255, 255, 255, 0.7);
  font-weight: 600;
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.3);
  pointer-events: none;
}

.header-footer {
  background-color: var(--bg-secondary);
  border-radius: 0 0 2px 2px;
  padding: 24px 24px 12px;
  display: flex;
  justify-content: space-between;
  gap: 24px;
  align-items: flex-start;
}

.profile-name-block {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.profile-name-block strong {
  color: var(--color-text-primary);
  font-size: 22px;
  font-weight: 700;
}

.profile-name-block span {
  color: var(--color-text-secondary);
  font-size: 14px;
}

.profile-content-form {
  padding: 0 40px;
}

.section-title {
  font-size: 18px;
  font-weight: 800;
  color: var(--color-text-primary);
  margin-bottom: 24px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.mt-40 {
  margin-top: 40px;
}

.mt-24 {
  margin-top: 24px;
}

.mt-16 {
  margin-top: 16px;
}

.form-grid {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-row {
  display: grid;
  grid-template-columns: 160px 1fr;
  gap: 12px;
}

.field-label {
  font-size: 12px;
  font-weight: 800;
  color: var(--color-text-secondary);
  display: flex;
  align-items: center;
  gap: 6px;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.info-icon {
  font-size: 11px;
  color: var(--color-text-muted);
}

.field-input-wrapper {
  max-width: 500px;
}

.save-row {
  margin-top: 12px;
}

.contact-card {
  padding-bottom: 24px;
}

.email-value {
  padding: 6px 0;
  font-size: 14px;
  color: var(--color-text-primary);
  font-weight: 600;
}

.password-card {
  background-color: var(--color-surface);
  border: 1px solid var(--color-border);
  border-radius: 12px;
  padding: 28px;
  margin-bottom: 48px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.16);
}

.step-indicator {
  display: flex;
  align-items: center;
  margin-bottom: 28px;
}

.step-badge {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: var(--color-border);
  color: var(--color-text-muted);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 700;
  transition: all 0.3s ease;
  flex-shrink: 0;
}

.step-badge.active {
  background: linear-gradient(135deg, #3b82f6, #2563eb);
  color: white;
  box-shadow: 0 4px 14px rgba(59, 130, 246, 0.4);
}

.step-badge.done {
  background: linear-gradient(135deg, #10b981, #059669);
  color: white;
  box-shadow: 0 4px 14px rgba(16, 185, 129, 0.4);
}

.step-line {
  flex: 1;
  height: 2px;
  background: var(--color-border);
  margin: 0 12px;
  max-width: 80px;
}

.step-line.done {
  background: linear-gradient(90deg, #10b981, #3b82f6);
}

.step-title {
  font-size: 18px;
  font-weight: 600;
  color: var(--color-text-primary);
  display: flex;
  align-items: center;
  gap: 10px;
  margin: 0 0 6px;
}

.step-title i {
  color: #3b82f6;
}

.step-desc {
  font-size: 14px;
  color: var(--color-text-muted);
  margin: 0 0 24px;
  line-height: 1.5;
}

.compact-password-row {
  align-items: start;
}

.otp-hint {
  margin-top: 8px;
  display: flex;
  align-items: center;
  gap: 6px;
  flex-wrap: wrap;
  color: var(--color-text-muted);
  font-size: 12px;
}

.otp-hint i,
.otp-hint a {
  color: #3b82f6;
}

.password-actions {
  margin-top: 28px;
  display: flex;
  justify-content: flex-end;
  gap: 12px;
}

.password-actions.between {
  justify-content: space-between;
}

.verified-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 10px 16px;
  background: rgba(16, 185, 129, 0.08);
  border: 1px solid rgba(16, 185, 129, 0.2);
  border-radius: 8px;
  font-size: 13px;
  color: #10b981;
}

.verified-badge strong {
  color: var(--color-text-primary);
}

.password-strength {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 12px;
}

.strength-bars {
  display: flex;
  gap: 4px;
  flex: 1;
  max-width: 220px;
}

.strength-bars .bar {
  flex: 1;
  height: 6px;
  background-color: var(--color-border);
  border-radius: 3px;
  transition: background-color 0.3s ease;
}

.bar.strength-weak {
  background-color: #ef4444;
}

.bar.strength-fair {
  background-color: #f59e0b;
}

.bar.strength-good {
  background-color: #3b82f6;
}

.bar.strength-strong {
  background-color: #10b981;
}

.strength-text {
  font-size: 12px;
  font-weight: 600;
}

.strength-text.strength-weak {
  color: #ef4444;
}

.strength-text.strength-fair {
  color: #f59e0b;
}

.strength-text.strength-good {
  color: #3b82f6;
}

.strength-text.strength-strong {
  color: #10b981;
}

.password-hints {
  list-style: none;
  padding: 0;
  margin: 12px 0 0;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
}

.password-hints li {
  font-size: 13px;
  color: var(--color-text-muted);
  display: flex;
  align-items: center;
  gap: 6px;
}

.password-hints li.passed {
  color: #10b981;
}

.error-msg {
  margin-top: 6px;
  color: #ef4444;
  font-size: 13px;
}

.logout-option {
  margin-top: 20px;
}

.checkbox-title {
  font-weight: 600;
  color: var(--color-text-primary);
}

.success-section {
  text-align: center;
  padding: 36px 0 24px;
}

.success-icon-wrapper {
  width: 88px;
  height: 88px;
  border-radius: 50%;
  background: linear-gradient(135deg, rgba(16, 185, 129, 0.15), rgba(5, 150, 105, 0.08));
  border: 2px solid rgba(16, 185, 129, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 24px;
}

.success-icon-wrapper i {
  font-size: 40px;
  color: #10b981;
}

.success-title {
  font-size: 22px;
  font-weight: 700;
  color: var(--color-text-primary);
  margin: 0 0 12px;
}

.success-desc {
  font-size: 14px;
  color: var(--color-text-secondary);
  max-width: 400px;
  margin: 0 auto 24px;
  line-height: 1.7;
}

:deep(.el-input__wrapper) {
  background-color: rgba(255, 255, 255, 0.02);
}

@media (max-width: 768px) {
  .profile-header-section {
    margin-bottom: 60px;
  }

  .header-image-box {
    height: 120px;
  }

  .large-profile-avatar {
    height: 80px;
    width: 80px;
    font-size: 24px;
  }

  .avatar-inside-wrapper {
    left: 20px;
    bottom: -40px;
  }

  .profile-content-form {
    padding: 0 16px;
  }

  .form-row {
    grid-template-columns: 1fr;
    gap: 6px;
  }

  .field-input-wrapper {
    max-width: 100%;
  }

  .password-hints {
    grid-template-columns: 1fr;
  }

  .password-actions,
  .password-actions.between {
    justify-content: stretch;
    flex-direction: column;
  }
}
</style>
