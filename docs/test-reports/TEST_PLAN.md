# 🧪 KỊ HOẠCH TEST DỰ ÁN QUẢN LÝ CÔNG VIỆC

**Phạm vi:** Toàn bộ dự án trừ Admin Đăng ký/Đăng nhập  
**Ngày test:** 2026-04-18  
**Tester:** QA Team

---

## 📋 PHẦN I: AUTHENTICATION (Đăng nhập - không bao gồm Đăng ký Admin)

### TC-AUTH-01: Đăng nhập với tài khoản hợp lệ
- **Bước:**
  1. Truy cập trang Login
  2. Nhập email hợp lệ
  3. Nhập mật khẩu đúng
  4. Click nút "Login"
- **Kỳ vọng:** Đăng nhập thành công, chuyển đến dashboard

### TC-AUTH-02: Đăng nhập với email sai
- **Bước:** Nhập email không tồn tại, mật khẩu bất kì
- **Kỳ vọng:** Hiển thị lỗi "Email hoặc mật khẩu không chính xác"

### TC-AUTH-03: Đăng nhập với mật khẩu sai
- **Bước:** Nhập email đúng, mật khẩu sai
- **Kỳ vọng:** Hiển thị lỗi "Email hoặc mật khẩu không chính xác"

---

## 📋 PHẦN II: WORKSPACE/PROJECT MANAGEMENT

### TC-PROJ-01: Tạo project mới
- **Bước:**
  1. Click "New Project" hoặc "+"
  2. Nhập tên project
  3. Chọn icon/màu
  4. Click "Create"
- **Kỳ vọng:** Project được tạo và hiển thị trong list

### TC-PROJ-02: Xem danh sách projects
- **Bước:** Vào trang Projects
- **Kỳ vọng:** Tất cả projects hiển thị đúng thông tin

### TC-PROJ-03: Chỉnh sửa thông tin project
- **Bước:**
  1. Chọn project
  2. Click settings/edit
  3. Thay đổi tên, mô tả, icon
  4. Click "Save"
- **Kỳ vọng:** Thay đổi được lưu ngay

### TC-PROJ-04: Xóa project
- **Bước:**
  1. Chọn project
  2. Click delete icon
  3. Xác nhận xóa
- **Kỳ vọng:** Project bị xóa khỏi list

---

## 📋 PHẦN III: TASK MANAGEMENT (Công việc)

### TC-TASK-01: Tạo công việc mới
- **Bước:**
  1. Vào project
  2. Click "New Work Item" hoặc "+"
  3. Nhập tiêu đề
  4. Nhập mô tả (optional)
  5. Chọn State (Status)
  6. Chọn Priority
  7. Click "Save"
- **Kỳ vọng:** Công việc được tạo, xuất hiện trong board/list

### TC-TASK-02: Xem chi tiết công việc
- **Bước:**
  1. Vào project
  2. Click vào một công việc
  3. Slide panel chi tiết mở từ bên phải
- **Kỳ vọng:** Hiển thị đầy đủ thông tin: tiêu đề, mô tả, assignee, priority, labels, dates, v.v.

### TC-TASK-03: Chỉnh sửa tiêu đề công việc ✅ **FIXED**
- **Bước:**
  1. Mở detail task
  2. Click vào tiêu đề
  3. Sửa text
  4. Click outside để lưu
- **Kỳ vọng:** Tiêu đề cập nhật ngay, không cần F5

### TC-TASK-04: Thay đổi Status (State)
- **Bước:**
  1. Mở detail task
  2. Tại section Properties → State
  3. Click dropdown
  4. Chọn status khác (To Do, In Progress, Review, Done, Cancelled)
- **Kỳ vọng:** Status cập nhật ngay, không cần F5

### TC-TASK-05: Thay đổi Priority ✅ **FIXED**
- **Bước:**
  1. Mở detail task
  2. Tại section Properties → Priority
  3. Click dropdown
  4. Chọn priority: Urgent/High/Medium/Low/None
- **Kỳ vọng:** Priority cập nhật ngay, Icon + Text thay đổi, không cần F5

### TC-TASK-06: Gán người chỉ định (Assignees)
- **Bước:**
  1. Mở detail task
  2. Properties → Assignees
  3. Click dropdown
  4. Search và chọn members
  5. Nhập % tiến độ cho mỗi assignee
- **Kỳ vọng:** Assignees được gán, tiến độ được lưu

### TC-TASK-07: Thêm/Xóa Labels
- **Bước:**
  1. Mở detail task
  2. Properties → Labels
  3. Search label hoặc tạo mới
  4. Click để thêm/xóa
- **Kỳ vọng:** Labels được cập nhật, hiển thị đúng màu sắc

### TC-TASK-08: Chỉnh sửa Start Date
- **Bước:**
  1. Mở detail task
  2. Properties → Start date
  3. Click date picker
  4. Chọn ngày
- **Kỳ vọng:** Ngày được lưu, format hiển thị đúng (MMM DD)

### TC-TASK-09: Chỉnh sửa Due Date
- **Bước:**
  1. Mở detail task
  2. Properties → Due date
  3. Click date picker
  4. Chọn ngày
- **Kỳ vọng:** Ngày được lưu

### TC-TASK-10: Gán Module
- **Bước:**
  1. Mở detail task
  2. Properties → Module
  3. Search và chọn module
- **Kỳ vọng:** Module được gán, tên hiển thị đúng

### TC-TASK-11: Gán Cycle (Sprint)
- **Bước:**
  1. Mở detail task
  2. Properties → Cycle
  3. Search và chọn cycle
- **Kỳ vọng:** Cycle được gán

### TC-TASK-12: Gán Parent Task
- **Bước:**
  1. Mở detail task
  2. Properties → Parent
  3. Search parent task
  4. Click để gán
- **Kỳ vọng:** Parent task được gán, hiển thị ID + tiêu đề

### TC-TASK-13: Chỉnh sửa Description
- **Bước:**
  1. Mở detail task
  2. Click vào description area
  3. Sử dụng rich editor:
     - Format text (Bold, Italic, Underline)
     - Chọn Heading 1/2/3
     - Quote, Color, Alignment
     - Insert image/file
  4. Click outside để lưu
- **Kỳ vọng:** Description được lưu, formatting được giữ

### TC-TASK-14: Tạo Sub-work item (Subtask)
- **Bước:**
  1. Mở detail task
  2. Click "Add sub-work item"
  3. Nhập tiêu đề subtask
  4. Click "Create"
  5. Click subtask để xem chi tiết
- **Kỳ vọng:** Subtask được tạo và hiển thị trong danh sách

### TC-TASK-15: AI Split vào Subtasks
- **Bước:**
  1. Mở detail task
  2. Click "AI split into subtasks"
  3. Chờ AI xử lý
- **Kỳ vọng:** AI tạo multiple subtasks từ task chính

### TC-TASK-16: Xóa công việc
- **Bước:**
  1. Mở detail task
  2. Click menu (...) → Delete
  3. Xác nhận xóa
- **Kỳ vọng:** Công việc bị xóa khỏi board

### TC-TASK-17: Attach Files
- **Bước:**
  1. Mở detail task
  2. Click "Attach"
  3. Chọn file từ computer
- **Kỳ vọng:** File được upload, hiển thị trong attachments

---

## 📋 PHẦN IV: BOARD VIEW (Kanban)

### TC-BOARD-01: Xem Kanban board
- **Bước:**
  1. Vào project
  2. Chọn view "Board" hoặc Kanban
- **Kỳ vọng:** Hiển thị các cột theo status (To Do, In Progress, Review, Done, etc.)

### TC-BOARD-02: Drag & Drop task giữa các cột
- **Bước:**
  1. Xem Kanban board
  2. Drag một task từ cột "To Do" sang "In Progress"
- **Kỳ vọng:** Task được di chuyển, status cập nhật ngay

### TC-BOARD-03: Filter tasks trên board
- **Bước:**
  1. Xem Kanban board
  2. Click filter icon
  3. Chọn filter (by priority, assignee, label)
- **Kỳ vọng:** Board hiển thị tasks theo filter

---

## 📋 PHẦN V: LIST VIEW (Danh sách)

### TC-LIST-01: Xem task list
- **Bước:**
  1. Vào project
  2. Chọn view "List"
- **Kỳ vọng:** Hiển thị table với các cột: ID, Title, Status, Priority, Assignee, Due Date

### TC-LIST-02: Sort tasks
- **Bước:**
  1. Xem list view
  2. Click vào header column (Title, Status, Priority)
- **Kỳ vọng:** Tasks được sort A-Z, Z-A theo column

### TC-LIST-03: Inline edit từ list
- **Bước:**
  1. Xem list view
  2. Click vào Priority cell
  3. Chọn priority khác
- **Kỳ vọng:** Priority cập nhật ngay trong table

---

## 📋 PHẦN VI: CYCLES & SPRINTS

### TC-CYCLE-01: Tạo cycle mới
- **Bước:**
  1. Vào project
  2. Chọn tab "Cycles"
  3. Click "New Cycle"
  4. Nhập tên cycle
  5. Set start date, end date
  6. Click "Create"
- **Kỳ vọng:** Cycle được tạo

### TC-CYCLE-02: Gán tasks vào cycle
- **Bước:**
  1. Mở detail task
  2. Properties → Cycle
  3. Chọn cycle
- **Kỳ vọng:** Task được gán vào cycle

### TC-CYCLE-03: Xem tasks trong cycle
- **Bước:**
  1. Click vào cycle
- **Kỳ vọng:** Hiển thị danh sách tasks trong cycle

---

## 📋 PHẦN VII: MODULES

### TC-MODULE-01: Tạo module mới
- **Bước:**
  1. Vào project
  2. Chọn tab "Modules"
  3. Click "New Module"
  4. Nhập tên module
  5. Click "Create"
- **Kỳ vọng:** Module được tạo

### TC-MODULE-02: Gán tasks vào module
- **Bước:**
  1. Mở detail task
  2. Properties → Module
  3. Chọn module
- **Kỳ vọng:** Task được gán

---

## 📋 PHẦN VIII: LABELS

### TC-LABEL-01: Tạo label mới
- **Bước:**
  1. Vào project
  2. Click "Labels" setting
  3. Click "New Label"
  4. Nhập tên, chọn màu
  5. Click "Create"
- **Kỳ vọng:** Label được tạo

### TC-LABEL-02: Gán label cho task
- **Bước:**
  1. Mở detail task
  2. Properties → Labels
  3. Search label
  4. Click để gán
- **Kỳ vọng:** Label được gán, hiển thị đúng màu sắc

### TC-LABEL-03: Xóa label
- **Bước:**
  1. Vào project Labels setting
  2. Click delete icon trên label
- **Kỳ vọng:** Label bị xóa, không còn hiển thị

---

## 📋 PHẦN IX: COMMENTS & ACTIVITY

### TC-COMMENT-01: Thêm comment
- **Bước:**
  1. Mở detail task
  2. Scroll đến section "Activity"
  3. Nhập comment
  4. Click "Submit"
- **Kỳ vọng:** Comment hiển thị với tên user, thời gian

### TC-COMMENT-02: Edit comment
- **Bước:**
  1. Xem comment
  2. Hover, click menu (...)
  3. Click "Edit"
  4. Chỉnh sửa text
  5. Click "Save"
- **Kỳ vọng:** Comment được update, có dòng "(edited)"

### TC-COMMENT-03: Delete comment
- **Bước:**
  1. Hover comment
  2. Click menu (...)
  3. Click "Delete"
  4. Xác nhận
- **Kỳ vọng:** Comment bị xóa

### TC-COMMENT-04: Add reaction (emoji)
- **Bước:**
  1. Hover comment
  2. Click emoji icon
  3. Chọn emoji
- **Kỳ vọng:** Emoji reaction hiển thị dưới comment

---

## 📋 PHẦN X: TEAM & MEMBERS

### TC-TEAM-01: Xem danh sách members
- **Bước:**
  1. Vào project
  2. Click "Settings" → "Members"
- **Kỳ vọng:** Hiển thị danh sách project members

### TC-TEAM-02: Invite member (nếu có feature)
- **Bước:**
  1. Vào project settings
  2. Click "Invite member"
  3. Nhập email
  4. Chọn role
  5. Click "Send invite"
- **Kỳ vọng:** Invite được gửi

### TC-TEAM-03: Remove member
- **Bước:**
  1. Vào project Members
  2. Click remove icon trên member
  3. Xác nhận
- **Kỳ vọng:** Member bị remove khỏi project

---

## 📋 PHẦN XI: ANALYTICS & REPORTS

### TC-ANALYTICS-01: Xem Project Analytics
- **Bước:**
  1. Vào project
  2. Click "Analytics" tab
- **Kỳ vọng:** Hiển thị:
  - Tổng tasks
  - Tasks by status
  - Tasks by priority
  - Tasks by assignee

### TC-ANALYTICS-02: Xem Global Analytics
- **Bước:**
  1. Vào Global Views
  2. Click "Analytics"
- **Kỳ vọng:** Hiển thị stats toàn bộ workspace

---

## 📋 PHẦN XII: VIEWS & FILTERING

### TC-VIEW-01: Tạo custom view
- **Bước:**
  1. Vào project
  2. Click "+" ngoài view list
  3. Nhập tên view
  4. Chọn view type (List, Kanban, Calendar)
  5. Set filters/sorting
  6. Click "Create"
- **Kỳ vọng:** View được tạo

### TC-VIEW-02: Filter tasks
- **Bước:**
  1. Trong view
  2. Click filter icon
  3. Set filter: by assignee, status, priority, label, date
- **Kỳ vọng:** Tasks được filter đúng

### TC-VIEW-03: Save filter as view
- **Bước:**
  1. Set filters
  2. Click "Save as view"
  3. Nhập tên
- **Kỳ vọng:** View được save

---

## 📋 PHẦN XIII: NAVIGATION & UI

### TC-NAV-01: Sidebar navigation
- **Bước:**
  1. Kiểm tra sidebar
  2. Click vào projects/views
- **Kỳ vọng:** Chuyển trang đúng, active state hiển thị

### TC-NAV-02: Breadcrumb navigation
- **Bước:**
  1. Xem breadcrumb
  2. Click các links trong breadcrumb
- **Kỳ vọng:** Chuyển trang đúng

### TC-NAV-03: Dark mode toggle
- **Bước:**
  1. Click theme toggle
- **Kỳ vọng:** Giao diện chuyển dark/light mode

---

## 📋 PHẦN XIV: RESPONSIVENESS

### TC-RESP-01: Test trên desktop (1920px)
- **Kỳ vọng:** Layout hiển thị đầy đủ, không bị cut off

### TC-RESP-02: Test trên tablet (768px)
- **Kỳ vọng:** Sidebar ẩn/collapse, content responsive

### TC-RESP-03: Test trên mobile (375px)
- **Kỳ vọng:** Mobile menu hoạt động, touch-friendly

---

## 📋 PHẦN XV: PERFORMANCE

### TC-PERF-01: Load project với 100+ tasks
- **Kỳ vọng:** Page load < 3s, scrolling smooth

### TC-PERF-02: Open detail task
- **Kỳ vọng:** Panel slide open < 1s

### TC-PERF-03: Search tasks
- **Bước:** Nhập search term
- **Kỳ vọng:** Results hiển thị < 500ms

---

## 📋 PHẦN XVI: ERROR HANDLING

### TC-ERROR-01: Network timeout
- **Bước:** Tắt internet khi updating task
- **Kỳ vọng:** Hiển thị error message, cho retry

### TC-ERROR-02: Invalid input
- **Bước:** Tạo task với title rỗng
- **Kỳ vọng:** Hiển thị validation error "Title is required"

### TC-ERROR-03: Delete confirmation
- **Bước:** Delete task mà không xác nhận
- **Kỳ vọng:** Hiển thị confirmation dialog

---

## 🎯 REGRESSION TEST

Chạy lại các test trên mỗi build mới:
- [ ] TC-TASK-03 (Edit tiêu đề)
- [ ] TC-TASK-04 (Change status)
- [ ] **TC-TASK-05 (Change priority) ✅ FIXED**
- [ ] TC-TASK-06 (Assign members)
- [ ] TC-COMMENT-01 (Add comment)

---

## 📝 NOTES

- **Lỗi đã fix:** Priority selection không cần F5 refresh lên (selectPriority handler added)
- **Test environment:** http://localhost:5173 (Frontend) + Backend API
- **Browser:** Chrome latest, Firefox latest
- **Database:** Test DB được seed với sample data

---

## ✅ PASS/FAIL CRITERIA

| Criteria | Status |
|----------|--------|
| Tất cả TC phải PASS | Pending |
| Không có critical bugs | Pending |
| Performance < 3s | Pending |
| Responsive OK trên 3 devices | Pending |

---

**Ngày hoàn thành test:** ___________  
**Người test:** ___________  
**Ghi chú:** ___________

