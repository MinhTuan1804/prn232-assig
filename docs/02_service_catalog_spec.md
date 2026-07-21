# Service Catalog Specification

| Service | Responsibility | Port | DB |
|---|---|---|---|
| Identity.Api | User Authentication, JWT, Wallet | 5001 | IdentityDb |
| Catalog.Api | Products, Categories, Flash Sales | 5002 | CatalogDb |
| Inventory.Api | Stock Reservation & Inventory | 5003 | InventoryDb |
| Ordering.Api | Cart, Checkout, Order Lifecycle | 5004 | OrderingDb |
| Notification.Api | Email Notifications & Reports | 5005 | NotificationDb |
