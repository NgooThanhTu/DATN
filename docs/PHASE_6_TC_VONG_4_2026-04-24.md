# Phase 6 - Vong 4 - TC AI ho tro PM/SM chot backlog va cycle

Muc tieu vong nay:

- Kiem tra `Dot 6.4` da bien repo analysis thanh mot khu `operational review`
- PM/PO/SM/Admin theo project co the:
  - xem tong backlog AI duoc chon
  - xem tong estimate
  - xem so risky items
  - xem `test plan`
  - dua task vao `backlog` hoac `cycle` da chon
- Member/dev van chi duoc xem va chat, khong duoc chot backlog AI

---

## TC64A - Operational review hien du thong tin tong hop

### Buoc test

1. Dang nhap bang user co quyen `PM/PO/SM/Admin` theo project dang mo.
2. Vao `AI Assistant`.
3. Nhap repo GitHub va bam `Analyze repo`.
4. Sau khi co ket qua, quan sat khu `Operational review`.

### Ket qua mong doi

- Co khu `Operational review`.
- Co cac thong tin:
  - `Selected`
  - `Estimate`
  - `Risky`
- Co danh sach:
  - `Quick wins`
  - `Medium tasks`
  - `Risky tasks`
- Co khu `Test plan`.

---

## TC64B - Co the bo chon tung AI backlog item

### Buoc test

1. O khu `Operational review`, bo tick 1 vai item.
2. Kiem tra so `Selected` va `Estimate`.

### Ket qua mong doi

- So item duoc chon giam dung.
- Tong estimate thay doi dung theo item dang chon.
- Bam `Select all AI backlog items` thi chon lai toan bo.

---

## TC64C - Tao selected backlog vao backlog

### Buoc test

1. O `Target cycle`, de mac dinh la `Backlog`.
2. Chon mot vai item.
3. Bam `Create selected to backlog`.

### Ket qua mong doi

- He thong tao dung so task da chon.
- Task moi xuat hien trong project.
- Task moi nam o `backlog` (khong co cycle).
- Khong can F5 van thay du lieu moi.

---

## TC64D - Tao selected backlog vao cycle da chon

### Buoc test

1. Dam bao project dang co it nhat 1 cycle chua completed.
2. O `Target cycle`, chon 1 cycle.
3. Chon mot vai item AI.
4. Bam `Create selected to <ten cycle>`.

### Ket qua mong doi

- He thong tao task thanh cong.
- Task moi duoc gan vao cycle da chon.
- Qua work list / cycle view co the nhin thay task nam dung cycle do.

---

## TC64E - Phan quyen theo project dang mo

### Buoc test

1. Dang nhap bang user chi la `member/dev` trong project dang mo.
2. Vao `AI Assistant`.
3. Phan tich repo.
4. Kiem tra khu `Operational review` va cac nut tao backlog AI.

### Ket qua mong doi

- User van co the chat / xem ket qua repo analysis.
- User khong duoc chot backlog AI.
- Khu review tao task khong hien hoac cac nut tao bi chan.
- Khong tao duoc task moi vao project.

---

## TC64F - Chon cycle sai project phai bi chan

### Buoc test

1. Goi tao selected backlog AI khi `Target cycle` la 1 cycle khong thuoc project hien tai.
2. Hoac test qua devtools / request tay neu can.

### Ket qua mong doi

- Backend chan.
- Bao loi ro rang:
  - `Cycle dich khong thuoc du an nay.`
- Khong tao task moi.

---

## TC64G - Dong bo sau khi tao

### Buoc test

1. Tao selected backlog AI vao backlog hoac cycle.
2. Quay lai project view / work list / cycle view.

### Ket qua mong doi

- Du lieu da tao duoc refresh ngay.
- Sidebar/project detail/task list khong can F5 cung thay task moi.
- Khong lam hong cac logic da pass o Dot 6.1, 6.2, 6.3.
