# Phase 6 - Vong 3 - TC AI goi y assignee

## Muc tieu

Kiem tra AI co the goi y nguoi phu hop cho task/subtask dua tren velocity, accuracy, workload va lich su lam viec trong project, sau do PM/PO/SM/Admin co the ap dung nhanh vao task.

## Cach test

### TC63A - Goi y assignee cho task thuong
- Mo 1 task da co estimate hoac story point.
- Bam `AI suggest assignees`.
- Ky vong:
  - Hien panel goi y.
  - Co `recommended team size`.
  - Co danh sach 1-3 nguoi de xuat.

### TC63B - Hien metric hop ly
- Xem tung nguoi trong panel goi y.
- Ky vong:
  - Co `fit %`
  - Co `project role`
  - Co `pts done`
  - Co `accuracy %`
  - Co `active h`

### TC63C - Apply top suggestion
- Bam `Apply top suggestion`.
- Ky vong:
  - Task duoc assign nguoi top 1.
  - UI assignee cap nhat ngay.
  - Khong can F5.

### TC63D - Apply suggested team
- Bam `Apply suggested team`.
- Ky vong:
  - Task duoc assign dung so nguoi theo `recommended team size`.
  - Contribution weight / estimated hours cua tung nguoi duoc cap nhat.

### TC63E - Khong pha estimate cu
- Neu task da co estimate tong truoc do, sau khi apply team:
- Ky vong:
  - Tong estimate van hop ly.
  - Neu task khong phai parent-derived thi estimate tong khong bi vo.

### TC63F - Phan quyen
- Dang nhap bang role khong co quyen PM/PO/SM/Admin theo project va khong phai system admin.
- Goi API suggest assignee.
- Ky vong:
  - Bi chan quyen hoac khong cho thao tac.

### TC63G - Fallback on du lieu that
- Neu Gemini cham hoac khong san sang, thu bam goi y.
- Ky vong:
  - Van co ket qua goi y dua tren du lieu that trong project.
  - Khong fail day chuyen vi Gemini.
