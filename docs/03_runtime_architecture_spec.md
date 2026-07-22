# Runtime Architecture Specification

Mô tả chi tiết kiến trúc động của FlashShop Microservices:
- **Client-to-Gateway**: REST API qua YARP Reverse Proxy.
- **Service-to-Service (Sync)**: gRPC PayWithWallet (Ordering -> Identity).
- **Service-to-Service (Async)**: RabbitMQ MassTransit Event Bus.
- **Service-to-Database**: Database-per-Service qua EF Core SQL Server.
- **Direct Cross-DB Exception**: Ordering -> CatalogDb via Direct Raw SQL.
