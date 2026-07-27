# Code Evidence Mapping Guide for Runtime View

Bảng tra cứu minh chứng mã nguồn dòng theo dòng cho buổi bảo vệ Runtime View:
- YARP Routes: Gateway `appsettings.json` L17-50
- gRPC Call: Ordering `OrderService.cs` L67 & Identity `WalletGrpcService.cs` L16
- Raw SQL: Ordering `OrderService.cs` L163-164
- RabbitMQ Events: Ordering `OrderService.cs` L177-188 & Inventory `OrderCreatedConsumer.cs` L34
- Hangfire Jobs: Ordering `Program.cs` L150 & Notification `Program.cs` L152
