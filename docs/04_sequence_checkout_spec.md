# Order Checkout Sequence Specification

Quy trình xử lý đơn hàng (Checkout Flow):
1. Client gửi `POST /api/orders/checkout` tới Gateway.
2. Gateway forward tới Ordering.Api.
3. Ordering kiểm tra CartItems không rỗng.
4. Ordering gọi gRPC `PayWithWalletAsync` sang Identity (REST Fallback dự phòng).
5. Ordering tạo Order và thực thi Raw SQL trừ kho hiển thị CatalogDb.
6. Ordering bắn `OrderCreatedEvent` hoặc `OrderPaidEvent` qua RabbitMQ.
7. Inventory consume `OrderCreatedEvent` để thực hiện `ReserveStockAsync`.
8. Notification consume event để gửi Email qua MailKit SMTP.
