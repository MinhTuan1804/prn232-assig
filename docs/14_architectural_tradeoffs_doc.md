# Architectural Trade-offs Documentation: Direct Raw SQL Execution

## Bối cảnh và Lý do Đánh đổi (Architectural Trade-off)
Trong luồng Flash Sale lưu lượng cao, `Ordering Service` thực thi Raw SQL trực tiếp sang `CatalogDb` để giảm 1 network hop và overhead API.

### Ưu điểm (Pros):
- Tối ưu latency checkout trong Flash Sale.
- Cập nhật kho hiển thị sản phẩm tức thì.

### Đánh đổi (Cons):
- Coupling giữa Ordering Service và CatalogDb schema.
