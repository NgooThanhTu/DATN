<template>
  <div class="auth-page">
    <header class="auth-navbar">
      <div class="container nav-content">
        <router-link to="/" class="logo">
          <img :src="logoImg" alt="SprintA Logo" class="custom-logo" />
          <span class="logo-text">SprintA</span>
        </router-link>
        <div class="nav-actions" style="display: flex; align-items: center; gap: 16px;">
          <el-button link @click="$router.push('/login')">Đăng nhập</el-button>
          <el-button type="primary" round @click="$router.push('/register')">Đăng ký</el-button>
        </div>
      </div>
    </header>

    <div class="auth-container">
      <div class="auth-card">
        <h1 class="auth-title">Tạo Tài Khoản</h1>
        <p class="auth-subtitle">Bắt đầu quản lý dự án và công việc của bạn với SprintA.</p>
        
        <el-button plain class="social-btn full-width">
          <img :src="googleIcon" alt="Google" class="social-icon" /> Tiếp tục với Google
        </el-button>
        
        <div class="divider">
          <span>hoặc</span>
        </div>
        
        <el-form class="auth-form" @submit.prevent="handleRegister" label-position="top">
          <el-form-item label="Họ và Tên">
            <el-input v-model="form.name" placeholder="John Doe" size="large" />
          </el-form-item>
          
          <el-form-item label="Địa chỉ Email">
            <el-input v-model="form.email" placeholder="name@company.com" size="large" />
          </el-form-item>
          
          <el-form-item label="Mật khẩu">
            <el-input v-model="form.password" type="password" placeholder="••••••••" size="large" show-password />
          </el-form-item>
          
          <el-form-item label="Xác nhận Mật khẩu">
            <el-input v-model="form.confirmPassword" type="password" placeholder="••••••••" size="large" show-password />
          </el-form-item>
          
          <el-button type="primary" native-type="submit" class="auth-btn" size="large">Tạo Tài Khoản</el-button>
          
          <p class="auth-footer-text">
            Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link>
          </p>
        </el-form>
      </div>
      
      <div class="auth-small-links">
        <a href="#">Điều khoản Dịch vụ</a> &nbsp;•&nbsp; <a href="#">Chính sách Bảo mật</a>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import logoImg from '../assets/logo_QLCV.png'
import googleIcon from '../assets/Icongoogle.png'

const form = reactive({
  name: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const handleRegister = () => {
  console.log('Register attempt:', form)
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background-color: #f8fafc;
}

.auth-navbar {
  height: 80px;
  display: flex;
  align-items: center;
  background: transparent;
}

.container {
  max-width: 1440px;
  width: 100%;
  margin: 0 auto;
  padding: 0 24px;
}

.nav-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

/* Logo Styles */
.logo {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 800;
  font-size: 24px;
  color: #1a1a1a;
  text-decoration: none;
}

.custom-logo {
  height: 48px;
  width: auto;
  object-fit: contain;
}

.nav-actions {
  display: flex;
  gap: 16px;
}

.auth-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.auth-card {
  background: white;
  width: 100%;
  max-width: 440px;
  padding: 48px 40px 40px;
  border-radius: 20px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.04), 0 1px 3px rgba(0,0,0,0.02);
}

.auth-title {
  font-size: 24px;
  font-weight: 700;
  color: #0f172a;
  text-align: center;
  margin-bottom: 8px;
}

.auth-subtitle {
  color: #64748b;
  text-align: center;
  font-size: 13px;
  margin-bottom: 32px;
  line-height: 1.5;
}

.auth-form {
  width: 100%;
}

:deep(.el-form-item__label) {
  font-weight: 600;
  color: #334155;
  padding-bottom: 4px;
  font-size: 13px;
}

.auth-btn {
  width: 100%;
  font-weight: 600;
  border-radius: 12px;
  height: 48px;
  font-size: 15px;
  background: linear-gradient(135deg, #0ea5e9, var(--el-color-primary));
  border: none;
  margin-top: 8px;
  margin-bottom: 24px;
  transition: transform 0.2s, box-shadow 0.2s;
}

.auth-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 97, 255, 0.3);
}

@media (max-width: 640px) {
  .logo-text {
    display: none;
  }
  .auth-navbar {
    height: 60px;
  }
  .auth-card {
    padding: 32px 20px;
    border-radius: 12px;
  }
  .auth-title {
    font-size: 20px;
  }
}

.social-btn.full-width {
  width: 100%;
  height: 44px;
  border-radius: 10px;
  font-weight: 500;
  color: #334155;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
}

.social-icon {
  width: 20px;
  height: 20px;
  object-fit: contain;
}

.divider {
  display: flex;
  align-items: center;
  text-align: center;
  margin: 24px 0;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  border-bottom: 1px solid #e2e8f0;
}

.divider span {
  padding: 0 16px;
  color: #94a3b8;
  font-size: 12px;
  font-style: italic;
}

.auth-footer-text {
  text-align: center;
  font-size: 14px;
  color: #64748b;
  margin: 0;
}

.auth-footer-text a {
  color: var(--el-color-primary);
  text-decoration: none;
  font-weight: 600;
}

.auth-small-links {
  margin-top: 24px;
  font-size: 12px;
  color: #94a3b8;
}

.auth-small-links a {
  color: #94a3b8;
  text-decoration: none;
  transition: color 0.2s;
}

.auth-small-links a:hover {
  color: #64748b;
}
</style>
