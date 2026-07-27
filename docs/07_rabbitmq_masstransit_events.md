# RabbitMQ MassTransit Message Contracts Specification

Danh sách 5 Event Contracts chính trong hệ thống:
1. `OrderCreatedEvent`: Bắn bởi Ordering khi tạo đơn COD.
2. `OrderPaidEvent`: Bắn bởi Ordering khi thanh toán Ví thành công.
3. `OrderCancelledEvent`: Bắn bởi Ordering khi đơn quá hạn bị hủy.
4. `InventoryReservedEvent`: Bắn bởi Inventory khi giữ chỗ kho thành công.
5. `InventoryReservationFailedEvent`: Bắn bởi Inventory khi hết kho.
