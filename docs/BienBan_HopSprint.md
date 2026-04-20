# 📝 BIÊN BẢN CÁC CUỘC HỌP SPRINT

**Dự án:** Hệ thống Quản lý Công việc (SprintA)  
**Ngày lập:** 19/04/2026

---

## BIÊN BẢN HỌP SPRINT 1

### Sprint 1 Planning Meeting

| Thuộc tính | Giá trị |
|-----------|---------|
| **Ngày:** | [Ngày bắt đầu Sprint 1] |
| **Thời gian:** | [Giờ bắt đầu - Giờ kết thúc] |
| **Thành viên tham dự:** | [Tất cả thành viên] |
| **Người ghi:** | [Tên] |

**1. Mục tiêu Sprint 1:**
- Xây dựng nền tảng: Authentication, Workspace, Project
- Đăng ký, đăng nhập, quản lý hồ sơ
- CRUD Workspace + mời thành viên
- CRUD Project + quản lý thành viên project
- Sidebar navigation + Dark mode

**2. User Stories được chọn:**
- US-001 → US-013 (Auth + Workspace + Project)
- US-065 (Sidebar Navigation)
- US-066 (Dark Mode)

**3. Phân công:**

| Task | Người phụ trách | Est (h) |
|------|----------------|---------|
| DB Schema User, RefreshToken | | |
| API Auth (register, login, refresh) | | |
| Frontend Login.vue, Register.vue | | |
| DB Schema Workspace, WorkspaceMember | | |
| API Workspace CRUD + Members | | |
| Frontend ManageSpaces.vue | | |
| DB Schema Project, ProjectMember | | |
| API Project CRUD + Members | | |
| Frontend CreateProjectModal.vue | | |
| Sidebar Layout + Dark Mode CSS | | |
| Viết Test Cases cho Auth + Project | | |

**4. Definition of Done:**
- [ ] Code review PASS
- [ ] API response đúng format chuẩn
- [ ] Frontend hiển thị đúng
- [ ] Test case PASS
- [ ] Không có critical bugs

**5. Rủi ro nhận diện:**
- Database schema cần thống nhất sớm
- RBAC phức tạp, cần thiết kế kỹ

**Ký tên:**
- Scrum Master: ___________
- Product Owner: ___________
- Dev Team: ___________

---

### Sprint 1 Daily Standup Summary

| Ngày | Thành viên | Yesterday | Today | Blockers |
|------|-----------|-----------|-------|----------|
| Day 1 | | | | |
| Day 2 | | | | |
| Day 3 | | | | |
| Day 4 | | | | |
| Day 5 | | | | |
| Day 6 | | | | |
| Day 7 | | | | |
| Day 8 | | | | |
| Day 9 | | | | |
| Day 10 | | | | |

---

### Sprint 1 Review Meeting

| Thuộc tính | Giá trị |
|-----------|---------|
| **Ngày:** | [Ngày kết thúc Sprint 1] |
| **Thành viên tham dự:** | [Tất cả] |

**1. Demo items:**
- [ ] Đăng ký tài khoản mới
- [ ] Đăng nhập / Đăng xuất
- [ ] Tạo Workspace + mời thành viên
- [ ] Tạo Project + quản lý members
- [ ] Sidebar navigation
- [ ] Dark mode theme

**2. Feedback từ PO/stakeholders:**
- 
- 
- 

**3. User Stories hoàn thành:**

| US ID | Story | Status |
|-------|-------|--------|
| US-001 | Đăng ký | |
| US-002 | Đăng nhập | |
| US-004 | Profile | |
| US-005 | Token refresh | |
| US-006 | Workspace CRUD | |
| US-007 | Invite member | |
| US-008 | Role management | |
| US-009 | Project create | |
| US-010 | Project list | |
| US-011 | Project edit | |
| US-012 | Project delete | |
| US-013 | Project members | |
| US-065 | Sidebar | |
| US-066 | Dark mode | |

**4. Velocity Sprint 1:** ___ Story Points

---

### Sprint 1 Retrospective

**Went Well (Tốt):**
- 
- 
- 

**Didn't Go Well (Chưa tốt):**
- 
- 
- 

**Improvements (Cải tiến):**
- 
- 
- 

**Action Items:**

| Action | Owner | Deadline |
|--------|-------|----------|
| | | |
| | | |

---

## BIÊN BẢN HỌP SPRINT 2

### Sprint 2 Planning Meeting

| Thuộc tính | Giá trị |
|-----------|---------|
| **Ngày:** | [Ngày bắt đầu Sprint 2] |
| **Thời gian:** | [Giờ bắt đầu - Giờ kết thúc] |
| **Thành viên tham dự:** | [Tất cả] |

**1. Mục tiêu Sprint 2:**
- Task CRUD với đầy đủ 15+ properties
- Kanban Board + Drag & Drop
- List View + Sort + Inline Edit
- Cycles (Sprint) Management
- Comments + Labels

**2. User Stories được chọn:**
- US-014 → US-033 (Task + Board + List)
- US-037 → US-039 (Cycles)
- US-043 (Comments)
- US-046 (Labels)

**3. Phân công:**

| Task | Người phụ trách | Est (h) |
|------|----------------|---------|
| DB Schema WorkTask, TaskStatus | | |
| API WorkTasks CRUD | | |
| TaskDetailModal.vue (15+ properties) | | |
| Status/Priority dropdowns | | |
| Assignees management | | |
| Labels CRUD + gán label | | |
| Date pickers (start/due) | | |
| Rich text editor description | | |
| Subtask creation | | |
| KanbanBoard.vue + Drag & Drop | | |
| ListView.vue + Sort + Inline edit | | |
| Sprint CRUD API | | |
| CyclesTab.vue | | |
| Comments API + UI | | |
| Test Cases Sprint 2 | | |

**4. Rủi ro:**
- Sprint 2 có nhiều SP (~100), cần monitor velocity
- Kanban drag & drop phức tạp

---

### Sprint 2 Daily Standup Summary

| Ngày | Thành viên | Yesterday | Today | Blockers |
|------|-----------|-----------|-------|----------|
| Day 1 | | | | |
| Day 2 | | | | |
| Day 3 | | | | |
| Day 4 | | | | |
| Day 5 | | Bug phát hiện: Priority cần F5 | Fix bug Priority | 🚫 Priority bug |
| Day 6 | | Fix xong bug Priority → selectPriority() | Test regression | ✅ Bug resolved |
| Day 7 | | | | |
| Day 8 | | | | |
| Day 9 | | | | |
| Day 10 | | | | |

---

### Sprint 2 Review Meeting

**1. Demo items:**
- [ ] Tạo Task với full properties
- [ ] Edit title inline (no refresh)
- [ ] Change Status dropdown
- [ ] **Change Priority dropdown → Update NGAY (no F5)** ⭐
- [ ] Assign members + % progress
- [ ] Labels + Date pickers
- [ ] Rich text editor
- [ ] Subtask creation
- [ ] Kanban Board + Drag & Drop
- [ ] List View + Sorting
- [ ] Cycles CRUD + assign tasks
- [ ] Comments + edit/delete

**2. Bug đã fix:**
- BUG-001: Priority selection cần F5 refresh → ✅ FIXED

**3. Velocity Sprint 2:** ___ Story Points

---

### Sprint 2 Retrospective

**Went Well:**
- Bug Priority phát hiện nhanh qua QA testing
- Fix bug trong 1 ngày (selectPriority handler)
- Kanban drag & drop hoạt động mượt

**Didn't Go Well:**
- Sprint quá nhiều SP (100) → áp lực
- TaskDetailModal.vue quá lớn (~100KB)

**Improvements:**
- Giảm SP sprint tiếp theo xuống 80
- Chia nhỏ component lớn
- Thêm automated testing

---

## BIÊN BẢN HỌP SPRINT 3

### Sprint 3 Planning Meeting

| Thuộc tính | Giá trị |
|-----------|---------|
| **Ngày:** | [Ngày bắt đầu Sprint 3] |

**1. Mục tiêu Sprint 3:**
- Calendar, Timeline, Spreadsheet views
- Modules management
- Notifications + Real-time (SignalR)
- Pages (WYSIWYG), Sticky Notes, Drafts
- Custom Views + Filters
- Project + Global Analytics

**2. User Stories:** US-034→US-055, US-063, US-067

**3. Phân công:**

| Task | Người phụ trách | Est (h) |
|------|----------------|---------|
| CalendarTab.vue | | |
| TimelineTab.vue (Gantt) | | |
| SpreadsheetTab.vue | | |
| Module CRUD API + ModulesTab.vue | | |
| Notification API + NotificationsDropdown.vue | | |
| SignalR Hub setup | | |
| Pages CRUD + PagesTab.vue (WYSIWYG) | | |
| StickiesView.vue | | |
| DraftsView.vue | | |
| Custom Views API + ViewsTab.vue | | |
| FilterBar.vue enhanced | | |
| GlobalAnalyticsView.vue | | |
| Responsive testing | | |

---

### Sprint 3 Daily Standup Summary

| Ngày | Thành viên | Yesterday | Today | Blockers |
|------|-----------|-----------|-------|----------|
| Day 1-10 | | | | |

---

### Sprint 3 Review & Retrospective

**Demo items:**
- [ ] Calendar view hiển thị tasks
- [ ] Timeline/Gantt chart
- [ ] Spreadsheet view
- [ ] Module CRUD + assign tasks
- [ ] Notifications dropdown
- [ ] Pages WYSIWYG editor
- [ ] Sticky Notes
- [ ] Custom Views + Filters
- [ ] Analytics charts

**Velocity Sprint 3:** ___ Story Points

---

## BIÊN BẢN HỌP SPRINT 4

### Sprint 4 Planning Meeting

| Thuộc tính | Giá trị |
|-----------|---------|
| **Ngày:** | [Ngày bắt đầu Sprint 4] |

**1. Mục tiêu Sprint 4:**
- Admin panel + Audit Log
- AI Split Subtasks
- Gamification / Rewards
- Full regression testing (85 test cases)
- Bug fixing + UI polish
- Tài liệu + Slide

**2. User Stories:** US-003, US-027, US-056→US-064

**3. Phân công:**

| Task | Người phụ trách | Est (h) |
|------|----------------|---------|
| Admin Users management | | |
| Departments management | | |
| Audit Log view | | |
| AI subtask feature | | |
| Gamification points system | | |
| GitHub OAuth | | |
| Task Dependencies | | |
| Regression testing 85 TC | | |
| Bug fixing | | |
| UI Polish | | |
| Slide trình bày Scrum | | |
| Tài liệu nộp bài | | |

---

### Sprint 4 Review (Final)

**Final Demo:**
- [ ] Full system walkthrough
- [ ] Admin panel
- [ ] AI features
- [ ] Gamification
- [ ] All 85 test cases executed

**Final Velocity:** ___ Story Points  
**Total Velocity (4 Sprints):** ___ Story Points

---

**Người lập:** ___________  
**Ngày tạo:** 19/04/2026
