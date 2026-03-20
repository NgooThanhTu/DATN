<script setup>
import { 
  CheckCircle, 
  Layout, 
  Users, 
  TrendingUp, 
  Play, 
  ArrowRight,
  Zap,
  Clock,
  Github,
  Twitter,
  Instagram,
  Facebook
} from 'lucide-vue-next'
import HelpDropdown from '../components/HelpDropdown.vue'

import dashboardPreview from '../assets/task_management_dashboard_preview_1773375713763.png'
import focusImage from '../assets/modern_desk_setup_product_design_1773375736492.png'
import logoImg from '../assets/logo_QLCV.png'

const features = [
  {
    title: 'Quản lý Công việc',
    desc: 'Theo dõi tiến độ dự án với các công cụ quản lý dễ sử dụng.',
    icon: CheckCircle,
    color: '#0061ff',
    bg: '#eff6ff'
  },
  {
    title: 'Bảng Kanban',
    desc: 'Trực quan hóa luồng công việc và tăng hiệu suất trong chớp mắt.',
    icon: Layout,
    color: '#a855f7',
    bg: '#f5f3ff'
  },
  {
    title: 'Cộng tác Nhóm',
    desc: 'Trò chuyện, bình luận và cộng tác theo thời gian thực với đội ngũ.',
    icon: Users,
    color: '#0ea5e9',
    bg: '#f0f9ff'
  },
  {
    title: 'Theo dõi Quá trình',
    desc: 'Nắm bắt chi tiết thông qua các báo cáo và phân tích chuyên sâu.',
    icon: TrendingUp,
    color: '#3b82f6',
    bg: '#eff6ff'
  }
]

const kanbanColumns = [
  {
    title: 'CẦN LÀM',
    count: 3,
    items: [
      { title: 'Cập nhật Hệ thống Thiết kế', tag: 'UI/UX', tagType: 'primary' },
      { title: 'Nghiên cứu Thị trường', tag: 'Analysis', tagType: 'info' }
    ]
  },
  {
    title: 'ĐANG LÀM',
    count: 2,
    items: [
      { title: 'Tích hợp API', tag: 'Backend', tagType: 'warning' }
    ]
  },
  {
    title: 'CHỜ DUYỆT',
    count: 1,
    items: [
      { title: 'Mẫu Prototype V2.0', tag: 'Design', tagType: 'success' }
    ]
  },
  {
    title: 'HOÀN THÀNH',
    count: 4,
    items: [
      { title: 'Đăng nhập Người dùng', tag: 'Core', tagType: 'danger' }
    ]
  }
]
</script>

<template>
  <div class="landing-page">
    <!-- Navbar -->
    <header class="navbar">
      <div class="container nav-content">
        <a href="/" class="logo">
          <img :src="logoImg" alt="SprintA Logo" class="custom-logo" />
          <span class="logo-text">SprintA</span>
        </a>
        <div class="nav-actions" style="margin-left: auto; display: flex; align-items: center; gap: 16px;">
          <HelpDropdown />
          <el-button link @click="$router.push('/login')">Đăng nhập</el-button>
          <el-button type="primary" round @click="$router.push('/register')">Đăng ký</el-button>
        </div>
      </div>
    </header>

    <!-- Hero Section -->
    <section class="hero-section text-center section-padding">
      <div class="container animate-fade-up">
        <div class="new-badge">
          <span>MỚI</span> Có gì mới trong SprintA v3.0?
        </div>
        <h1 class="hero-title">
          Tổ chức Công việc. Quản lý Dự án.<br />
          <span class="text-primary">Tiến độ Thần tốc.</span>
        </h1>
        <p class="hero-sub">
          Nền tảng quản lý công việc tất-cả-trong-một dành cho các đội ngũ muốn tạo ra<br />
          sản phẩm xuất sắc mà không bị rối loạn.
        </p>
        <div class="hero-btns" style="justify-content: flex-start;">
          <el-button type="primary" size="large" round class="btn-cta" @click="$router.push('/register')">Bắt đầu Miễn phí</el-button>
        </div>

        <div class="dashboard-preview">
          <img :src="dashboardPreview" alt="Dashboard Preview" />
        </div>
      </div>
    </section>

    <!-- Features Grid -->
    <section class="features-section section-padding">
      <div class="container text-center">
        <h2 class="section-title">Mọi thứ bạn cần đều ở đây</h2>
        <p class="section-sub">Tối ưu hóa quy trình làm việc bằng các công cụ cấp doanh nghiệp.</p>
        
        <el-row :gutter="32" class="feature-grid">
          <el-col :xs="24" :sm="12" :md="6" v-for="f in features" :key="f.title">
            <div class="feature-card">
              <div class="icon-wrap" :style="{ backgroundColor: f.bg }">
                <component :is="f.icon" :size="24" :color="f.color" />
              </div>
              <h3>{{ f.title }}</h3>
              <p>{{ f.desc }}</p>
            </div>
          </el-col>
        </el-row>
      </div>
    </section>

    <!-- Pipeline Section -->
    <section class="pipeline-section section-padding bg-light">
      <div class="container">
        <div class="pipeline-header">
          <h2>Dòng chảy Công việc Khép kín</h2>
          <div class="header-action">
            <a href="#" class="view-all">Xem tất cả biểu mẫu <ArrowRight :size="16" /></a>
          </div>
        </div>
        
        <el-row :gutter="24" class="kanban-wrapper">
          <el-col :md="6" v-for="col in kanbanColumns" :key="col.title">
            <div class="kanban-col">
              <div class="col-head">
                <span>{{ col.title }}</span>
                <span class="counter">{{ col.count }}</span>
              </div>
              <div class="tasks">
                <div v-for="task in col.items" :key="task.title" class="task-card">
                  <h4>{{ task.title }}</h4>
                  <el-tag :type="task.tagType" size="small">{{ task.tag }}</el-tag>
                </div>
              </div>
            </div>
          </el-col>
        </el-row>
      </div>
    </section>

    <!-- Focus Section -->
    <section class="focus-section section-padding">
      <div class="container">
        <el-row align="middle" :gutter="80">
          <el-col :md="12">
            <div class="focus-content">
              <h2 class="section-title text-left">Thiết kế để Tập trung</h2>
              <p class="section-sub text-left">Loại bỏ sự phiền nhiễu với các công cụ được xây dựng cho công việc chuyên sâu và năng suất cao.</p>
              
              <div class="focus-item">
                <div class="item-icon"><Zap :size="20" color="#0061ff" /></div>
                <div class="item-text">
                  <h4>Trang chủ Cá Nhân hóa</h4>
                  <p>Bảng điều khiển trung tâm cho công việc hàng ngày, thời hạn sắp tới và các thông báo.</p>
                </div>
              </div>
              <div class="focus-item">
                <div class="item-icon"><Clock :size="20" color="#0061ff" /></div>
                <div class="item-text">
                  <h4>Lọc theo độ Ưu tiên</h4>
                  <p>Hệ thống lọc thông minh giúp bạn tập trung tuyệt đối vào việc quan trọng nhất ngay lúc này.</p>
                </div>
              </div>
            </div>
          </el-col>
          <el-col :md="12">
            <div class="focus-image">
              <img :src="focusImage" alt="Focus Focus" />
            </div>
          </el-col>
        </el-row>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta-section container">
      <div class="cta-card">
        <h2>Bắt đầu Quản lý Công việc<br />Hiệu quả hơn</h2>
        <p>Gia nhập 10,000+ đội ngũ đã tối ưu hóa quy trình làm việc với SprintA. Không cần liên kết thẻ.</p>
        <el-button type="primary" size="large" class="cta-primary-btn" @click="$router.push('/register')">Tạo Tài Khoản Miễn Phí</el-button>
        <p class="small-note">Miễn phí cho nhóm dưới 5 thành viên</p>
      </div>
    </section>

    <!-- Footer -->
    <footer class="footer">
      <div class="container">
        <el-row :gutter="40">
          <el-col :md="8">
            <a href="/" class="logo">
              <img :src="logoImg" alt="SprintA Logo" class="custom-logo" />
              <span class="logo-text">SprintA</span>
            </a>
            <p class="footer-about">
              Trao quyền cho đội ngũ bàn giao sản phẩm bứt phá rào cản thông qua hệ thống quản lý chuẩn Agile và cộng tác liền mạch.
            </p>
            <div class="socials">
              <Twitter :size="20" />
              <Github :size="20" />
              <Instagram :size="20" />
              <Facebook :size="20" />
            </div>
          </el-col>
          <el-col :md="4">
            <h4>Sản Phẩm</h4>
            <ul>
              <li><a href="#">Tính năng</a></li>
              <li><a href="#">Tích hợp</a></li>
              <li><a href="#">Biểu mẫu</a></li>
              <li><a href="#">Cập nhật</a></li>
            </ul>
          </el-col>
          <el-col :md="4">
            <h4>Công Ty</h4>
            <ul>
              <li><a href="#">Về chúng tôi</a></li>
              <li><a href="#">Tuyển dụng</a></li>
              <li><a href="#">Blog</a></li>
              <li><a href="#">Báo chí</a></li>
            </ul>
          </el-col>
          <el-col :md="4">
            <h4>Hỗ trợ</h4>
            <ul>
              <li><a href="#">Trung tâm trợ giúp</a></li>
              <li><a href="#">Trạng thái hệ thống</a></li>
              <li><a href="#">Tài liệu API</a></li>
              <li><a href="#">Bảo mật</a></li>
            </ul>
          </el-col>
        </el-row>
        <div class="footer-bottom">
          <p>© 2024 SprintA Technology Inc. Đã đăng ký bản quyền.</p>
          <div class="bottom-links">
            <a href="#">Chính sách bảo mật</a>
            <a href="#">Điều khoản dịch vụ</a>
            <a href="#">Cookies</a>
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.landing-page {
  overflow-x: hidden;
}

/* Navbar */
.navbar {
  height: 100px;
  display: flex;
  align-items: center;
  position: sticky;
  top: 0;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  z-index: 1000;
  border-bottom: 1px solid #f0f0f0;
}

.navbar .container {
  max-width: 1440px;
  width: 100%;
}

.nav-content {
  display: flex;
  align-items: center;
  width: 100%;
}

.logo {
  display: flex;
  align-items: center;
  gap: 0;
  font-weight: 900;
  font-size: 32px;
  color: #1a1a1a;
  flex-shrink: 0;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif;
  letter-spacing: -1px;
  text-decoration: none;
  cursor: pointer;
}

.logo-text {
  margin-left: -90px;
}

.footer .logo .logo-text {
  color: white;
  /* (inherit -32px from above) */
}

.logo-icon {
  width: 1024px;
  height: 1024px;
  background: var(--el-color-primary);
  border-radius: 8px;
}

.custom-logo {
  height: 128px;
  width: auto;
  object-fit: contain;
}

.nav-links {
  display: flex;
  gap: 32px;
  margin-left: auto;
  margin-right: 48px;
}

.nav-links a {
  text-decoration: none;
  color: #666;
  font-weight: 500;
  font-size: 12px;
  transition: color 0.3s;
}

.nav-links a:hover {
  color: var(--el-color-primary);
}

/* Hero Section */
.hero-section {
  background: var(--bg-gradient);
  padding-bottom: 0;
}

.new-badge {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  padding: 6px 16px;
  background: #f0f7ff;
  border-radius: 100px;
  font-size: 12px;
  font-weight: 600;
  color: var(--el-color-primary);
  margin-bottom: 24px;
}

.new-badge span {
  background: var(--el-color-primary);
  color: white;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.hero-title {
  font-size: 14px;
  font-weight: 700;
  margin-bottom: 24px;
  color: #0d121f;
}

.hero-sub {
  font-size: 12px;
  color: #666;
  line-height: 1.6;
  margin-bottom: 40px;
}

.hero-btns {
  display: flex;
  justify-content: center;
  gap: 16px;
  margin-bottom: 60px;
}

.btn-cta {
  padding: 24px 40px;
  font-size: 12px;
  font-weight: 600;
}

.btn-secondary {
  padding: 24px 32px;
  font-size: 12px;
}

.dashboard-preview {
  margin-top: 40px;
  border-radius: 20px 20px 0 0;
  overflow: hidden;
  box-shadow: 0 -20px 60px rgba(0, 0, 0, 0.1);
  max-width: 1000px;
  margin-left: auto;
  margin-right: auto;
}

.dashboard-preview img {
  width: 100%;
  display: block;
}

/* Features */
.section-title {
  font-size: 14px;
  font-weight: 700;
  margin-bottom: 16px;
}

.section-sub {
  font-size: 12px;
  color: #666;
  margin-bottom: 60px;
}

.feature-grid {
  margin-top: 40px;
}

.feature-card {
  padding: 40px 24px;
  text-align: left;
  transition: transform 0.3s;
}

.feature-card:hover {
  transform: translateY(-5px);
}

.icon-wrap {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 24px;
}

.feature-card h3 {
  font-size: 14px;
  margin-bottom: 12px;
}

.feature-card p {
  font-size: 12px;
  color: #666;
  line-height: 1.6;
}

/* Pipeline */
.bg-light {
  background-color: #f8faff;
}

.pipeline-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 48px;
}

.view-all {
  display: flex;
  align-items: center;
  gap: 8px;
  color: var(--el-color-primary);
  text-decoration: none;
  font-weight: 600;
}

.kanban-col {
  height: 100%;
}

.col-head {
  display: flex;
  justify-content: space-between;
  padding: 12px 16px;
  background: white;
  border-radius: 12px;
  font-weight: 700;
  font-size: 14px;
  margin-bottom: 16px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}

.counter {
  color: #999;
}

.task-card {
  background: white;
  padding: 20px;
  border-radius: 12px;
  margin-bottom: 12px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}

.task-card h4 {
  font-size: 14px;
  margin-bottom: 12px;
}

/* Focus Section */
.text-left {
  text-align: left;
}

.focus-content {
  padding-right: 40px;
}

.focus-item {
  display: flex;
  gap: 20px;
  margin-top: 32px;
}

.item-icon {
  width: 48px;
  height: 48px;
  background: #f0f7ff;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.item-text h4 {
  font-size: 14px;
  margin-bottom: 8px;
}

.item-text p {
  font-size: 12px;
  color: #666;
  line-height: 1.6;
}

.focus-image img {
  width: 100%;
  border-radius: 32px;
  box-shadow: 0 24px 64px rgba(0,0,0,0.1);
}

/* CTA */
.cta-section {
  padding: 80px 24px;
}

.cta-card {
  background: var(--footer-bg);
  padding: 80px 40px;
  border-radius: 32px;
  text-align: center;
  color: white;
  position: relative;
  overflow: hidden;
}

.cta-card h2 {
  font-size: 14px;
  font-weight: 700;
  margin-bottom: 24px;
}

.cta-card p {
  font-size: 12px;
  opacity: 0.8;
  margin-bottom: 40px;
}

.cta-primary-btn {
  padding: 28px 48px;
  font-size: 12px;
  font-weight: 700;
}

.small-note {
  margin-top: 20px;
  font-size: 12px !important;
  opacity: 0.6 !important;
}

/* Footer */
.footer {
  background: var(--footer-bg);
  padding: 80px 0 40px;
  color: white;
}

.logo.white { color: white; }
.logo-icon.white { background: #fff; }

.footer-about {
  margin: 24px 0;
  opacity: 0.6;
  line-height: 1.6;
}

.socials {
  display: flex;
  gap: 20px;
}

.socials svg {
  opacity: 0.6;
  cursor: pointer;
  transition: opacity 0.3s;
}

.socials svg:hover { opacity: 1; }

.footer h4 {
  font-size: 14px;
  margin-bottom: 24px;
}

.footer ul {
  list-style: none;
}

.footer ul li {
  font-size: 12px;
  margin-bottom: 12px;
}

.footer ul li a {
  color: white;
  text-decoration: none;
  opacity: 0.6;
  transition: opacity 0.3s;
}

.footer ul li a:hover { opacity: 1; }

.footer-bottom {
  margin-top: 80px;
  padding-top: 32px;
  border-top: 1px solid rgba(255,255,255,0.1);
  display: flex;
  justify-content: space-between;
  opacity: 0.4;
  font-size: 12px;
}

.bottom-links {
  display: flex;
  gap: 24px;
}

.bottom-links a {
  color: white;
  text-decoration: none;
}
</style>
