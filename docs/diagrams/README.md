# 📐 FlashShop Microservices - Architecture Diagrams Index

Tài liệu này tổng hợp toàn bộ **Sơ đồ Kiến trúc & Vận hành (Diagrams)** cho dự án FlashShop Microservices, được biên soạn chuẩn theo tài liệu **"HƯỚNG DẪN MÔ TẢ KIẾN TRÚC HỆ THỐNG MICROSERVICES"** (Môn PRN232).

---

## 🗂️ Danh sách các Sơ đồ trong Thư mục

### 1. Góc nhìn Bối cảnh Hệ thống (System Context View)
- 📄 **[01_system_context.d2](file:///c:/Users/anh58/Downloads/ass_prn232/prn232-assig/docs/diagrams/01_system_context.d2)**: Sơ đồ System Context thể hiện tác nhân (Actors), API Gateway, Phạm vi Microservices và các Hệ thống bên ngoài (MS SQL Server, RabbitMQ, Mail SMTP).
- 📄 **[02_usecase_diagram.puml](file:///c:/Users/anh58/Downloads/ass_prn232/prn232-assig/docs/diagrams/02_usecase_diagram.puml)**: Sơ đồ Use Case Diagram (PlantUML) thể hiện các nhóm chức năng chính của Khách hàng, Quản trị viên và Background Workers.

### 2. Góc nhìn Vận hành & Giao tiếp (Runtime View)
- 📄 **[03_runtime_architecture.d2](file:///c:/Users/anh58/Downloads/ass_prn232/prn232-assig/docs/diagrams/03_runtime_architecture.d2)**: Sơ đồ Runtime Architecture thể hiện 5 Microservices, YARP Gateway, 5 CSDL độc lập và 3 hình thức giao tiếp (REST API, gRPC, RabbitMQ).
- 📄 **[04_sequence_order_checkout.mmd](file:///c:/Users/anh58/Downloads/ass_prn232/prn232-assig/docs/diagrams/04_sequence_order_checkout.mmd)**: Sequence Diagram (Mermaid) cho luồng Đặt hàng, Kiểm tra kho & Giữ tồn kho qua gRPC + RabbitMQ.
- 📄 **[05_sequence_wallet_payment.mmd](file:///c:/Users/anh58/Downloads/ass_prn232/prn232-assig/docs/diagrams/05_sequence_wallet_payment.mmd)**: Sequence Diagram (Mermaid) cho luồng Thanh toán Ví điện tử gRPC & Tự động Hủy đơn quá hạn ngầm (Hangfire Job).

### 3. Góc nhìn Triển khai Hạ tầng (Deployment View)
- 📄 **[06_deployment_docker.d2](file:///c:/Users/anh58/Downloads/ass_prn232/prn232-assig/docs/diagrams/06_deployment_docker.d2)**: Sơ đồ Deployment Diagram thể hiện 8 Docker Containers, Dải mạng `flashshop-network` (bridge), Bảng Port Mapping và Docker Persistent Volumes.

---

## 🛠️ Hướng dẫn Xem và Render Sơ đồ

1. **Xem trực tiếp trong IDE**:
   - Sử dụng Extension **D2**, **PlantUML**, hoặc **Mermaid Preview** trong VS Code / Visual Studio để xem sơ đồ trực quan.
2. **Xem online**:
   - Copy nội dung `.mmd` dán vào: [Mermaid Live Editor](https://mermaid.live)
   - Copy nội dung `.puml` dán vào: [PlantUML Web Server](https://www.plantuml.com/plantuml)
   - Copy nội dung `.d2` dán vào: [D2 Playground](https://play.d2lang.com)
