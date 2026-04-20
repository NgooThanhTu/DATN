# 📊 Y5: SLIDE TRÌNH BÀY - QUÁ TRÌNH ÁP DỤNG SCRUM TRONG DỰ ÁN QUANLYCONGVIEC

**Dự án:** Hệ thống Quản lý Công việc (SprintA)  
**Ngày lập:** 19/04/2026  
**Nhóm:** Nhóm dự án SprintA  

> **Ghi chú:** Nội dung dưới đây là kịch bản và nội dung từng slide. Nhóm sử dụng nội dung này để tạo file PowerPoint (.pptx) với thiết kế chuyên nghiệp.

---

## 📑 DANH SÁCH SLIDE

---

### SLIDE 1: TRANG BÌA

```
┌──────────────────────────────────────────────┐
│                                              │
│        🚀 QUẢN LÝ DỰ ÁN PHẦN MỀM           │
│       BẰNG PHƯƠNG PHÁP SCRUM                 │
│                                              │
│   Dự án: Hệ thống Quản lý Công việc         │
│   (SprintA - Task Management System)         │
│                                              │
│   Nhóm: [Tên nhóm]                          │
│   GVHD: [Tên giảng viên]                    │
│   Lớp:  [Mã lớp]                            │
│   Ngày: 19/04/2026                           │
│                                              │
│   Thành viên:                                │
│   1. [Họ tên 1] - [MSSV]                    │
│   2. [Họ tên 2] - [MSSV]                    │
│   3. [Họ tên 3] - [MSSV]                    │
│   4. [Họ tên 4] - [MSSV]                    │
│   5. [Họ tên 5] - [MSSV]                    │
│                                              │
└──────────────────────────────────────────────┘
```

---

### SLIDE 2: NỘI DUNG TRÌNH BÀY

```
📋 NỘI DUNG TRÌNH BÀY

1. Giới thiệu dự án
2. Tổng quan về Scrum
3. Vai trò trong nhóm Scrum
4. Product Backlog & User Stories
5. Sprint Planning & Sprint Backlog
6. Quy trình Sprint
7. Daily Standup & Communication
8. Sprint Review & Retrospective
9. Burndown Chart
10. Kết quả sản phẩm
11. Khó khăn gặp phải
12. Thuận lợi
13. Giải pháp & Đề xuất cải tiến
14. Demo sản phẩm
15. Q&A
```

---

### SLIDE 3: GIỚI THIỆU DỰ ÁN

```
🎯 GIỚI THIỆU DỰ ÁN

Tên dự án: SprintA - Hệ thống Quản trị Dự án & Công việc

Mục tiêu:
• Xây dựng hệ thống quản lý dự án theo phương pháp Agile/Scrum
• Hỗ trợ quản lý task, sprint, module, thành viên
• Giao diện hiện đại, dark theme, responsive
• Tích hợp AI để chia nhỏ công việc tự động
• Hệ thống Gamification khuyến khích làm việc

Công nghệ:
┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│  Backend    │  │  Frontend   │  │  Database   │
│  .NET 10    │  │  Vue 3      │  │  SQL Server │
│  C# Web API │  │  Element+   │  │  EF Core    │
│  SignalR    │  │  TailwindCSS│  │  Code First │
└─────────────┘  └─────────────┘  └─────────────┘

Quy mô:
• 24 API Controllers | 47 Domain Entities
• 24 Vue Components  | 27 Page Views
• 100+ API Endpoints | 85 Test Cases
```

---

### SLIDE 4: TỔNG QUAN VỀ SCRUM

```
🔄 TỔNG QUAN VỀ SCRUM

Scrum là gì?
• Framework quản lý dự án theo phương pháp Agile
• Phát triển sản phẩm theo các vòng lặp ngắn (Sprint)
• Tập trung vào feedback liên tục và cải tiến

3 Trụ cột (Pillars):
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  TRANSPARENCY│  │  INSPECTION  │  │  ADAPTATION  │
│  Minh bạch   │  │  Kiểm tra    │  │  Thích ứng   │
│              │  │              │  │              │
│  • Trello    │  │  • Daily     │  │  • Sprint    │
│  • GitHub    │  │    Standup   │  │    Retro     │
│  • Docs      │  │  • Sprint   │  │  • Backlog   │
│              │  │    Review   │  │    Grooming  │
└──────────────┘  └──────────────┘  └──────────────┘

5 Giá trị (Values):
Commitment | Focus | Openness | Respect | Courage
```

---

### SLIDE 5: VAI TRÒ TRONG NHÓM SCRUM

```
👥 VAI TRÒ TRONG NHÓM SCRUM

┌─────────────────────────────────────────┐
│           PRODUCT OWNER                  │
│  [Tên thành viên]                       │
│  • Quản lý Product Backlog             │
│  • Ưu tiên User Stories                │
│  • Đại diện khách hàng/stakeholder     │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│           SCRUM MASTER                   │
│  [Tên thành viên]                       │
│  • Đảm bảo nhóm tuân thủ Scrum         │
│  • Loại bỏ trở ngại (impediments)      │
│  • Tổ chức Sprint ceremonies           │
└─────────────────────────────────────────┘

┌─────────────────────────────────────────┐
│         DEVELOPMENT TEAM                 │
│  [Tên thành viên 3] - Backend Dev       │
│  [Tên thành viên 4] - Frontend Dev      │
│  [Tên thành viên 5] - QA/Tester         │
│  • Tự tổ chức, cross-functional         │
│  • Deliver increment mỗi sprint        │
└─────────────────────────────────────────┘
```

---

### SLIDE 6: PRODUCT BACKLOG & USER STORIES

```
📚 PRODUCT BACKLOG

Tổng: 67 User Stories | ~300 Story Points

Phân loại theo MoSCoW:
┌──────────────┬───────┬──────────┐
│ Must Have    │  22   │  33%     │  ████████
│ Should Have  │  25   │  37%     │  ██████████
│ Could Have   │  20   │  30%     │  ███████
└──────────────┴───────┴──────────┘

Top User Stories (VD):
┌────┬────────────────────────────────────────────┬────────┐
│ ID │ User Story                                 │ Points │
├────┼────────────────────────────────────────────┼────────┤
│ 01 │ Đăng ký tài khoản                          │   5    │
│ 02 │ Đăng nhập email/password                   │   3    │
│ 14 │ Tạo công việc mới (CRUD)                   │   5    │
│ 28 │ Xem Kanban Board                           │   8    │
│ 29 │ Drag & Drop task giữa cột                  │   8    │
│ 37 │ Tạo Sprint/Cycle                           │   5    │
│ 43 │ Bình luận trên task                        │   5    │
│ 60 │ AI chia nhỏ task tự động                   │   8    │
└────┴────────────────────────────────────────────┴────────┘
```

---

### SLIDE 7: RELEASE PLAN

```
📅 RELEASE PLAN

Release 1.0 (MVP) ──────────────────────────────
│                                                │
│  Sprint 1  │  Sprint 2                         │
│  Auth+WS   │  Task+Board+Cycle+Comment         │
│  ~58 SP    │  ~100 SP                          │
│                                                │
└── Tổng ~158 SP ────────────────────────────────┘

Release 2.0 (Full) ─────────────────────────────
│                                                │
│  Sprint 3         │  Sprint 4                  │
│  Views+Analytics+ │  Admin+AI+Gamification+    │
│  Modules+Notify   │  Testing+Polish            │
│  ~110 SP          │  ~47 SP                    │
│                                                │
└── Tổng ~157 SP ────────────────────────────────┘

Tổng dự án: ~315 Story Points | 4 Sprints | ~7 tuần
```

---

### SLIDE 8: SPRINT PLANNING

```
📋 SPRINT PLANNING

Sprint 1 (2 tuần):
• Goal: Auth + Workspace + Project CRUD
• Tasks: 24 tasks | ~58 Story Points
• Deliverable: Đăng nhập, tạo workspace/project

Sprint 2 (2 tuần):
• Goal: Task CRUD + Kanban + List + Cycles + Comments
• Tasks: 26 tasks | ~100 Story Points
• Deliverable: Quản lý task đầy đủ, Kanban board

Sprint 3 (2 tuần):
• Goal: Views + Analytics + Modules + Notifications
• Tasks: 19 tasks | ~110 Story Points
• Deliverable: Calendar, Timeline, Analytics

Sprint 4 (1 tuần):
• Goal: Admin + AI + Testing + Polish
• Tasks: 16 tasks | ~47 Story Points
• Deliverable: Hệ thống hoàn chỉnh, tested
```

---

### SLIDE 9: QUY TRÌNH SPRINT

```
🔄 QUY TRÌNH SPRINT (VD: Sprint 2)

      Sprint Planning        Daily Standup
      (Lên kế hoạch)        (Họp hàng ngày)
           │                      │
           ▼                      ▼
    ┌─────────────────────────────────────┐
    │                                     │
    │     S P R I N T  (2 tuần)           │
    │                                     │
    │  Week 1:                            │
    │  • Task CRUD API + UI               │
    │  • Status/Priority dropdowns        │
    │  • Assignee management              │
    │                                     │
    │  Week 2:                            │
    │  • Kanban Board + Drag & Drop       │
    │  • List View + Sort                 │
    │  • Cycles + Comments                │
    │  • Bug fix: Priority no F5 ⭐       │
    │                                     │
    └─────────────────────────────────────┘
           │                      │
           ▼                      ▼
      Sprint Review         Sprint Retro
      (Demo sản phẩm)      (Đánh giá & cải tiến)
```

---

### SLIDE 10: DAILY STANDUP

```
☀️ DAILY STANDUP

Thời gian: 15 phút mỗi ngày
Công cụ: Trello + Chat nhóm

3 câu hỏi mỗi ngày:
┌─────────────────────────────────────────┐
│ 1. Hôm qua làm được gì?               │
│ 2. Hôm nay sẽ làm gì?                 │
│ 3. Có gặp trở ngại gì?                │
└─────────────────────────────────────────┘

Ví dụ daily standup Sprint 2:

[Backend Dev]:
✅ Hôm qua: Hoàn thành WorkTasks API CRUD
📋 Hôm nay: Làm Comments API + reaction emoji
🚫 Trở ngại: Không có

[Frontend Dev]:
✅ Hôm qua: TaskDetailModal - 15+ properties done
📋 Hôm nay: Fix Priority dropdown không update ngay
🚫 Trở ngại: Priority selection cần F5 → cần debug

[QA]:
✅ Hôm qua: Viết 30 test cases cho Task module
📋 Hôm nay: Test Board View + Drag & Drop
🚫 Trở ngại: Không có
```

---

### SLIDE 11: SPRINT REVIEW & RETROSPECTIVE

```
🔍 SPRINT REVIEW (Demo)

Sprint 2 Review:
• Demo cho PO/stakeholder xem Kanban Board
• Demo drag & drop task giữa các cột
• Demo chi tiết task: 15+ properties
• ✅ Priority fix: cập nhật ngay không cần F5
• Feedback: Cần thêm Calendar view → Sprint 3

🔄 SPRINT RETROSPECTIVE

┌──────────────────────────────────────────────┐
│ 😊 WHAT WENT WELL (Tốt)                     │
│ • Kanban board hoạt động mượt mà            │
│ • Team communication tốt qua Trello          │
│ • Bug priority fix nhanh (1 ngày)            │
│ • Code review kỹ, ít merge conflict         │
├──────────────────────────────────────────────┤
│ 😟 WHAT DIDN'T GO WELL (Chưa tốt)           │
│ • Task estimate chưa chính xác              │
│ • Sprint 2 overloaded (100 SP quá nhiều)    │
│ • Frontend components quá lớn (100KB+)      │
│ • Thiếu unit test backend                   │
├──────────────────────────────────────────────┤
│ 💡 IMPROVEMENTS (Cải tiến)                   │
│ • Chia nhỏ component > 50KB                 │
│ • Giảm SP mỗi sprint xuống 60-80            │
│ • Thêm CI/CD pipeline                       │
│ • Tăng code review frequency                │
└──────────────────────────────────────────────┘
```

---

### SLIDE 12: BURNDOWN CHART

```
📉 BURNDOWN CHART - Sprint 2

Story Points
100 ┤ ●
 90 ┤  ╲ ●
 80 ┤   ╲  ●
 70 ┤    ╲   ●----●     ← Trở ngại (Priority bug)
 60 ┤     ╲       ●
 50 ┤      ╲       ╲●
 40 ┤       ╲        ╲●
 30 ┤        ╲         ╲●
 20 ┤         ╲          ╲●
 10 ┤          ╲           ╲●
  0 ┤           ╲____________●
    └────┬──┬──┬──┬──┬──┬──┬──┬──┬──┤
     D1  D2 D3 D4 D5 D6 D7 D8 D9 D10

── Planned (lý tưởng)
●── Actual (thực tế)

Nhận xét:
• Day 4-5: Stalled do phát hiện bug Priority
• Day 6: Bug fixed → Velocity tăng trở lại
• Day 10: Sprint hoàn thành 95% planned work

Sử dụng: https://www.burndownfortrello.com/
```

---

### SLIDE 13: TRELLO BOARD

```
📋 QUẢN LÝ TRÊN TRELLO

Board: "QuanLyCongViec - Sprint Management"

┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐
│ 📝       │ │ 🔄       │ │ 🧪       │ │ ✅       │ │ 🚀       │
│ Backlog  │ │ Doing    │ │ Testing  │ │ Done     │ │ Released │
│          │ │          │ │          │ │          │ │          │
│ ┌──────┐ │ │ ┌──────┐ │ │ ┌──────┐ │ │ ┌──────┐ │ │ ┌──────┐ │
│ │US-049│ │ │ │US-054│ │ │ │US-028│ │ │ │US-001│ │ │ │Auth  │ │
│ │Pages │ │ │ │Analyt│ │ │ │Board │ │ │ │Login │ │ │ │Module│ │
│ └──────┘ │ │ └──────┘ │ │ └──────┘ │ │ └──────┘ │ │ └──────┘ │
│ ┌──────┐ │ │ ┌──────┐ │ │ ┌──────┐ │ │ ┌──────┐ │ │          │
│ │US-060│ │ │ │US-041│ │ │ │US-029│ │ │ │US-014│ │ │          │
│ │AI    │ │ │ │Module│ │ │ │D&D   │ │ │ │Task  │ │ │          │
│ └──────┘ │ │ └──────┘ │ │ └──────┘ │ │ └──────┘ │ │          │
└──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘

Labels: 🔴 Critical  🟡 High  🟢 Medium  ⚪ Low
Members: Mỗi card gán đúng assignee

> Capture screenshots Trello board → Đưa vào file Word
> Sử dụng burndownfortrello.com để sinh Burndown Chart
```

---

### SLIDE 14: BIÊN BẢN HỌP SPRINT

```
📝 BIÊN BẢN HỌP SPRINT (Mẫu)

═══════════════════════════════════════
SPRINT 2 PLANNING MEETING
═══════════════════════════════════════
Ngày: [Ngày họp]
Thời gian: [Giờ - Giờ]
Thành viên: [Tất cả thành viên nhóm]
Người ghi: [Tên]

1. MỤC TIÊU SPRINT:
   Task CRUD + Kanban Board + Print/Cycle + Comments

2. USER STORIES CHỌN:
   US-014 đến US-033, US-037-039, US-043, US-046

3. PHÂN CÔNG:
   - [TV3]: Backend APIs (WorkTasks, Comments)
   - [TV4]: Frontend (TaskDetail, KanbanBoard, ListView)
   - [TV5]: Viết Test Cases

4. DEFINITION OF DONE:
   - Code review ✓
   - Test pass ✓
   - No critical bugs ✓
   - Responsive desktop ✓

5. RỦI RO:
   - Component quá lớn → Chia nhỏ
   - Sprint quá nhiều SP → Có thể cắt Calendar view

Ký tên: _________  _________  _________
═══════════════════════════════════════
```

---

### SLIDE 15: KẾT QUẢ SẢN PHẨM

```
🏆 KẾT QUẢ SẢN PHẨM

Tính năng đã hoàn thành:

✅ Authentication (Đăng ký/Đăng nhập/OAuth GitHub)
✅ Workspace & Project Management
✅ Task CRUD (15+ properties)
✅ Kanban Board + Drag & Drop
✅ List View + Sort + Inline Edit
✅ Calendar / Timeline / Spreadsheet Views
✅ Sprint/Cycle Management
✅ Module Management
✅ Comments + Emoji Reactions
✅ Labels + Filters + Custom Views
✅ Pages (WYSIWYG Editor)
✅ Sticky Notes + Drafts
✅ Notifications (Real-time SignalR)
✅ Analytics (Project + Global)
✅ Admin Panel + Audit Log
✅ AI Subtask Split
✅ Gamification (Points/Rewards)
✅ Dark Theme + Responsive Design

Thống kê:
• 24 API Controllers | 47 Entities | 100+ Endpoints
• 24 Components | 27 Views | 85 Test Cases
```

---

### SLIDE 16: KHÓ KHĂN GẶP PHẢI

```
😟 KHÓ KHĂN GẶP PHẢI

1. 🔧 KỸ THUẬT
   • Component quá lớn (TaskDetailModal.vue ~100KB)
     → Khó maintain, slow loading
   • Real-time sync (SignalR) phức tạp
     → Race condition khi nhiều user edit cùng lúc
   • Database migration conflict khi nhiều dev làm song song

2. 👥 NHÂN SỰ & GIAO TIẾP
   • Khác biệt skill level giữa các thành viên
   • Thời gian hạn chế (vừa học vừa làm dự án)
   • Khó duy trì daily standup đều đặn

3. 📋 QUY TRÌNH
   • Sprint 2 overload (100 SP - quá nhiều)
   • Estimate story points chưa chính xác
   • Thiếu automated testing ban đầu
   • Merge conflict khi nhiều người cùng sửa file chung

4. 🐛 BUGS
   • Priority dropdown không update UI ngay (cần F5)
     → Phát hiện qua QA testing
     → Fix bằng thêm selectPriority() handler
   • Static/mock data chưa được loại bỏ hết
```

---

### SLIDE 17: THUẬN LỢI

```
😊 THUẬN LỢI

1. 🛠️ CÔNG NGHỆ
   • .NET 10 + Vue 3 có ecosystem phong phú
   • Element Plus cung cấp component UI sẵn có
   • Vite hot-reload giúp phát triển nhanh
   • Entity Framework Code First → generate DB tự động

2. 📋 QUY TRÌNH SCRUM
   • Sprint ngắn (2 tuần) → feedback nhanh
   • Product Backlog rõ ràng → biết cần làm gì
   • Daily standup → phát hiện vấn đề sớm
   • Retrospective → cải tiến liên tục

3. 🔧 CÔNG CỤ
   • Trello: Quản lý task trực quan
   • GitHub: Quản lý code, review PR
   • VS Code + Copilot: Hỗ trợ code AI
   • burndownfortrello.com: Tự sinh burndown chart

4. 👥 TEAM
   • Thành viên có tinh thần hợp tác tốt
   • Code review giúp nâng cao chất lượng
   • Chia module rõ ràng → giảm conflict
```

---

### SLIDE 18: GIẢI PHÁP & ĐỀ XUẤT CẢI TIẾN

```
💡 GIẢI PHÁP & ĐỀ XUẤT CẢI TIẾN

1. 📏 CẢI TIẾN QUY TRÌNH
   • Giảm Story Points mỗi sprint: 60-80 SP thay vì 100
   • Planning Poker: Dùng kỹ thuật ước lượng tập thể
   • Sprint 0: Dành 1 sprint setup cơ bản trước
   • Code review checklist: Đảm bảo quality trước merge

2. 🔧 CẢI TIẾN KỸ THUẬT
   • Component splitting: Chia component >50KB
   • Automated testing: Viết unit test cho API
   • CI/CD Pipeline: GitHub Actions auto-test
   • Database migration tool: Quản lý schema changes

3. 👥 CẢI TIẾN NHÂN SỰ
   • Pair programming cho tasks phức tạp
   • Knowledge sharing session cuối mỗi sprint
   • Cross-training: Mỗi dev biết cả frontend lẫn backend

4. 📋 CẢI TIẾN GIAO TIẾP
   • Standup meeting online nếu không gặp trực tiếp
   • Sprint goal poster: Dán mục tiêu sprint visible
   • Impediment board: Track và giải quyết trở ngại

5. 🔒 CẢI TIẾN BẢO MẬT
   • IDOR protection trên mọi API
   • Rate limiting cho authentication endpoints
   • Input validation chặt chẽ hơn
```

---

### SLIDE 19: SO SÁNH TRƯỚC & SAU SCRUM

```
📊 SO SÁNH TRƯỚC & SAU ÁP DỤNG SCRUM

                    Trước Scrum          Sau Scrum
┌───────────────┬─────────────────┬─────────────────────┐
│ Kế hoạch      │ Waterfall       │ Sprint 2 tuần       │
│ Feedback      │ Cuối dự án      │ Mỗi 2 tuần          │
│ Thay đổi req  │ Khó thay đổi    │ Linh hoạt điều chỉnh│
│ Communication │ Email/Document   │ Daily Standup        │
│ Delivery      │ 1 lần cuối      │ Increment mỗi sprint│
│ Bug tracking  │ Cuối dự án      │ Liên tục trong sprint│
│ Team morale   │ Áp lực cuối     │ Đều đặn, rõ ràng    │
│ Risk          │ Phát hiện muộn  │ Phát hiện sớm       │
└───────────────┴─────────────────┴─────────────────────┘

Kết quả cụ thể:
• Sprint 1: Deliver đúng hạn 100%
• Sprint 2: Phát hiện & fix bug Priority trong 1 ngày
• Sprint 3: Bổ sung Calendar view theo feedback Sprint Review
• Sprint 4: Hoàn thành 95% planned features
```

---

### SLIDE 20: DEMO SẢN PHẨM

```
🖥️ DEMO SẢN PHẨM

Demo flow:
1. Login vào hệ thống
2. Tạo Workspace + Project
3. Tạo Task mới với properties
4. Xem Kanban Board + Drag & Drop
5. Chỉnh sửa Priority → Thấy update ngay (no F5!)
6. Tạo Sprint/Cycle + gán tasks
7. Bình luận + Emoji reaction
8. Xem Analytics dashboard
9. Tạo Page với WYSIWYG editor
10. Xem responsive trên mobile

URL Demo:
• Frontend: http://localhost:5173
• Backend API: http://localhost:5136

Test Account:
• Email: test@example.com
• Password: Test@123
```

---

### SLIDE 21: CẢM ƠN & Q&A

```
┌──────────────────────────────────────────────┐
│                                              │
│          🙏 CẢM ƠN QUÝ THẦY/CÔ             │
│             VÀ CÁC BẠN ĐÃ LẮNG NGHE!       │
│                                              │
│                                              │
│              ❓ HỎI ĐÁP (Q&A)               │
│                                              │
│                                              │
│    Repository: github.com/[username]/        │
│                QuanLyCongViec                │
│                                              │
│    Liên hệ:                                 │
│    📧 [email nhóm trưởng]                    │
│    📱 [SĐT liên hệ]                         │
│                                              │
│                                              │
│         Nhóm: [Tên nhóm] - [Mã lớp]         │
│                                              │
└──────────────────────────────────────────────┘
```

---

## PHỤ LỤC: GHI CHÚ CHO LÀM SLIDE POWERPOINT

### Thiết kế đề xuất

| Yếu tố | Đề xuất |
|---------|---------|
| **Theme** | Dark background (#1E1E2E) + gradient accent |
| **Font** | Tiêu đề: Montserrat Bold / Nội dung: Inter Regular |
| **Màu chính** | Blue gradient (#3B82F6 → #6366F1) |
| **Màu phụ** | Green (#22C55E), Orange (#F97316), Red (#EF4444) |
| **Animation** | Fade-in từng bullet point, slide transition: Morph |
| **Số slide** | 21 slides (bao gồm trang bìa và Q&A) |
| **Thời gian** | ~15-20 phút trình bày |

### Hình ảnh cần chụp/tạo

1. Screenshot Trello board (5-6 lists)
2. Screenshot Burndown chart từ burndownfortrello.com
3. Screenshot sản phẩm: Login, Dashboard, Kanban Board, Task Detail
4. Screenshot GitHub repository (commits, branches)
5. Sơ đồ kiến trúc hệ thống (Visio hoặc draw.io)

---

**Người lập:** ___________  
**Ngày tạo:** 19/04/2026  
**Phiên bản:** 1.0
