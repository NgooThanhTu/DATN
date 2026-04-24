# PHASE 6 - TC VONG 2

## Pham vi vong nay

Vong 2 test `Dot 6.2`:

- AI preview subtasks trong task detail
- xac nhan roi moi tao that
- tao xong phai refresh ngay

---

## Dieu kien truoc khi test

1. Backend dang chay.
2. Frontend dang chay.
3. Ban dang o trong 1 project co quyen tao task/subtask.
4. Da co it nhat 1 task cha co title/description de AI co du lieu phan tich.

---

## TC62A - AI breakdown chi preview, chua tao ngay

### Buoc test

1. Mo `Task detail` cua 1 task cha.
2. Bam `AI split into subtasks`.

### Ket qua mong doi

- Khong tao subtask ngay vao DB.
- Hien panel `AI subtask preview`.
- Trong panel co:
  - title subtask
  - estimate hours
  - priority
  - description

---

## TC62B - Bam discard preview

### Buoc test

1. Sau khi co panel preview.
2. Bam `Discard`.

### Ket qua mong doi

- Panel preview bien mat.
- Khong co subtask nao duoc tao vao task cha.

---

## TC62C - Bam confirm create

### Buoc test

1. Tao lai preview subtasks.
2. Bam `Create X sub-work items`.

### Ket qua mong doi

- Tao duoc subtasks that.
- Danh sach subtask duoi task detail duoc cap nhat ngay.
- Khong can F5.
- So subtask tao ra bang so muc trong preview.

---

## TC62D - Kiem tra parent/subtask sau khi tao

### Buoc test

1. Sau khi AI tao subtasks xong.
2. Bam vao 1 subtask vua tao de mo detail.

### Ket qua mong doi

- Subtask mo dung detail.
- Parent task hien dung.
- Co the back lai task cha nhu logic hien tai.

---

## TC62E - Kiem tra estimate cua subtask AI

### Buoc test

1. Sau khi AI tao subtasks.
2. Kiem tra estimate tung subtask.
3. Kiem tra phan roll-up o task cha.

### Ket qua mong doi

- Estimate cua subtask duoc luu.
- Neu task cha dang dung logic derive tu subtasks thi roll-up van dung.
- Khong bi mat estimate sau khi save/mo lai.

---

## TC62F - Kiem tra loi Gemini/fallback

### Buoc test

1. Neu Gemini loi tam thoi, bam `AI split into subtasks`.

### Ket qua mong doi

- He thong thong bao ro rang.
- Khong lam crash modal.
- Khong tao subtask rac vao DB.

---

## Mau bao cao tra ket qua

`TC62A: PASS/FAIL + mo ta ngan`

`TC62B: PASS/FAIL + mo ta ngan`

`TC62C: PASS/FAIL + mo ta ngan`

`TC62D: PASS/FAIL + mo ta ngan`

`TC62E: PASS/FAIL + mo ta ngan`

`TC62F: PASS/FAIL + mo ta ngan`

