# Bao cao test Phase 1-2-3 Estimate Reward

## Pham vi

Dot nay bao gom:

- Phase 1: chot nghiep vu estimate/reward de test duoc tren giao dien
- Phase 2: backend core cho base/share/bonus/penalty
- Phase 3: hien thi du lieu tren task detail va rewards de test tay

File da sua trong dot nay:

- `Backend/src/TaskManagement.Infrastructure/Data/ApplicationDbContext.cs`
- `Backend/src/TaskManagement.Infrastructure/Services/GamificationService.cs`
- `Backend/src/TaskManagement.API/Controllers/GamificationController.cs`
- `Frontend/src/components/TaskDetailModal.vue`
- `Frontend/src/views/RewardsView.vue`

## Da thay doi gi

1. `Actual hours` cua task duoc cong tu `TimeLogs`.
2. `Actual hours` cua tung assignee duoc cong tu `TimeLogs` theo `task + user`.
3. Task la task la co the lay `estimate` tu tong estimate cua assignee.
4. Task cha se roll-up `estimate` va `actual hours` tu cac sub-work item.
5. Reward logic hien tai theo huong:
   - `base points = do kho x thoi gian chuan`
   - chia diem theo estimate cua assignee truoc
   - neu khong co thi fallback sang contribution weight
   - co bonus som
   - co bonus estimate sat thuc te
   - co penalty neu tre han
6. Trong task detail da hien thi:
   - story points
   - estimate
   - actual tracked hours
   - estimate chia theo assignee
   - actual hours cua tung assignee
   - ti le share cua tung assignee
   - canh bao estimate duoc suy ra tu sub-work item neu la task cha
7. Trang Rewards da co:
   - base points
   - bonus points
   - penalty points
   - cong thuc moi de test

## Du lieu de test

Nen dung 1 project co it nhat:

- 2 thanh vien
- 1 parent task
- 2 sub-work item nam trong parent task
- due date la hom nay hoac ngay tuong lai

Du lieu mau de test:

- Parent task: `Implement payment flow`
- Subtask A: `Build payment API`
- Subtask B: `Build payment UI`
- Assignee 1 estimate: `12h`
- Assignee 2 estimate: `8h`

## Test case Phase 1

### TC01: Task detail hien thi estimate, actual, share

Buoc test:

1. Mo task detail.
2. Gan 2 thanh vien vao task.
3. Dat estimate cua 2 assignee khac nhau.

Ket qua mong doi:

- O Estimate hien tong estimate.
- Co phan chia estimate theo assignee.
- Moi dong assignee hien:
  - estimated hours
  - actual hours
  - share percent

### TC02: Story points hien thi va sua duoc

Buoc test:

1. Mo task detail.
2. Doi `Story points`.

Ket qua mong doi:

- Co field story points.
- Sua duoc.
- Gia tri khong vuot gioi han UI cho phep.

### TC03: Parent estimate tro thanh estimate suy ra tu sub-work item

Buoc test:

1. Mo task detail cua task cha.
2. Dam bao task cha co sub-work item va cac subtask da co estimate.

Ket qua mong doi:

- O estimate hien phan roll-up tu sub-work item.
- Input estimate cua task cha bi disable.
- Co helper text bao estimate nay duoc suy ra tu sub-work item.
- Tong roll-up bang tong estimate cua cac subtask.

## Test case Phase 2

### TC04: Actual hours cua assignee duoc cong tu time logs

Buoc test:

1. Log time tren cung mot task cho assignee 1.
2. Log time them lan nua cho assignee 1.
3. Mo lai task detail hoac refresh du lieu task.

Ket qua mong doi:

- Actual hours cua assignee = tong cac time log cua user do tren task do.

### TC05: Leaf task estimate di theo estimate cua assignee

Buoc test:

1. Tren 1 leaf task, set assignee A = `12h`.
2. Set assignee B = `8h`.

Ket qua mong doi:

- Tong estimate cua task = `20h`.

### TC06: Parent task roll-up estimate va actual tu subtask

Buoc test:

1. Cho subtask A co estimate va time logs.
2. Cho subtask B co estimate va time logs.
3. Mo parent task.

Ket qua mong doi:

- Parent estimate = tong estimate cua subtask.
- Parent actual hours = tong actual hours cua subtask.

### TC07: Rewards sinh ra base/share/bonus/penalty

Buoc test:

1. Hoan thanh 1 task co estimate va due date.
2. Neu duoc thi hoan thanh som hon hoac dung han.
3. Dam bao actual hours gan voi estimate.

Ket qua mong doi:

- Co transaction reward.
- Phan summary co base points.
- Neu xong dung han hoac som thi co early bonus.
- Neu actual gan estimate thi co accuracy bonus.
- Neu xong tre han thi moi co late penalty.

## Test case Phase 3

### TC08: Trang Rewards hien summary day du

Buoc test:

1. Mo trang `Rewards`.

Ket qua mong doi:

- Summary hien:
  - completed tasks
  - early bonuses
  - base points
  - bonus points
  - penalty points
  - contribution share
  - estimated hours
  - actual hours
  - logged hours
  - rollback points

### TC09: Cong thuc Rewards da doi theo nghiep vu moi

Buoc test:

1. Mo trang `Rewards`.

Ket qua mong doi:

- Formula hien cac thong tin:
  - difficulty
  - duration
  - share
  - final points

### TC10: Nut refresh Rewards van hoat dong

Buoc test:

1. Tao them thay doi reward bang cach complete hoac update task.
2. Mo trang `Rewards`.
3. Bam `Refresh`.

Ket qua mong doi:

- Du lieu duoc refresh.
- Giao dien khong vo.

## Cach gui ket qua test lai cho toi

Ban tra ket qua theo mau:

- `TC01: Pass/Fail + ghi chu`
- `TC02: Pass/Fail + ghi chu`
- `TC03: Pass/Fail + ghi chu`
- `TC04: Pass/Fail + ghi chu`
- `TC05: Pass/Fail + ghi chu`
- `TC06: Pass/Fail + ghi chu`
- `TC07: Pass/Fail + ghi chu`
- `TC08: Pass/Fail + ghi chu`
- `TC09: Pass/Fail + ghi chu`
- `TC10: Pass/Fail + ghi chu`

Neu co case fail, vui long gui them:

- anh hoac video
- thao tac da lam
- ket qua sai dang gap
- ket qua dung mong doi

## Xac nhan build

Trang thai build sau dot nay:

- `dotnet build Backend/src/TaskManagement.API/TaskManagement.API.csproj`: passed
- `npm run build` trong `Frontend`: passed
