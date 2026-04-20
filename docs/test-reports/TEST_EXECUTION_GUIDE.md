# 🚀 TEST EXECUTION GUIDE - Hướng dẫn thực thi test

**Ngày:** 2026-04-18  
**Version:** 1.0

---

## 📌 CHUẨN BỊ TRƯỚC KHI TEST

### 1️⃣ Môi trường
```bash
# Frontend - Port 5173
cd Frontend
npm install
npm run dev

# Backend - Port 5136
cd Backend/src/TaskManagement.API
dotnet run

# Database - Tạo test data
# Chạy seed script nếu có
```

### 2️⃣ Test Account
```
Email: test@example.com
Password: Test@123
Name: Test User
Role: Team Member
```

### 3️⃣ Kiểm tra trước
- [ ] Frontend load thành công (http://localhost:5173)
- [ ] Backend API response OK (http://localhost:5136/api)
- [ ] Database kết nối OK
- [ ] Network không có timeout
- [ ] Browser console không có critical errors

---

## 🎯 TEST EXECUTION FLOW

### **DAY 1: SMOKE TEST (30 phút)**

```
✅ TC-AUTH-01: Đăng nhập thành công
   └─ Expected: Redirect dashboard

✅ TC-PROJ-02: Xem danh sách projects
   └─ Expected: Hiển thị 2-3 projects

✅ TC-TASK-01: Tạo task mới
   └─ Expected: Task xuất hiện ngay

✅ TC-TASK-02: Xem chi tiết task
   └─ Expected: Detail panel mở từ phải

✅ TC-TASK-05: Chỉnh sửa priority ⭐ NEW FIX
   └─ Expected: Priority cập nhật NGAY (không cần F5)
```

**Nếu tất cả PASS → Tiếp tục. Nếu FAIL → Dừng, report bug.**

---

### **DAY 2: CORE FUNCTIONALITY (2 giờ)**

#### **Block 1: Task Properties (45 phút)**
Mở 1 task và test tuần tự:

```
✅ TC-TASK-03: Edit title
   └─ Sửa text → Click outside → SAVE
   └─ Expected: Không cần F5

✅ TC-TASK-04: Change status
   └─ Click Status dropdown → Chọn "In Progress"
   └─ Expected: Cập nhật ngay, icon thay đổi

✅ TC-TASK-05: Change priority ⭐
   └─ Click Priority → Chọn "High"
   └─ Expected: Icon + Text update ngay (RED)
   └─ [CRITICAL FIX] Không cần refresh!

✅ TC-TASK-06: Add assignees
   └─ Click Assignees → Select member → Add %
   └─ Expected: Member gán OK, % lưu

✅ TC-TASK-08: Set start date
   └─ Click date picker → Chọn +5 days
   └─ Expected: Hiển thị format "MMM DD"

✅ TC-TASK-09: Set due date
   └─ Click due date → Chọn +10 days
   └─ Expected: Due date lưu

✅ TC-TASK-07: Add labels
   └─ Click Labels → Select "Bug" label (màu xanh)
   └─ Expected: Label hiển thị đúng màu
```

**Checkpoint 1:** Tất cả properties OK?

#### **Block 2: Board & List View (45 phút)**

```
✅ TC-BOARD-01: Xem Kanban board
   └─ Click "Board" tab
   └─ Expected: 5+ cột theo status

✅ TC-BOARD-02: Drag task
   └─ Drag task từ "Todo" → "In Progress"
   └─ Expected: Status cập nhật, task di chuyển

✅ TC-LIST-01: Xem list view
   └─ Click "List" tab
   └─ Expected: Table với columns: ID, Title, Status, Priority, Assignee

✅ TC-LIST-03: Inline edit priority
   └─ Click Priority cell → Chọn "Medium"
   └─ Expected: Update ngay trong table
```

**Checkpoint 2:** Board & List hoạt động OK?

#### **Block 3: Subtasks & Comments (30 phút)**

```
✅ TC-TASK-14: Create subtask
   └─ Click "Add sub-work item"
   └─ Nhập title: "Subtask 1"
   └─ Expected: Subtask hiển thị dưới main task

✅ TC-COMMENT-01: Add comment
   └─ Type: "Testing comment..."
   └─ Click Submit
   └─ Expected: Comment hiển thị ngay

✅ TC-COMMENT-02: Edit comment
   └─ Hover, click (...)
   └─ Click Edit
   └─ Sửa text
   └─ Expected: Comment updated, có "(edited)"

✅ TC-COMMENT-04: Add emoji
   └─ Click emoji icon
   └─ Select emoji
   └─ Expected: Reaction hiển thị
```

---

### **DAY 3: ADVANCED FEATURES (2 giờ)**

#### **Block 1: Cycles & Modules (30 phút)**

```
✅ TC-CYCLE-01: Create cycle
   └─ Click "Cycles" → "New Cycle"
   └─ Name: "Sprint 1", Date: Today - +14 days
   └─ Expected: Cycle tạo OK

✅ TC-CYCLE-02: Assign task to cycle
   └─ Open task detail
   └─ Cycle → Select "Sprint 1"
   └─ Expected: Task gán vào cycle

✅ TC-MODULE-01: Create module
   └─ Click "Modules" → "New Module"
   └─ Name: "Backend"
   └─ Expected: Module tạo OK

✅ TC-MODULE-02: Assign module
   └─ Open task
   └─ Module → Select "Backend"
   └─ Expected: Task gán vào module
```

#### **Block 2: Advanced Editing (30 phút)**

```
✅ TC-TASK-13: Rich editor test
   └─ Click description area
   └─ Test:
      - Bold text: "**important**"
      - Heading: Select "Heading 1"
      - Color: Change text color
      - Insert image: Upload file
   └─ Click outside
   └─ Expected: Formatting lưu OK

✅ TC-TASK-12: Assign parent task
   └─ Click "Parent" → Search "Feature X"
   └─ Expected: Parent assigned
```

#### **Block 3: Filtering & Views (30 phút)**

```
✅ TC-VIEW-01: Create custom view
   └─ Click "+" next to views
   └─ Name: "High Priority"
   └─ Type: "List"
   └─ Filter: Priority = High
   └─ Expected: View created

✅ TC-VIEW-02: Filter tasks
   └─ In any view
   └─ Click filter icon
   └─ Set: Status = "In Progress"
   └─ Expected: Only In Progress tasks show

✅ TC-BOARD-03: Filter on board
   └─ Click filter on board
   └─ Set: Assignee = "Me"
   └─ Expected: Only my tasks show
```

#### **Block 4: Analytics (30 phút)**

```
✅ TC-ANALYTICS-01: View project analytics
   └─ Click "Analytics" tab
   └─ Expected:
      - Total tasks: > 0
      - Status breakdown: Shows pie/bar chart
      - Priority breakdown: Shows distribution
      - Assignee breakdown: Shows workload

✅ TC-ANALYTICS-02: Global analytics
   └─ Go to "Global Analytics" view
   └─ Expected: Workspace-wide stats
```

---

### **DAY 4: EDGE CASES & REGRESSION (1.5 giờ)**

#### **Block 1: Regression (Critical Fixes)**

```
⭐ TC-TASK-05: Priority fix verification
   └─ Priority cập nhật KHÔNG cần F5 (MAIN FIX)
   └─ Steps:
      1. Open task detail
      2. Change priority 3 lần: High → Medium → Urgent
      3. Kiểm tra: UI update ngay sau mỗi click
      4. Close detail, reopen → priority vẫn đúng
   └─ Expected: 100% SUCCESS (không cần refresh)
   └─ Status: ✅ FIXED & VERIFIED
```

#### **Block 2: Edge Cases**

```
✅ TC-ERROR-02: Create task without title
   └─ Click "New Work Item"
   └─ Không nhập title
   └─ Click Save
   └─ Expected: Validation error "Title required"

✅ TC-TASK-16: Delete task
   └─ Open task → Click (...) → Delete
   └─ Expected: Confirmation dialog
   └─ Click Confirm → Task deleted

✅ TC-COMMENT-03: Delete comment
   └─ Hover comment → Click (...) → Delete
   └─ Expected: Comment removed immediately

✅ TC-LIST-02: Sort tasks
   └─ Click "Title" column header
   └─ Expected: Tasks sorted A-Z
   └─ Click again: Z-A
```

#### **Block 3: Performance Check**

```
✅ TC-PERF-01: Load project with many tasks
   └─ Open project with 100+ tasks
   └─ Expected: Load < 3 seconds
   └─ Scrolling smooth (60 FPS)

✅ TC-PERF-02: Open task detail
   └─ Click any task
   └─ Expected: Panel open < 1 second

✅ TC-PERF-03: Search performance
   └─ In cycle view, search "test"
   └─ Expected: Results < 500ms
```

---

### **DAY 5: CROSS-BROWSER & RESPONSIVE (1 giờ)**

```
✅ Desktop (1920x1080)
   └─ Chrome: Full layout
   └─ Firefox: Full layout
   └─ Edge: Full layout

✅ Tablet (768x1024)
   └─ iPad view
   └─ Sidebar collapse/expand
   └─ Touch-friendly buttons

✅ Mobile (375x667)
   └─ iPhone view
   └─ Mobile menu works
   └─ Buttons accessible
```

---

## 🐛 BUG REPORTING FORMAT

### **Template:**
```
🐛 BUG #XXX: [Brief Title]

**Severity:** Critical | High | Medium | Low
**Status:** New | In Progress | Fixed | Verified

📝 **Description:**
[Detailed description]

📍 **Steps to Reproduce:**
1. ...
2. ...
3. ...

❌ **Expected Result:**
[What should happen]

⚠️ **Actual Result:**
[What actually happens]

📷 **Screenshot:** [if applicable]

🔗 **Related:** TC-TASK-05 (priority fix)

---
```

### **Example - Priority Fix Verification:**
```
✅ FIX VERIFIED: Priority Selection Issue

**Title:** Priority dropdown requires F5 refresh - FIXED

**Before Fix:**
1. Open task detail
2. Change priority
3. UI doesn't update
4. Must press F5 to see change

**After Fix:**
1. Open task detail
2. Change priority
3. UI updates IMMEDIATELY ✅
4. No refresh needed ✅

**Handler Added:** selectPriority() function
**File:** TaskDetailModal.vue line 1319
**Status:** ✅ PRODUCTION READY
```

---

## ✅ TEST CHECKLIST - Mark as you complete

### **Smoke Tests**
- [ ] Login successful
- [ ] Dashboard loads
- [ ] Create task
- [ ] View task detail
- [ ] Change priority (NO F5 needed!) ⭐

### **Core Features**
- [ ] Edit title
- [ ] Change status
- [ ] Change priority ✅
- [ ] Assign members
- [ ] Set dates
- [ ] Add labels

### **Board & List**
- [ ] Kanban board display
- [ ] Drag & drop tasks
- [ ] List view display
- [ ] Inline edit

### **Advanced**
- [ ] Create subtask
- [ ] Add comments
- [ ] Create cycle
- [ ] Create module
- [ ] Rich editor (formatting)

### **Quality**
- [ ] No console errors
- [ ] Performance OK
- [ ] Responsive on 3+ devices
- [ ] All fixes verified

---

## 🎯 FINAL SIGN-OFF

When all tests PASS:

```
PROJECT STATUS: ✅ READY FOR PRODUCTION

✅ Smoke test passed
✅ Core features working
✅ Advanced features working
✅ No critical bugs
✅ Performance acceptable
✅ Responsive design OK

🔧 KNOWN FIXES APPLIED:
✅ Priority selection no longer requires F5 (FIXED)

⚠️ KNOWN LIMITATIONS:
- Admin registration/login not tested (out of scope)

📅 SIGN-OFF DATE: __________
👤 TESTER: __________
📞 CONTACT: __________
```

---

**Next Step:** Deploy to production or staging based on test results!

