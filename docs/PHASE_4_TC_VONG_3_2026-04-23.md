# PHASE 4 - TEST CASE VONG 3

## Muc tieu
Kiem tra phan mo rong cua analytics:
- velocity theo sprint/cycle
- performance estimate theo tung user
- risk summary cho PM/SM/Admin

## Thu tu test

### TC52 - Velocity by sprint hien dung
1. Vao `Analytics` > `Work items`.
2. Tim card `Velocity by sprint`.
3. Neu project co nhieu sprint/cycle thi kiem tra nhieu cot.

Ky vong:
- Hien 3 nhom du lieu:
  - `Committed`
  - `Completed`
  - `Carry-over`
- Ten sprint/cycle hien dung.
- Neu task khong nam trong sprint thi co cot `No cycle`.

### TC53 - Performance theo assignee hien dung
1. Vao `Analytics` > `Work items`.
2. Tim bang `Estimator performance by assignee`.
3. Kiem tra tung cot:
  - `Tasks`
  - `Estimated`
  - `Actual`
  - `Logged`
  - `Accuracy`
  - `Progress`

Ky vong:
- User nao co task assign thi phai hien trong bang.
- So gio va accuracy khong bi 0 neu da co estimate/actual/log.
- Progress trung binh phai thay doi theo data that.

### TC54 - Risk summary hien dung tren Overview
1. Vao `Analytics` > `Overview`.
2. Xem card `Manager review`.
3. Kiem tra cac so:
  - `Over capacity`
  - `Near-limit members`
  - `Carry-over projects`
  - `Unplanned tasks`

Ky vong:
- Neu du lieu da co thi cac chi so hien dung.
- Khong can F5 van load duoc khi vao trang.

### TC55 - Scope/project doi thi sprint va user performance doi theo
1. O `Analytics`, doi project trong combobox tren cung.
2. Neu co, doi giua:
   - `Current project`
   - `All my projects`
3. Quan sat:
   - `Velocity by sprint`
   - `Estimator performance by assignee`
   - `Manager review`

Ky vong:
- Du lieu doi theo scope/project.
- Khong bi giu so lieu cu.
- Khong loi console/network bat thuong.

## Cach bao lai
Gui lai cho minh theo format:

- `TC52: Pass/Fail + ghi chu`
- `TC53: Pass/Fail + ghi chu`
- `TC54: Pass/Fail + ghi chu`
- `TC55: Pass/Fail + ghi chu`
