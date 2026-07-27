# Database-per-Service Schemas Specification

Mô hình 5 SQL Server Databases tách biệt:
- `FlashShop_IdentityDb`: `Users`, `Roles`, `Wallets`, `WalletTransactions`
- `FlashShop_CatalogDb`: `Categories`, `Products`, `FlashSales`, `FlashSaleItems`
- `FlashShop_InventoryDb`: `ProductStocks`, `StockReservations`
- `FlashShop_OrderingDb`: `Orders`, `OrderItems`, `CartItems`
- `FlashShop_NotificationDb`: `Notifications`, `EmailLogs`
