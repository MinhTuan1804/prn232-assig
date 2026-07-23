# Communication Protocol Matrix Specification

Tóm tắt ma trận giao tiếp giữa 5 Microservices:
- **YARP Gateway -> All Services**: REST Proxy
- **Ordering -> Identity**: gRPC HTTP/2 (`PayWithWallet`) + REST Fallback
- **Ordering -> CatalogDb**: Raw SQL Direct Execution
- **Ordering ↔ Inventory**: MassTransit Events (`OrderCreatedEvent`, `InventoryReservedEvent`)
- **Ordering/Inventory ➔ Notification**: MassTransit Events (`OrderPaidEvent`, `OrderCancelledEvent`)
