# Bao cao Phase 4-5: Velocity, Accuracy va AI

## Muc tieu chung

Sau khi Phase 1-2-3 da co:

- estimate
- actual hours
- time logs
- rewards

thi Phase 4-5 se bien du lieu nay thanh:

- quan tri thuc chien
- du bao nang luc giao viec
- goi y estimate thong minh
- AI ho tro phan ra task

---

## Phase 4 lam gi

### Muc tieu

Phase 4 tap trung vao:

- velocity
- estimate accuracy
- time tracking nghiep vu dung
- dashboard de PM/SM/nguoi quan tri du an co the chot va dieu phoi cong viec

### Gia tri kinh doanh

Lam xong Phase 4 se co loi ich:

- PM nhin duoc sprint sau nen nhan bao nhieu tai
- SM nhin duoc user nao dang qua tai, user nao con slot
- nguoi quan tri du an nhin duoc ai estimate sat, ai hay lech
- giam gian lan do actual/logged hours khong con la so nhap tay tuy y
- reward, velocity, workload va planning dung cung 1 nguon du lieu

### Chot them nghiep vu

Phase 4 se bo sung:

- bien du lieu do thanh quan tri thuc chien bang velocity va accuracy
- PM hoac nguoi co quyen quan tri du an duoc xem va chot
- SM cung duoc xem/chot neu duoc gan role phu hop trong du an vi SM la nguoi theo doi va dieu phoi cong viec cho nhom

### Ai duoc xem va chot

Quyen de xai man hinh Phase 4:

- `System Admin`: full quyen
- `PM` trong project: duoc xem va chot
- `PO` trong project: duoc xem, co the duoc chot neu nghiep vu cho phep
- `SM` trong project: duoc xem va chot
- `Project Admin` hoac nguoi co project permission tuong duong: duoc xem va chot
- `Member/Developer`: chi xem phan ca nhan cua minh, khong duoc chot

### Nghiep vu can lam trong Phase 4

1. `Velocity`
- Velocity theo sprint
- Velocity theo team
- Velocity theo user
- So sanh:
  - committed story points
  - completed story points
  - carry-over story points

2. `Estimate Accuracy`
- So sanh:
  - estimated hours
  - actual hours
  - logged hours
- Xep hang:
  - accurate
  - under-estimated
  - over-estimated

3. `Time Tracking dung nghiep vu`
- Khong cho sua truc tiep `actual hours` tong
- Gio thuc te phai den tu:
  - start
  - pause
  - resume
  - stop
  - done
  - idle timeout
- Co xu ly truong hop quen stop

4. `Planning support`
- Goi y:
  - user nao hop de nhan task
  - sprint sau co qua tai khong
  - task nao can tach nho

5. `Review va chot`
- PM/SM/nguoi quan tri project co the:
  - xem dashboard sprint
  - xem estimate accuracy
  - chot workload
  - xac nhan ke hoach sprint tiep theo

### UI can co o Phase 4

1. `Project Analytics / Planning tab`
- Team velocity
- Sprint velocity
- Estimate accuracy
- Workload by assignee
- Capacity gan dung

2. `Task detail`
- Start/Pause/Resume/Stop time tracking
- Session dang chay
- Idle timeout status
- Auto-log hoac de xuat log

3. `Manager review panel`
- danh sach task lech estimate nhieu
- danh sach user over-capacity
- sprint health
- nut `Confirm plan` hoac `Lock sprint estimate baseline`

### Backend/API can co o Phase 4

Can bo sung:

- API lay velocity theo sprint/project/user
- API lay estimate accuracy theo sprint/project/user
- API tracking work session
- API pause/resume/stop session
- API manager confirm planning baseline

### File du kien se sua trong Phase 4

Backend:

- `Backend/src/TaskManagement.Domain/Entities/WorkTask.cs`
- `Backend/src/TaskManagement.Domain/Entities/TaskAssignment.cs`
- `Backend/src/TaskManagement.Domain/Entities/TimeLog.cs`
- `Backend/src/TaskManagement.Infrastructure/Data/ApplicationDbContext.cs`
- `Backend/src/TaskManagement.Infrastructure/Services/GamificationService.cs`
- `Backend/src/TaskManagement.Infrastructure/Services/WorkTaskService.cs`
- `Backend/src/TaskManagement.API/Controllers/GamificationController.cs`
- `Backend/src/TaskManagement.API/Controllers/WorkTasksController.cs`
- can them moi:
  - `Backend/src/TaskManagement.Domain/Entities/WorkSession.cs`
  - `Backend/src/TaskManagement.API/Controllers/VelocityController.cs`
  - `Backend/src/TaskManagement.Infrastructure/Services/VelocityService.cs`

Frontend:

- `Frontend/src/components/TaskDetailModal.vue`
- `Frontend/src/views/RewardsView.vue`
- `Frontend/src/views/GlobalAnalyticsView.vue`
- `Frontend/src/views/SpaceSummary.vue`
- co the them moi:
  - `Frontend/src/views/ProjectVelocityView.vue`
  - `Frontend/src/components/WorkSessionTracker.vue`
  - `Frontend/src/store/useVelocityStore.js`

### Test case Phase 4

`TC41`
- Start task, co thao tac, pause, stop
- Ket qua mong doi:
  - sinh log dung
  - actual hours tang dung

`TC42`
- Start task roi bo may, khong stop
- Ket qua mong doi:
  - session auto pause khi idle
  - khong cong gio vo han

`TC43`
- User hoan thanh nhieu task trong sprint
- Ket qua mong doi:
  - velocity sprint cap nhat dung

`TC44`
- Compare estimated va actual
- Ket qua mong doi:
  - accuracy hien dung theo user va task

`TC45`
- Dang nhap bang PM/SM/project admin
- Ket qua mong doi:
  - xem duoc dashboard
  - chot duoc baseline/planning

`TC46`
- Dang nhap bang member thuong
- Ket qua mong doi:
  - khong thay nut chot
  - chi thay du lieu ca nhan neu duoc phep

---

## Phase 5 lam gi

### Muc tieu

Phase 5 la AI support:

- phan ra task
- goi y estimate
- goi y story points
- goi y phan chia effort
- phan tich repo GitHub

### Gia tri kinh doanh

Lam xong Phase 5 se co:

- giam thoi gian PM/SM phai phan ra task thu cong
- giam estimate cam tinh
- AI ho tro planning sprint
- du de chat va hoi dap nghiep vu ngay trong he thong

### Nghiep vu Phase 5

1. `AI breakdown`
- Nhin task cha
- Tao danh sach subtask
- Moi subtask co:
  - title
  - mo ta
  - story point goi y
  - estimated hours goi y

2. `AI estimate suggestion`
- Dua vao:
  - story points
  - priority
  - lich su task tuong tu
  - velocity user/team
  - repo context

3. `AI repo analysis`
- Doc repo GitHub
- Hieu:
  - module
  - feature
  - folder
  - dependency
- Tu do:
  - de xuat task
  - de xuat subtask
  - de xuat estimate

4. `AI chat assistant`
- Hoi dap:
  - tai sao task nay estimate cao
  - nen giao cho ai
  - sprint nay co qua tai khong
  - task cha nen tach the nao

### Pham vi AI cua Gemini 2.5 Flash

Model hien tai co the lam tot:

- doc prompt va mo ta task
- de xuat subtask
- tom tat repo o muc tot
- dua ra estimate suggestion ban dau
- giai thich ly do estimate
- chat voi nguoi dung

Model hien tai khong nen lam 100% tu dong ngay:

- tu dong save estimate khong qua duyet
- tu dong giao task khong co nguoi xac nhan
- tu dong ghi de story points cua PM/SM

### UI can co o Phase 5

1. `AI panel trong task detail`
- Generate subtasks
- Suggest estimate
- Suggest story points
- Suggest assignee split

2. `AI repo panel`
- Nhap GitHub URL
- Xem repo summary
- Generate work breakdown

3. `AI planning assistant`
- Hoi dap planning
- de xuat sprint loading

### Backend/API can co o Phase 5

- API analyze GitHub repo
- API generate subtask suggestions
- API suggest estimate/story points
- API conversational AI assistant

### File du kien se sua trong Phase 5

Backend:

- `Backend/src/TaskManagement.Infrastructure/Services/GeminiAiService.cs`
- `Backend/src/TaskManagement.API/Controllers/AiController.cs`
- `Backend/src/TaskManagement.Infrastructure/Services/WorkTaskService.cs`
- co the them moi:
  - `Backend/src/TaskManagement.Application/Interfaces/IAiPlanningService.cs`
  - `Backend/src/TaskManagement.Infrastructure/Services/AiPlanningService.cs`

Frontend:

- `Frontend/src/views/AIPage.vue`
- `Frontend/src/components/TaskDetailModal.vue`
- co the them moi:
  - `Frontend/src/components/AIEstimatePanel.vue`
  - `Frontend/src/components/AIBreakdownPanel.vue`
  - `Frontend/src/store/useAiPlanningStore.js`

### Test case Phase 5

`TC51`
- Nhap task cha
- Bam generate subtasks
- Ket qua mong doi:
  - AI tra ve danh sach subtask hop ly

`TC52`
- Bam suggest estimate
- Ket qua mong doi:
  - co story points va estimated hours goi y
  - co ly do giai thich

`TC53`
- Nhap GitHub repo
- Ket qua mong doi:
  - AI doc duoc context repo
  - tao de xuat task

`TC54`
- PM/SM hoi assistant
- Ket qua mong doi:
  - AI tra loi theo du lieu planning va velocity

---

## Thu tu uu tien khuyen nghi

Lam theo thu tu:

1. Phase 4.1
- work session tracking
- idle timeout
- auto-pause

2. Phase 4.2
- velocity
- estimate accuracy
- manager dashboard

3. Phase 4.3
- manager confirm/chot baseline

4. Phase 5.1
- AI subtask generation
- AI estimate suggestion

5. Phase 5.2
- AI repo analysis
- AI planning assistant

---

## Chot scope

### Phase 4 chot

- focus vao nghiep vu that
- tracking dung
- velocity
- estimate accuracy
- PM/SM/project manager duoc xem va chot

### Phase 5 chot

- AI goi y
- AI phan ra
- AI phan tich repo
- AI chat planning

Neu ban dong y, buoc tiep theo se la:

- tao file test case rieng cho Phase 4
- sau do bat dau lam Phase 4 truoc
