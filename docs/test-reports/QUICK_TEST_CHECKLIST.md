# ✅ QUICK TEST CHECKLIST

**Ngày test:** 2026-04-18  
**Tester:** QA Team  
**Status:** Pending

---

## 🚀 QUICK START (5 phút)

### Setup
```bash
# Terminal 1: Frontend
cd Frontend && npm run dev  # http://localhost:5173

# Terminal 2: Backend
cd Backend/src/TaskManagement.API && dotnet run  # http://localhost:5136

# Then: Login with test@example.com / Test@123
```

---

## ⭐ CRITICAL FIX VERIFICATION (Priority Feature)

**MAIN ISSUE FIXED:**
- ✅ Priority dropdown now updates UI immediately
- ✅ NO F5 refresh needed anymore
- ✅ Works in task detail panel

**Test Steps:**
```
1. Open task detail
2. Click Priority dropdown
3. Select "High" (orange chevron up)
   ✓ Icon should change IMMEDIATELY
   ✓ Text should change IMMEDIATELY
4. Select "Urgent" (red double up)
   ✓ Should update INSTANTLY
5. Close detail and reopen
   ✓ Priority should persist correctly
```

**Status:** ✅ **FIXED & READY**

---

## 📋 QUICK TEST MATRIX (30 items)

| # | Feature | Steps | Expected | Status |
|---|---------|-------|----------|--------|
| 1 | Login | Email + Password | Dashboard | ☐ |
| 2 | Create Task | New Item + Title | Task created | ☐ |
| 3 | View Detail | Click task | Panel opens | ☐ |
| 4 | Edit Title | Click title, edit | Save no F5 | ☐ |
| 5 | Change Status | Dropdown | Update instant | ☐ |
| **6** | **Change Priority** ⭐ | **Dropdown** | **Instant, no F5** | ☐ |
| 7 | Add Assignee | Assignees dropdown | Member assigned | ☐ |
| 8 | Set Start Date | Date picker | Date saved | ☐ |
| 9 | Set Due Date | Date picker | Date saved | ☐ |
| 10 | Add Labels | Labels dropdown | Label attached | ☐ |
| 11 | Assign Cycle | Cycle dropdown | Cycle assigned | ☐ |
| 12 | Assign Module | Module dropdown | Module assigned | ☐ |
| 13 | Edit Description | Rich editor | Formatting saved | ☐ |
| 14 | Create Subtask | "Add sub-work" | Subtask created | ☐ |
| 15 | Kanban View | Click "Board" | Columns display | ☐ |
| 16 | Drag Task | Drag card | Task moves | ☐ |
| 17 | List View | Click "List" | Table displays | ☐ |
| 18 | Add Comment | Input + Submit | Comment shows | ☐ |
| 19 | Edit Comment | Click (...) Edit | Comment updated | ☐ |
| 20 | Delete Comment | Click (...) Delete | Comment removed | ☐ |
| 21 | Emoji React | Emoji icon | Reaction added | ☐ |
| 22 | Create Cycle | "New Cycle" | Cycle created | ☐ |
| 23 | Create Module | "New Module" | Module created | ☐ |
| 24 | Create Label | Labels setting | Label created | ☐ |
| 25 | Analytics | "Analytics" tab | Stats display | ☐ |
| 26 | Filter View | Filter icon | Tasks filtered | ☐ |
| 27 | Delete Task | (...) Delete | Task removed | ☐ |
| 28 | Attach File | "Attach" button | File uploaded | ☐ |
| 29 | Responsive (Mobile) | 375px width | Layout adapts | ☐ |
| 30 | Performance | Load project | < 3 seconds | ☐ |

**Target:** 30/30 ✅

---

## 🔴 CRITICAL TESTS (Must Pass)

```
❌ → STOP, Report Bug
✅ → Continue

☐ Login successful? ✅/❌
☐ Create task? ✅/❌
☐ View task detail? ✅/❌
☐ Priority change NO F5? ✅/❌ ⭐⭐⭐
☐ Status change works? ✅/❌
☐ Board view loads? ✅/❌
☐ List view loads? ✅/❌
```

---

## 🟡 HIGH PRIORITY TESTS

```
☐ Edit title saves
☐ Add assignee
☐ Set dates
☐ Add labels
☐ Create subtask
☐ Add comment
☐ Create cycle
☐ Kanban drag & drop
```

---

## 🟢 MEDIUM PRIORITY TESTS

```
☐ Edit comment
☐ Delete comment
☐ Emoji reaction
☐ Rich text formatting
☐ Filter tasks
☐ Analytics view
☐ Attach files
☐ Create module
```

---

## 📱 BROWSER/DEVICE TESTS

### Desktop
- [ ] Chrome (latest)
- [ ] Firefox (latest)
- [ ] Edge (latest)

### Responsive
- [ ] Tablet (768px)
- [ ] Mobile (375px)
- [ ] Touch friendly?

---

## 🐛 BUGS FOUND

| ID | Issue | Severity | Status | Notes |
|----|-------|----------|--------|-------|
| BUG-001 | [Title] | High | New | [Details] |
| | | | | |
| | | | | |

---

## 📊 TEST SUMMARY

```
Total Tests: 30
Passed:  ___ / 30
Failed:  ___ / 30
Blocked: ___ / 30

Pass Rate: ___%

Critical Bugs: ___
High Bugs: ___
Medium Bugs: ___

Status: 
☐ READY FOR PROD
☐ NEEDS FIXES
☐ BLOCKED
```

---

## 🎯 DAILY TARGETS

**Monday (TODAY):**
- [ ] Smoke test (5 items) ✅
- [ ] Priority fix verification ⭐ ✅
- [ ] Core features (items 1-10) ✅

**Tuesday:**
- [ ] Board & List views (items 15-17) 
- [ ] Comments & Reactions (items 18-21)
- [ ] Cycles & Modules (items 22-23)

**Wednesday:**
- [ ] Advanced features (items 24-28)
- [ ] Analytics & Filtering
- [ ] Delete & Attach

**Thursday:**
- [ ] Cross-browser tests
- [ ] Responsive tests
- [ ] Performance checks

**Friday:**
- [ ] Regression test (retest all fixed items)
- [ ] Final sign-off
- [ ] Production readiness

---

## 👤 SIGN-OFF

```
Date: __________
Tester: __________
Status: ☐ PASS ☐ FAIL
Notes: _______________
```

---

**Questions?** Check TEST_PLAN.md or TEST_EXECUTION_GUIDE.md

