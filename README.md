# FlashShop — E-Commerce Microservices Platform

FlashShop là hệ thống thương mại điện tử kiến trúc Microservices xây dựng trên nền tảng .NET 8 (C#), thiết kế tối ưu cho các chiến dịch Flash Sale lưu lượng cao.

## 🏗️ Kiến trúc Hệ thống (Runtime View)

Hệ thống bao gồm 5 Business Microservices và các hạ tầng đồng bộ/bất đồng bộ:

| Service | Chức năng chính | Giao thức | Database |
|---|---|---|---|
| **API Gateway** | YARP Gateway tiếp nhận & định tuyến REST request | REST Proxy | — |
| **Identity Service** | Quản lý tài khoản, JWT Authentication, Ví FlashPay | gRPC Server / REST | `IdentityDb` |
| **Catalog Service** | Sản phẩm, Danh mục, Flash Sale Campaigns | REST API | `CatalogDb` |
| **Inventory Service** | Quản lý tồn kho, Giữ chỗ kho ngầm (`ReserveStock`) | MassTransit Events | `InventoryDb` |
| **Ordering Service** | Giỏ hàng, Đặt hàng, Vòng đời đơn hàng, Hangfire Jobs | gRPC Client / Event Pub-Sub / Raw SQL | `OrderingDb` |
| **Notification Service** | Gửi Email xác nhận & báo cáo qua MailKit SMTP | MassTransit Events | `NotificationDb` |

---

## 🎨 Sơ đồ Kiến trúc (Architecture Diagrams)

- **Runtime Architecture Diagram**: `docs/diagrams/renders/03_runtime_architecture.drawio`
- **Event Flow Diagram (RabbitMQ MassTransit)**: `docs/diagrams/renders/07_event_flow_diagram.drawio`
- **Communication Matrix Grid**: `docs/diagrams/renders/08_communication_matrix.drawio`
- **Order Checkout Sequence Diagram**: `docs/diagrams/04_sequence_order_checkout.mmd`
- **Wallet Topup & Timeout Sequence Diagram**: `docs/diagrams/05_sequence_wallet_payment.mmd`

---

## 🚀 Công nghệ Sử dụng

- **Backend Framework**: .NET 8 Web API, C#
- **API Gateway**: YARP (Yet Another Reverse Proxy)
- **Synchronous Protocol**: gRPC (HTTP/2) + REST Fallback
- **Asynchronous Messaging**: MassTransit + RabbitMQ Message Broker
- **Database**: SQL Server (EF Core + Direct Raw SQL Execution)
- **Background Jobs**: Hangfire Recurring Jobs (`Cron.Minutely`, `Cron.Daily`)
- **Emailer**: MailKit SMTP Client
