# Hangfire Background Workers Specification

Chi tiết 2 Background Workers chạy ngầm:
1. `PendingOrderTimeoutJob`:
   - Service: Ordering.Api
   - Schedule: `Cron.Minutely` (Mỗi phút)
   - Chức năng: Quét và hủy các đơn AwaitingPayment quá 15 phút.
2. `DailySalesReportJob`:
   - Service: Notification.Api
   - Schedule: `59 23 * * *` (23:59 daily)
   - Chức năng: Tổng hợp báo cáo doanh số trong ngày và gửi Email Admin.
