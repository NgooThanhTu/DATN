# PHASE 5 TC VONG 2 - REPO ANALYSIS AI

## Muc tieu
- Dua phan tich GitHub repo ve backend.
- Khong fetch GitHub truc tiep tu browser nua.
- AI co the doc metadata repo, issue mo, README, ngon ngu chinh va de xuat backlog.

## Cach test

### TC60 - Repo analysis tra ve ket qua
1. Vao trang `AI`.
2. Nhap mot GitHub repo URL hop le.
3. Bam `Analyze repo`.
4. Kiem tra:
   - co hien `summary`
   - co danh sach `Quick wins`
   - co danh sach `Medium tasks`
   - co danh sach `Risky tasks`
   - o chat xuat hien 1 tin nhan bot tom tat repo

Ket qua mong doi:
- PASS neu panel repo hien ket qua day du va khong crash.

### TC61 - Prompt de san vao chat box
1. Sau khi phan tich repo xong, kiem tra o input chat.
2. Xem prompt AI da duoc dien san hay chua.

Ket qua mong doi:
- PASS neu input chat da co prompt goi y de tiep tuc hoi AI.

### TC62 - Repo private hoac token optional
1. Thu repo public khong can token.
2. Neu co repo can token thi nhap token GitHub.
3. Bam `Analyze repo`.

Ket qua mong doi:
- PASS neu repo public van phan tich duoc khi khong co token.
- PASS neu repo can token khong lam vo UI.

### TC63 - Fallback khi GitHub hoac AI loi
1. Thu nhap repo URL sai hoac token sai.
2. Bam `Analyze repo`.

Ket qua mong doi:
- PASS neu co thong bao loi than thien.
- PASS neu UI khong bi treo, khong vo trang.

## Cach gui ket qua cho Codex
- `TC60: Pass/Fail + ghi chu`
- `TC61: Pass/Fail + ghi chu`
- `TC62: Pass/Fail + ghi chu`
- `TC63: Pass/Fail + ghi chu`
