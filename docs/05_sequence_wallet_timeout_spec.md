# Wallet TopUp and Hangfire Timeout Specification

## Luồng A: Wallet TopUp
Client -> Gateway -> Identity.Api `POST /api/wallets/topup` -> Cập nhật balance IdentityDb -> Return 200 OK.

## Luồng B: Hangfire Timeout Job
Hangfire Minutely Job -> Ordering `CancelExpiredOrdersAsync()` -> Quét đơn quá hạn 15 phút -> Đổi status `Cancelled` -> Publish `OrderCancelledEvent` -> Inventory `ReleaseReservationAsync` -> Notification send email.
