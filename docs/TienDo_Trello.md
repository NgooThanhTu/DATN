# 📋 TIẾN ĐỘ THỰC HIỆN CÔNG VIỆC TRÊN TRELLO

**Dự án:** Hệ thống Quản lý Công việc (SprintA)  
**Ngày lập:** 19/04/2026  
**Link Trello:** [Đường link board Trello]

> **Ghi chú:** File này chứa hướng dẫn cách setup Trello và template screenshots để đưa vào file Word nộp bài.

---

## I. HƯỚNG DẪN TẠO TRELLO BOARD

### 1.1 Tạo Board

1. Truy cập https://trello.com
2. Click "Create new board"
3. Tên board: **"SprintA - Quản Lý Công Việc"**
4. Chọn background: Dark theme

### 1.2 Tạo Lists

```
Tạo 6 lists theo thứ tự:

1. 📝 Product Backlog    → Tất cả US chưa lên plan
2. 📋 Sprint Backlog     → US được chọn cho sprint hiện tại
3. 🔄 In Progress        → US đang thực hiện
4. 🧪 Testing            → US đang được test
5. ✅ Done               → US hoàn thành
6. 🚀 Released           → US đã deploy/nộp
```

### 1.3 Tạo Labels

```
Labels cần tạo:
🔴 Critical   - Đỏ
🟠 High       - Cam
🟡 Medium     - Vàng
🟢 Low        - Xanh lá
🔵 Bug        - Xanh dương
🟣 Feature    - Tím
⚪ Docs       - Xám
```

### 1.4 Tạo Cards cho User Stories

Mỗi User Story → 1 card Trello:

```
Card title: US-001: Đăng ký tài khoản
Description:
  Là người dùng, tôi muốn đăng ký tài khoản với email/mật khẩu
  để tham gia hệ thống.
  
  Story Points: 5
  Priority: Must Have
  Sprint: Sprint 1

Checklist (Acceptance Criteria):
  ☐ Form register hiển thị đúng
  ☐ Validation email/password
  ☐ API register hoạt động
  ☐ Redirect sau register
  ☐ Test case TC-AUTH-06 PASS

Members: [Gán assignee]
Labels: 🔴 Critical, 🟣 Feature
Due date: [Ngày kết thúc sprint]
```

---

## II. THIẾT LẬP BURNDOWN CHART

### 2.1 Sử dụng burndownfortrello.com

1. Truy cập https://www.burndownfortrello.com/
2. Đăng nhập bằng tài khoản Trello
3. Chọn board "SprintA - Quản Lý Công Việc"
4. Cấu hình:
   - Sprint duration: 2 weeks (10 working days)
   - Story points: Dùng number trong title hoặc custom field
5. Hệ thống tự sinh Burndown Chart
6. Screenshot chart để đưa vào báo cáo

### 2.2 Mẫu Burndown Data

**Sprint 1 (58 SP):**

| Day | Planned | Actual |
|-----|---------|--------|
| 1 | 58 | 58 |
| 2 | 52 | 55 |
| 3 | 46 | 48 |
| 4 | 41 | 42 |
| 5 | 35 | 35 |
| 6 | 29 | 30 |
| 7 | 23 | 25 |
| 8 | 17 | 18 |
| 9 | 12 | 10 |
| 10 | 0 | 3 |

**Sprint 2 (100 SP):**

| Day | Planned | Actual |
|-----|---------|--------|
| 1 | 100 | 100 |
| 2 | 90 | 92 |
| 3 | 80 | 85 |
| 4 | 70 | 75 |
| 5 | 60 | 70 |  ← Bug phát hiện (Priority)
| 6 | 50 | 60 |  ← Bug fixing
| 7 | 40 | 45 |
| 8 | 30 | 30 |
| 9 | 20 | 15 |
| 10 | 0 | 5 |

---

## III. SCREENSHOT TEMPLATE CHO FILE WORD

> Cần chụp screenshot và đưa vào file Word theo thứ tự sau:

### 3.1 Screenshots cần chụp

| # | Screenshot | Mô tả | Khi nào chụp |
|---|-----------|-------|-------------|
| 1 | Trello Board Overview | Toàn bộ board với 6 lists | Mỗi đầu sprint |
| 2 | Sprint Backlog cards | Cards trong Sprint Backlog list | Đầu mỗi sprint |
| 3 | In Progress | Cards đang làm | Giữa sprint |
| 4 | Done list | Cards đã hoàn thành | Cuối sprint |
| 5 | Card detail | Chi tiết 1 card (checklist, activity) | Bất kì |
| 6 | Labels overview | Board với labels hiển thị | Bất kì |
| 7 | Member assignment | Cards với member avatars | Bất kì |
| 8 | Burndown Chart | Từ burndownfortrello.com | Cuối sprint |
| 9 | Activity Log | Trello activity bên phải | Cuối sprint |
| 10 | Calendar Power-Up | Calendar view nếu có | Bất kì |

### 3.2 Template File Word

```
FILE WORD: TienDoThucHien_Trello.docx

Trang 1: Bìa
  • Tên dự án, nhóm, ngày tháng

Trang 2-3: Sprint 1
  • Screenshot 1: Board Overview Sprint 1
  • Screenshot 2: Sprint 1 Backlog cards
  • Screenshot 3: Cards In Progress
  • Screenshot 4: Sprint 1 Done
  • Screenshot 5: Burndown Chart Sprint 1
  • Nhận xét tiến độ Sprint 1

Trang 4-5: Sprint 2
  • Screenshot 6: Board Overview Sprint 2
  • Screenshot 7: Sprint 2 Backlog cards
  • Screenshot 8: Bug card (Priority fix)
  • Screenshot 9: Sprint 2 Done
  • Screenshot 10: Burndown Chart Sprint 2
  • Nhận xét tiến độ Sprint 2

Trang 6-7: Sprint 3
  • Tương tự Sprint 1-2

Trang 8-9: Sprint 4
  • Tương tự
  • Screenshot cuối: Board final state

Trang 10: Tổng kết
  • Tổng số cards: ___
  • Completed: ___
  • Pass rate: ___%
  • Nhận xét chung
```

---

## IV. CẤU TRÚC TRELLO CHI TIẾT

### Sprint 1 Cards

| Card | List | Labels | Members | Due Date |
|------|------|--------|---------|----------|
| US-001: Đăng ký tài khoản | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-002: Đăng nhập | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-004: Quản lý hồ sơ | Sprint Backlog → Done | 🟡 Medium, 🟣 Feature | | |
| US-005: Token refresh | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-006: Tạo Workspace | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-007: Mời thành viên | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-008: Role management | Sprint Backlog → Done | 🟡 Medium, 🟣 Feature | | |
| US-009: Tạo Project | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-010: Xem projects | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-011: Sửa project | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-012: Xóa project | Sprint Backlog → Done | 🟡 Medium, 🟣 Feature | | |
| US-013: Members project | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-065: Sidebar | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-066: Dark mode | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |

### Sprint 2 Cards

| Card | List | Labels | Members | Due Date |
|------|------|--------|---------|----------|
| US-014: Tạo task | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-015: Xem chi tiết task | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-016: Edit title inline | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-017: Change Status | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-018: Change Priority | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| BUG-001: Priority cần F5 | Testing → Done | 🔵 Bug, 🔴 Critical | | |
| US-019: Assignees | Sprint Backlog → Done | 🔴 Critical, 🟣 Feature | | |
| US-020: Labels | Sprint Backlog → Done | 🟡 Medium | | |
| US-021: Dates | Sprint Backlog → Done | 🔴 Critical | | |
| US-022: Rich editor | Sprint Backlog → Done | 🟡 Medium | | |
| US-023: Subtasks | Sprint Backlog → Done | 🟡 Medium | | |
| US-028: Kanban Board | Sprint Backlog → Done | 🔴 Critical | | |
| US-029: Drag & Drop | Sprint Backlog → Done | 🔴 Critical | | |
| US-031: List View | Sprint Backlog → Done | 🔴 Critical | | |
| US-037: Tạo Cycle | Sprint Backlog → Done | 🔴 Critical | | |
| US-043: Comments | Sprint Backlog → Done | 🔴 Critical | | |
| US-046: Labels CRUD | Sprint Backlog → Done | 🟡 Medium | | |

---

## V. THEO DÕI

### Velocity Chart

| Sprint | Planned SP | Completed SP | Velocity |
|--------|-----------|-------------|----------|
| Sprint 1 | 58 | | |
| Sprint 2 | 100 | | |
| Sprint 3 | 110 | | |
| Sprint 4 | 47 | | |
| **Tổng** | **315** | | |

### Bug Tracking

| Bug ID | Title | Sprint | Severity | Status |
|--------|-------|--------|----------|--------|
| BUG-001 | Priority cần F5 refresh | Sprint 2 | 🔴 Critical | ✅ Fixed |
| | | | | |
| | | | | |

---

**Người lập:** ___________  
**Ngày tạo:** 19/04/2026
