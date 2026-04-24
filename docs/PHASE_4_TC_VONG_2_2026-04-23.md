# PHASE 4 - TEST CASE VONG 2

## Muc tieu
Kiem tra phan velocity, estimate accuracy, workload va quyen xac nhan planning baseline trong Analytics.

## Luu y truoc khi test
- Dang nhap bang tai khoan co du lieu task, estimate, actual va time log.
- Nen co it nhat 1 project co:
  - task da Done
  - task chua Done
  - task co estimate
  - task co logged hours
- De test quyen xac nhan, nen co 2 tai khoan:
  - 1 tai khoan PM/PO/SM/Admin hoac nguoi co quyen quan tri project
  - 1 tai khoan member/dev thong thuong

## Thu tu test

### TC46 - Analytics Overview tai du lieu planning that
1. Vao `Analytics`.
2. O tab `Overview`, xem cac o:
   - `Committed story points`
   - `Completion rate`
   - `Estimated hours`
   - `Actual hours`
   - `Logged hours`
3. Doi project trong combobox neu can.

Ky vong:
- Du lieu hien ra khong bi trong neu project da co task/estimate/log.
- So lieu thay doi theo project dang chon.
- Khong can F5 van load duoc.

### TC47 - Velocity by project hien dung
1. Vao tab `Work items`.
2. Xem bieu do `Velocity by project`.
3. Neu co nhieu project, doi scope giua:
   - `Current project`
   - `All my projects`

Ky vong:
- Bieu do hien duoc story point committed va completed.
- Khi doi scope, du lieu thay doi dung.
- Khong bao loi console/network bat thuong.

### TC48 - Estimate accuracy buckets tinh dung
1. O tab `Work items`, de dropdown `by Accuracy`.
2. Kiem tra cac chi so:
   - `Average accuracy`
   - `Accurate`
   - `Under-estimated`
   - `Over-estimated`
   - `Unplanned`
3. Xem bang `Top estimate accuracy gaps`.

Ky vong:
- Cac task co estimate/actual/logged duoc phan bucket.
- Bang ben duoi hien task, estimated, actual, logged, accuracy, bucket.
- Khong bi tat ca ve 0 neu du lieu thuc da co.

### TC49 - Workload chart hien dung estimated va actual
1. O tab `Work items`, doi dropdown sang `by Workload`.
2. Quan sat chart workload.

Ky vong:
- Hien duoc estimated hours va actual hours theo user.
- Neu user nao co du lieu estimate/log time thi chart phai co cot.
- Khong duoc hien trang neu da co du lieu.

### TC50 - PM/PO/SM/Admin duoc xac nhan baseline
1. Dang nhap bang tai khoan co quyen.
2. Vao `Analytics` > `Overview`.
3. Tim card `Manager review`.
4. Bam `Confirm baseline`.

Ky vong:
- Co nhin thay nut `Confirm baseline`.
- Bam thanh cong thi hien thong bao thanh cong.
- Du lieu manager review duoc load lai ngay sau khi confirm.

### TC51 - Member/Developer khong duoc xac nhan baseline
1. Dang nhap bang tai khoan member/dev thuong.
2. Vao `Analytics` > `Overview`.
3. Xem card `Manager review`.

Ky vong:
- Khong thay nut `Confirm baseline`.
- Neu chi co quyen xem thi card hien `View only`.

## Cach bao lai ket qua
Gui cho minh theo dung format:

- `TC46: Pass/Fail + ghi chu`
- `TC47: Pass/Fail + ghi chu`
- `TC48: Pass/Fail + ghi chu`
- `TC49: Pass/Fail + ghi chu`
- `TC50: Pass/Fail + ghi chu`
- `TC51: Pass/Fail + ghi chu`
