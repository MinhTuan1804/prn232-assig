# Tài liệu Mô tả Kiến trúc Động (Runtime View Specification)

Tài liệu này cung cấp chi tiết thuyết minh và các tài nguyên sơ đồ mô tả luồng vận hành của hệ thống FlashShop Microservices Platform.

## 📋 Mục lục
1. Service Catalog & Trách nhiệm Dịch vụ
2. Runtime Architecture Specification
3. Communication Protocol Matrix
4. Event-Driven Messaging Contract (RabbitMQ)
5. Background Workers & Scheduled Jobs (Hangfire)

---

## 1. Service Catalog

- **Identity Service**: Đăng ký, đăng nhập, JWT Token, Quản lý số dư và giao dịch Ví điện tử FlashPay.
- **Catalog Service**: Quản lý Catalog sản phẩm, danh mục hàng hóa, giá bán và cấu hình Flash Sale.
- **Inventory Service**: Quản lý số lượng kho thực tế, xử lý giữ chỗ kho ngầm (`ReserveStockAsync`) và giải phóng kho (`ReleaseReservationAsync`).
- **Ordering Service**: Quản lý giỏ hàng `CartItems`, xử lý checkout đơn hàng, tích hợp gRPC PayWithWallet, thực thi Raw SQL cập nhật kho hiển thị tức thì, phát hành sự kiện đơn hàng và quản lý vòng đời đơn hàng.
- **Notification Service**: Lắng nghe sự kiện qua RabbitMQ để tự động tạo thông báo và gửi Email xác nhận/hủy đơn qua MailKit.

---

## 2. Background Workers Table

| Service | Job Name | Loại Worker | Schedule / Trigger | Nhiệm vụ chính |
|---|---|---|---|---|
| **Ordering** | `PendingOrderTimeoutJob` | Hangfire Recurring Job | `Cron.Minutely` (Mỗi phút) | Quét các đơn `AwaitingPayment` quá hạn 15 phút chưa trả tiền, hủy đơn và bắn `OrderCancelledEvent`. |
| **Notification** | `DailySalesReportJob` | Hangfire Recurring Job | `59 23 * * *` (23:59 Daily) | Tổng hợp doanh số đơn hàng trong ngày và gửi Email báo cáo cho Admin. |
