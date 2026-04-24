# Test case Phase 4 Vong 1

## Muc tieu vong nay

Vong nay chi test `Phase 4.1` truoc:

- work session tracking
- idle timeout
- pause/resume/stop
- log time sinh ra tu session thay vi nhap tay

Khong test AI o vong nay.

---

## Dieu kien truoc khi test

Can co:

- 1 project
- 1 task thuong, khong phai parent task co subtask
- user dang test phai duoc assign vao task do

Nen dung 1 task co:

- estimate > 0
- status chua la Done

---

## Thu tu test de gui lai

Tra ket qua theo mau:

- `TC41: Pass/Fail + ghi chu`
- `TC42: Pass/Fail + ghi chu`
- `TC43: Pass/Fail + ghi chu`
- `TC44: Pass/Fail + ghi chu`
- `TC45: Pass/Fail + ghi chu`

Neu fail thi gui them:

- anh hoac video
- thao tac da lam
- ket qua sai
- ket qua mong doi

---

## TC41: Start session

Buoc test:

1. Mo task detail cua task da assign cho user dang login.
2. O muc `Estimate > Work session`, bam `Start`.

Ket qua mong doi:

- Hien trang thai `Tracking ... h`
- Nut `Pause` va `Stop` hien ra
- Khong can nhap tay so gio van bat dau theo doi duoc

---

## TC42: Pause va Resume

Buoc test:

1. Sau khi Start, doi khoang 10-20 giay.
2. Bam `Pause`.
3. Kiem tra trang thai.
4. Bam `Resume`.

Ket qua mong doi:

- Khi Pause:
  - session dung lai
  - hien `Paused at ... h`
- Khi Resume:
  - session tiep tuc chay
  - quay lai `Tracking ... h`

---

## TC43: Auto pause khi idle

Buoc test:

1. Start session.
2. Khong thao tac tren may trong hon 5 phut.
3. Quay lai task detail.

Ket qua mong doi:

- Session tu dong pause
- Co thong bao session bi pause do idle
- Khong cong gio vo han khi bo may

---

## TC44: Stop de sinh time log

Buoc test:

1. Start session.
2. Doi mot luc ngan.
3. Bam `Stop`.
4. Xem lai `Actual tracked` trong task detail.

Ket qua mong doi:

- Session ket thuc
- He thong tao time log tu dong
- `Actual tracked` tang len
- Khong can nhap tay tong so gio

---

## TC45: Chi assignee moi duoc start

Buoc test:

1. Dang nhap bang user khong duoc assign vao task.
2. Mo task detail cua task do.

Ket qua mong doi:

- Khong duoc start session tracking
- Hien helper text bao chi assigned member moi duoc tracking

---

## Ghi chu nghiep vu tam thoi

Ban dau vong nay dang theo huong:

- session chay o frontend
- duoc luu local de khong mat ngay khi doi task
- khi `Stop` moi tao `TimeLog` that ve backend

Buoc tiep theo sau khi vong nay on:

- them velocity
- them estimate accuracy
- them manager review panel
