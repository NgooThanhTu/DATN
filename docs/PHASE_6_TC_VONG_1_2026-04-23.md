# PHASE 6 - TC VONG 1

## Pham vi vong nay

Vong 1 chi test `Dot 6.1`:

- tu `repo analysis`
- tao `work items that` vao project
- refresh du lieu ngay

Chua test:

- AI breakdown thanh subtask
- AI goi y assignee
- AI chot backlog/cycle

---

## Dieu kien truoc khi test

1. Backend dang chay.
2. Frontend dang chay.
3. Ban da dang nhap bang:
   - `System Admin`
   - hoac role co quyen trong project: `PM / PO / SM / Admin`
4. Da co 1 project dang duoc chon tren sidebar.
5. Trong `AI Assistant`, da phan tich repo thanh cong va nhin thay:
   - `Quick wins`
   - `Medium tasks`
   - `Risky tasks`

---

## TC61A - Tao Quick wins vao project

### Buoc test

1. Mo `AI Assistant`.
2. Phan tich repo GitHub thanh cong.
3. Bam `Create quick wins`.
4. Quan sat thong bao thanh cong.
5. Quay ve project dang chon.
6. Mo `Work items`.

### Ket qua mong doi

- Tao duoc task that.
- So task moi bang so muc `Quick wins` da phan tich.
- Task xuat hien trong project ma khong can F5.
- Moi task co:
  - title
  - description co ghi nguon AI/repo
  - priority
  - estimate hours
  - story points
  - status mac dinh

---

## TC61B - Tao Medium tasks vao project

### Buoc test

1. Tiep tuc o `AI Assistant`.
2. Bam `Create medium tasks`.
3. Mo lai `Work items` trong project.

### Ket qua mong doi

- Tao duoc nhom `Medium tasks`.
- Khong ghi de cac task da tao truoc do.
- Priority va estimate cua task moi duoc luu.
- Story point duoc sinh tu estimate.

---

## TC61C - Tao Risky tasks vao project

### Buoc test

1. Bam `Create risky tasks`.
2. Mo `Work items`.
3. Kiem tra task vua tao.

### Ket qua mong doi

- Tao duoc nhom `Risky tasks`.
- Task risky duoc tao vao cung project dang chon.
- Task xuat hien ngay trong work list.

---

## TC61D - Tao tat ca mot lan

### Buoc test

1. Phan tich 1 repo moi, hoac tiep tuc voi ket qua hien tai.
2. Bam `Create all`.

### Ket qua mong doi

- Tao toan bo cac muc dang co trong `quick wins + medium tasks + risky tasks`.
- Khong bao loi server.
- Thong bao hien dung so task duoc tao.

---

## TC61E - Kiem tra quyen

### Buoc test

1. Dang nhap bang user chi la member/dev khong co quyen PM/PO/SM/Admin theo project.
2. Vao `AI Assistant`.
3. Bam mot nut tao backlog AI.

### Ket qua mong doi

- He thong chan tao.
- Bao loi quyen truy cap khong hop le.
- Khong tao task moi vao project.

---

## TC61F - Kiem tra project chua duoc chon

### Buoc test

1. Vao `AI Assistant` trong trang thai khong co `currentProjectId`.
2. Phan tich repo.
3. Bam nut tao backlog.

### Ket qua mong doi

- He thong khong goi tao task.
- Bao canh bao: can chon project truoc.

---

## TC61G - Kiem tra fallback Gemini

### Buoc test

1. Phan tich repo trong luc Gemini tam loi nhung GitHub van doc duoc.
2. Kiem tra AI van tra ve fallback analysis.
3. Bam `Create all`.

### Ket qua mong doi

- Van tao duoc work items tu fallback analysis.
- Khong phu thuoc Gemini phai tra ve ket qua hoan hao.

---

## Mau bao cao tra ket qua

Ban chi can tra theo mau:

`TC61A: PASS/FAIL + mo ta ngan`

`TC61B: PASS/FAIL + mo ta ngan`

`TC61C: PASS/FAIL + mo ta ngan`

`TC61D: PASS/FAIL + mo ta ngan`

`TC61E: PASS/FAIL + mo ta ngan`

`TC61F: PASS/FAIL + mo ta ngan`

`TC61G: PASS/FAIL + mo ta ngan`

