# System Context Specification

FlashShop là hệ thống Thương mại Điện tử Microservices được thiết kế theo mô hình Event-Driven Architecture trên nền tảng .NET 8.

## Các thành phần biên (System Boundaries)
- **Web / Mobile Client**: Tương tác với hệ thống qua REST API.
- **YARP API Gateway**: Cửa ngõ duy nhất điều hướng request.
- **Microservices Layer**: Identity, Catalog, Inventory, Ordering, Notification.
- **Message Broker**: RabbitMQ (MassTransit).
