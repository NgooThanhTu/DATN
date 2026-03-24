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
        <h1 class="auth-title">Chào mừng trở lại</h1>
        <p class="auth-subtitle">Đăng nhập để tiếp tục quản lý công việc của bạn với SprintA.</p>
        
        <el-form class="auth-form" @submit.prevent="handleLogin" label-position="top">
          <el-form-item label="Email">
            <el-input v-model="form.email" placeholder="name@email.com" size="large" />
          </el-form-item>
          
          <el-form-item class="password-item">
            <template #label>
              <div class="password-label">
                <span>Mật khẩu</span>
                <a href="#" class="forgot-pwd">Quên mật khẩu?</a>
              </div>
            </template>
            <el-input 
              v-model="form.password" 
              type="password" 
              placeholder="••••••••" 
              size="large" 
              show-password 
            />
          </el-form-item>
          
          <div class="remember-action">
            <el-checkbox v-model="form.remember">Ghi nhớ đăng nhập</el-checkbox>
          </div>
          
          <el-button type="primary" native-type="submit" class="auth-btn" size="large">Đăng nhập</el-button>
          
          <div class="divider">
            <span>HOẶC TIẾP TỤC VỚI</span>
          </div>
          
          <div class="social-login">
            <el-button plain class="social-btn">
              <img :src="googleIcon" alt="Google" class="social-icon" /> Google
            </el-button>
            <el-button plain class="social-btn">
              <img :src="githubIcon" alt="GitHub" class="social-icon" /> GitHub
            </el-button>
          </div>
          
          <p class="auth-footer-text">
            Chưa có tài khoản? <router-link to="/register">Đăng ký</router-link>
          </p>
        </el-form>
      </div>
    </div>
    
    <div class="auth-bottom">
      <p>© 2024 SprintA Inc. Đã đăng ký bản quyền.</p>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import logoImg from '../assets/logo_QLCV.png'
import googleIcon from '../assets/Icongoogle.png'
import githubIcon from '../assets/Icongithub.png'

import { useRouter } from 'vue-router'

const form = reactive({
  email: '',
  password: '',
  remember: false
})

const router = useRouter()

const handleLogin = () => {
  console.log('Login attempt:', form)
  router.push('/dashboard')
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
  align-items: center;
  justify-content: center;
  padding: 40px 20px;
}

.auth-card {
  background: white;
  width: 100%;
  max-width: 440px;
  padding: 48px 40px;
  border-radius: 20px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.04), 0 1px 3px rgba(0,0,0,0.02);
}

.auth-title {
  font-size: 28px;
  font-weight: 700;
  color: #0f172a;
  text-align: center;
  margin-bottom: 12px;
}

.auth-subtitle {
  color: #64748b;
  text-align: center;
  font-size: 14px;
  margin-bottom: 40px;
}

.auth-form {
  width: 100%;
}

:deep(.el-form-item__label) {
  font-weight: 600;
  color: #334155;
  padding-bottom: 4px;
}

.password-label {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}

:deep(.password-item .el-form-item__label) {
  width: 100%;
}

.forgot-pwd {
  color: var(--el-color-primary);
  text-decoration: none;
  font-weight: 500;
  font-size: 13px;
}

.remember-action {
  margin-bottom: 24px;
  margin-top: -10px;
}

/* Beautified button */
.auth-btn {
  width: 100%;
  font-weight: 600;
  border-radius: 12px;
  height: 48px;
  font-size: 15px;
  background: linear-gradient(135deg, #0f4c81, var(--el-color-primary));
  border: none;
  transition: transform 0.2s, box-shadow 0.2s;
}

.auth-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(0, 97, 255, 0.3);
}

.divider {
  display: flex;
  align-items: center;
  text-align: center;
  margin: 32px 0;
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
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.social-login {
  display: flex;
  gap: 16px;
  margin-bottom: 32px;
}

@media (max-width: 640px) {
  .logo-text {
    display: none;
  }
  .auth-card {
    padding: 32px 24px;
    border-radius: 12px;
  }
  .auth-title {
    font-size: 24px;
  }
  .social-login {
    flex-direction: column;
  }
}

.social-btn {
  flex: 1;
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

.auth-footer-text {
  text-align: center;
  font-size: 14px;
  color: #64748b;
}

.auth-footer-text a {
  color: var(--el-color-primary);
  text-decoration: none;
  font-weight: 600;
}

.auth-bottom {
  text-align: center;
  padding: 24px;
  color: #94a3b8;
  font-size: 12px;
}
</style>
