# Bao Cao Gap Phase 3-4-5 Va Toan Du An

Ngay cap nhat: 2026-04-22

## 1. Tong ket nhanh

Trang thai hien tai sau pass nay:

- Phase 3: da khop hon o project settings, cycles, carry-over, va integrations project-level
- Phase 4: da khop hon o role-based admin experience, co route rieng cho role management
- Phase 5: dat muc code/build handoff tot de user vao test truc tiep

Build:

- Frontend: `npm run build` PASS
- Backend: `dotnet build Backend/src/TaskManagement.API/TaskManagement.API.csproj` PASS

## 2. Nhung gi da duoc khac phuc trong pass nay

### 2.1. Integrations project-level

Da bo scaffold va noi thanh du lieu that:

- backend co `GET/PUT /api/projects/{projectId}/integrations`
- luu cau hinh theo tung project trong `SystemSettings`
- `Project Settings > Integrations` da co form that cho:
  - `GitHub`
  - `Jira`
  - `Slack`
- co cac truong:
  - `enabled`
  - `endpoint`
  - `project key`
  - `secret / webhook`
  - `notes`

Ket qua:

- integrations khong con la tab placeholder nua
- cau hinh da duoc tach khoi system admin va luu theo tung project

### 2.2. Role Management da co man rieng

Da tach `Role Management` thanh route/man rieng:

- route moi: `/admin/roles`
- PM va nhom system-admin deu vao duoc theo rule hien tai
- `AdminSidebar` va `SettingsDropdown` da co entry rieng cho `Role Management`
- `User Management` van giu kha nang role assignment, dong thoi co nut mo nhanh sang man role rieng

Ket qua:

- khong con gap "role management dang nam chung va chua co man rieng"

### 2.3. Estimate -> reward -> actual minh bach hon

Da mo rong backend/frontend rewards theo huong cong bang hon:

- rewards da doc them:
  - `estimated hours`
  - `actual hours`
  - `logged hours`
- cong thuc hien thi da doi sang:
  - `Base effort x Efficiency x Quality x Contribution share`
- spotlight tasks da hien them:
  - estimate theo nguoi
  - actual theo nguoi
  - efficiency modifier
  - quality modifier
  - fair points
- policy duoc hien ro hon cho:
  - multi-assignee
  - carry-over
  - reopened / rollback
  - actual-hours fallback

Ket qua:

- khu `Rewards` khong con chi la bang diem tong don gian
- da co nen tang minh bach hon de doi chieu estimate, actual, contribution share

### 2.4. Warning backend

Da don xong warning backend:

- `AuditLogsController.cs`
- `StickiesController.cs`
- warning nullability moi phat sinh tu `GamificationController.cs`
- warning nullability moi phat sinh tu `ProjectsController.cs`

Ket qua:

- backend build hien tai: `0 warning`

## 3. Danh gia theo tung phase

### 3.1. Phase 3

Trang thai:

- `Project Settings` da la trang project-level that
- `Labels`, `Modules`, `Cycles` da co management sau hon
- `Integrations` da co backend + UI that
- carry-over planner van giu duoc flow tu cycle complete sang backlog / cycle moi

Ket luan:

- Phase 3 da khop o muc code/build

### 3.2. Phase 4

Trang thai:

- `Admin` va `Project Settings` tach scope ro hon
- `PM` khong bi day vao toan bo system admin
- `Role Management` da co route/man rieng
- menu/sidebar/dropdown da dong bo hon theo quyen that

Ket luan:

- Phase 4 da khop tot hon o ca permission va information architecture

### 3.3. Phase 5

Trang thai:

- frontend/backend deu build pass
- cac flow moi da san sang de user test truc tiep
- report nay khong coi browser/manual la blocker vi user se tu test

Can user test truc tiep cho cac flow:

- `Project Settings > Integrations`
- `/admin/roles`
- assign role cho user
- reward summary + spotlight task formula
- cycle carry-over planner

Ket luan:

- Phase 5 dang o trang thai handoff tot

## 4. Gap toan du an tu nang den nhe

### High

- Khong con gap high nao trong pham vi report cu da neu

### Medium

1. `Experience-based estimate` van chua la engine that

Trang thai hien tai:

- he thong da co estimate tong
- chia theo multi-assignee
- roll-up tu subtasks
- rewards da doc duoc estimate / actual / logged hours

Nhung neu muon tinh estimate theo trinh do user that su thi van can:

- `skill profile`
- `historical delivery profile`
- `task taxonomy`
- `difficulty signals`
- `recommendation engine`

Danh gia:

- day la huong mo rong tiep theo, khong con la blocker cho Phase 3-4-5 vua khac phuc

### Low

1. Frontend van con warning build do `:deep` cu o mot so file chua nam trong pham vi patch nay

Tac dong:

- khong chan build
- la debt tooling/CSS syntax, khong phai blocker nghiep vu

2. Frontend van con warning `chunk size`

Tac dong:

- build van pass
- can toi uu code-splitting sau neu muon giam kich thuoc bundle lon

## 5. Danh gia hien tai ve estimate va cham cong

Huong hien tai da co:

1. `Estimate`

- planning effort tong
- chia theo assignee
- roll-up tu subtasks

2. `Actual`

- lay tu `time log` neu co
- fallback sang `assignment actual hours`

3. `Reward preview`

- base effort
- efficiency modifier
- quality modifier
- contribution share

Ket luan:

- da co nen tang that de di tiep sang cong thuc diem cong/cham cong cong bang hon
- chua nen goi day la engine HR/payroll hoan chinh

## 6. Neu muon tinh theo trinh do user thi can phat trien them gi

Khong nen suy doan trinh do chi tu role hoac title nguoi dung. Can xay them:

1. `Skill profile`

- ky nang theo domain
- muc thanh thao theo thang do ro rang

2. `Historical delivery profile`

- estimate trung binh
- actual trung binh
- ty le dung deadline
- ty le reopen

3. `Task taxonomy`

- bug
- feature
- refactor
- integration
- migration
- docs

4. `Difficulty signals`

- story points
- dependency count
- module/domain
- so assignee
- block count
- carry-over count

5. `Recommendation engine`

- so sanh task moi voi task lich su tuong tu
- dua ra estimate theo team va theo user
- tra kem ly do de quan ly co the override

## 7. De xuat thu tu tiep theo

1. User test truc tiep cac flow moi vua dong
2. Dọn nốt toan bo `:deep` cu de frontend build sach warning hon
3. Toi uu chunk size cho cac route lon nhu `Pages`, `SpaceSummary`, `ApexCharts`, `vue-router`
4. Neu muon di sau hon, bat dau xay tang du lieu cho `experience-based estimate`

## 8. Ket luan

Tinh den pass nay:

- cac gap chinh trong report cu da duoc khac phuc o muc code/build:
  - integrations project-level
  - role management route rieng
  - reward/estimate minh bach hon
  - backend warning = 0
- phan con lai chu yeu la debt frontend tooling/performance nhe va huong mo rong estimate theo trinh do user
