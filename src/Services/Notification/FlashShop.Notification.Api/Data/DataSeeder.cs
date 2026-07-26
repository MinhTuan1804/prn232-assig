using FlashShop.Notification.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Notification.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(NotificationDbContext context)
    {
        var existingTemplates = await context.NotificationTemplates.ToListAsync();

        var templatesToUpsert = new List<NotificationTemplate>
        {
            new NotificationTemplate
            {
                Id = existingTemplates.FirstOrDefault(t => t.Key == "OrderAwaitingPayment")?.Id ?? Guid.NewGuid(),
                Key = "OrderAwaitingPayment",
                Subject = "⚡ [FlashShop] Đơn hàng {{OrderNumber}} đang chờ thanh toán",
                IsActive = true,
                Body = """
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Đơn hàng đang chờ thanh toán</title>
                </head>
                <body style="margin:0; padding:0; background-color:#07070C; font-family:'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color:#E2E8F0; -webkit-font-smoothing:antialiased;">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background-color:#07070C; padding: 40px 10px;">
                        <tr>
                            <td align="center">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px; background-color:#0D0D16; border:1px solid #1F1F2E; border-radius:20px; overflow:hidden; box-shadow: 0 20px 50px rgba(0, 0, 0, 0.8);">
                                    <tr>
                                        <td style="background: linear-gradient(135deg, #161626 0%, #0D0D16 100%); padding: 32px 40px; border-bottom:1px solid #1F1F2E; text-align:center;">
                                            <div style="display:inline-block; background:linear-gradient(90deg, #FF1E27, #FF5533); color:#FFFFFF; font-weight:800; font-size:14px; letter-spacing:3px; padding:6px 18px; border-radius:30px; text-transform:uppercase; margin-bottom:12px;">FLASHSHOP</div>
                                            <h1 style="margin:10px 0 0; color:#FFFFFF; font-size:24px; font-weight:700;">Xác Nhận Đặt Hàng</h1>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 36px 40px;">
                                            <p style="font-size:16px; line-height:1.6; color:#CBD5E1; margin-top:0;">Kính gửi <strong>Quý khách hàng</strong>,</p>
                                            <p style="font-size:15px; line-height:1.6; color:#94A3B8;">Đơn hàng <strong style="color:#FFB800; font-family: monospace;">{{OrderNumber}}</strong> của bạn đã được khởi tạo thành công và đang <strong>chờ thanh toán</strong>.</p>
                                            <div style="background-color:#131320; border:1px solid #2A2A3D; border-radius:14px; padding:24px; margin: 28px 0;">
                                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                                    <tr>
                                                        <td style="padding-bottom:12px; color:#64748B; font-size:13px; text-transform:uppercase; font-weight:600; letter-spacing:1px;">Thông Tin Đơn Hàng</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Mã đơn hàng:</td>
                                                        <td align="right" style="padding: 8px 0; color:#FFFFFF; font-weight:700; font-family:monospace;">{{OrderNumber}}</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Tổng tiền thanh toán:</td>
                                                        <td align="right" style="padding: 8px 0; color:#FF1E27; font-weight:800; font-size:18px; font-family:monospace;">{{TotalAmount}} VNĐ</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Hạn chót thanh toán:</td>
                                                        <td align="right" style="padding: 8px 0; color:#FFB800; font-weight:600; font-size:14px;">{{PaymentDeadline}}</td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <p style="font-size:14px; color:#94A3B8; line-height:1.6;">Vui lòng hoàn tất thanh toán trước thời hạn để giữ suất ưu đãi Flash Sale cho sản phẩm.</p>
                                            <div style="text-align:center; margin-top:32px;">
                                                <a href="https://fe-flash-sale-prn-232-ir31.vercel.app/my-orders" style="display:inline-block; background:linear-gradient(135deg, #FF1E27 0%, #D00015 100%); color:#FFFFFF; font-weight:700; font-size:15px; text-decoration:none; padding:14px 36px; border-radius:30px; box-shadow:0 8px 25px rgba(255, 30, 39, 0.4);">Thanh Toán Ngay</a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="background-color:#0A0A12; padding: 24px 40px; border-top:1px solid #1F1F2E; text-align:center; color:#64748B; font-size:12px; line-height:1.6;">
                                            <p style="margin:0;">Cần hỗ trợ? Liên hệ hotline <strong>1900-FLASH</strong> hoặc phản hồi email này.</p>
                                            <p style="margin:6px 0 0;">© 2026 FlashShop E-Commerce. All rights reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """
            },
            new NotificationTemplate
            {
                Id = existingTemplates.FirstOrDefault(t => t.Key == "OrderPaid")?.Id ?? Guid.NewGuid(),
                Key = "OrderPaid",
                Subject = "🎉 [FlashShop] Thanh toán thành công đơn hàng {{OrderNumber}}",
                IsActive = true,
                Body = """
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Thanh toán thành công</title>
                </head>
                <body style="margin:0; padding:0; background-color:#07070C; font-family:'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color:#E2E8F0; -webkit-font-smoothing:antialiased;">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background-color:#07070C; padding: 40px 10px;">
                        <tr>
                            <td align="center">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px; background-color:#0D0D16; border:1px solid #1F1F2E; border-radius:20px; overflow:hidden; box-shadow: 0 20px 50px rgba(0, 0, 0, 0.8);">
                                    <tr>
                                        <td style="background: linear-gradient(135deg, #161626 0%, #0D0D16 100%); padding: 32px 40px; border-bottom:1px solid #1F1F2E; text-align:center;">
                                            <div style="display:inline-block; background:linear-gradient(90deg, #00FF87, #60EFFF); color:#07070C; font-weight:800; font-size:14px; letter-spacing:3px; padding:6px 18px; border-radius:30px; text-transform:uppercase; margin-bottom:12px;">FLASHSHOP</div>
                                            <h1 style="margin:10px 0 0; color:#FFFFFF; font-size:24px; font-weight:700;">Thanh Toán Thành Công!</h1>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 36px 40px;">
                                            <p style="font-size:16px; line-height:1.6; color:#CBD5E1; margin-top:0;">Kính gửi <strong>Quý khách hàng</strong>,</p>
                                            <p style="font-size:15px; line-height:1.6; color:#94A3B8;">Giao dịch thanh toán cho đơn hàng <strong style="color:#00FF87; font-family: monospace;">{{OrderNumber}}</strong> đã hoàn tất thành công qua Ví FlashPay.</p>
                                            <div style="background-color:#131320; border:1px solid #10B981; border-radius:14px; padding:24px; margin: 28px 0;">
                                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                                    <tr>
                                                        <td style="padding-bottom:12px; color:#10B981; font-size:13px; text-transform:uppercase; font-weight:700; letter-spacing:1px;">✓ Đã Xác Nhận Thanh Toán</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Mã đơn hàng:</td>
                                                        <td align="right" style="padding: 8px 0; color:#FFFFFF; font-weight:700; font-family:monospace;">{{OrderNumber}}</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Số tiền đã thanh toán:</td>
                                                        <td align="right" style="padding: 8px 0; color:#00FF87; font-weight:800; font-size:18px; font-family:monospace;">{{TotalAmount}} VNĐ</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Trạng thái vận chuyển:</td>
                                                        <td align="right" style="padding: 8px 0; color:#FFFFFF; font-weight:600; font-size:14px;">Đang chuẩn bị đóng gói</td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <p style="font-size:14px; color:#94A3B8; line-height:1.6;">Đơn hàng của bạn sẽ được bàn giao cho đơn vị vận chuyển trong thời gian sớm nhất.</p>
                                            <div style="text-align:center; margin-top:32px;">
                                                <a href="https://fe-flash-sale-prn-232-ir31.vercel.app/my-orders" style="display:inline-block; background:linear-gradient(135deg, #10B981 0%, #059669 100%); color:#FFFFFF; font-weight:700; font-size:15px; text-decoration:none; padding:14px 36px; border-radius:30px; box-shadow:0 8px 25px rgba(16, 185, 129, 0.4);">Xem Chi Tiết Đơn Hàng</a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="background-color:#0A0A12; padding: 24px 40px; border-top:1px solid #1F1F2E; text-align:center; color:#64748B; font-size:12px; line-height:1.6;">
                                            <p style="margin:0;">Cảm ơn bạn đã tin tưởng mua sắm tại FlashShop!</p>
                                            <p style="margin:6px 0 0;">© 2026 FlashShop E-Commerce. All rights reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """
            },
            new NotificationTemplate
            {
                Id = existingTemplates.FirstOrDefault(t => t.Key == "OrderOutOfStock")?.Id ?? Guid.NewGuid(),
                Key = "OrderOutOfStock",
                Subject = "⚠️ [FlashShop] Đơn hàng {{OrderNumber}} tạm thời hết hàng",
                IsActive = true,
                Body = """
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Sản phẩm tạm hết hàng</title>
                </head>
                <body style="margin:0; padding:0; background-color:#07070C; font-family:'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color:#E2E8F0; -webkit-font-smoothing:antialiased;">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background-color:#07070C; padding: 40px 10px;">
                        <tr>
                            <td align="center">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px; background-color:#0D0D16; border:1px solid #1F1F2E; border-radius:20px; overflow:hidden; box-shadow: 0 20px 50px rgba(0, 0, 0, 0.8);">
                                    <tr>
                                        <td style="background: linear-gradient(135deg, #161626 0%, #0D0D16 100%); padding: 32px 40px; border-bottom:1px solid #1F1F2E; text-align:center;">
                                            <div style="display:inline-block; background:linear-gradient(90deg, #FFB800, #FF5500); color:#07070C; font-weight:800; font-size:14px; letter-spacing:3px; padding:6px 18px; border-radius:30px; text-transform:uppercase; margin-bottom:12px;">FLASHSHOP</div>
                                            <h1 style="margin:10px 0 0; color:#FFFFFF; font-size:24px; font-weight:700;">Thông Báo Tồn Kho</h1>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 36px 40px;">
                                            <p style="font-size:16px; line-height:1.6; color:#CBD5E1; margin-top:0;">Kính gửi <strong>Quý khách hàng</strong>,</p>
                                            <p style="font-size:15px; line-height:1.6; color:#94A3B8;">Rất tiếc, đơn hàng <strong style="color:#FFB800; font-family: monospace;">{{OrderNumber}}</strong> của bạn chưa thể thực hiện do một số sản phẩm trong kho tạm thời đã hết hàng.</p>
                                            <div style="background-color:#131320; border:1px solid #FFB800; border-radius:14px; padding:24px; margin: 28px 0;">
                                                <p style="margin:0; color:#FFB800; font-weight:600;">Chúng tôi rất tiếc vì sự bất tiện này. Hệ thống đã tự động hoàn trả số dư ví (nếu đã thanh toán) hoặc hủy yêu cầu giữ hàng.</p>
                                            </div>
                                            <div style="text-align:center; margin-top:32px;">
                                                <a href="https://fe-flash-sale-prn-232-ir31.vercel.app/" style="display:inline-block; background:linear-gradient(135deg, #FF1E27 0%, #D00015 100%); color:#FFFFFF; font-weight:700; font-size:15px; text-decoration:none; padding:14px 36px; border-radius:30px; box-shadow:0 8px 25px rgba(255, 30, 39, 0.4);">Khám Phá Sản Phẩm Khác</a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="background-color:#0A0A12; padding: 24px 40px; border-top:1px solid #1F1F2E; text-align:center; color:#64748B; font-size:12px; line-height:1.6;">
                                            <p style="margin:0;">Cần hỗ trợ thêm? Liên hệ hotline <strong>1900-FLASH</strong>.</p>
                                            <p style="margin:6px 0 0;">© 2026 FlashShop E-Commerce. All rights reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """
            },
            new NotificationTemplate
            {
                Id = existingTemplates.FirstOrDefault(t => t.Key == "OrderCancelled")?.Id ?? Guid.NewGuid(),
                Key = "OrderCancelled",
                Subject = "❌ [FlashShop] Thông báo hủy đơn hàng {{OrderNumber}}",
                IsActive = true,
                Body = """
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Đơn hàng đã hủy</title>
                </head>
                <body style="margin:0; padding:0; background-color:#07070C; font-family:'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color:#E2E8F0; -webkit-font-smoothing:antialiased;">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background-color:#07070C; padding: 40px 10px;">
                        <tr>
                            <td align="center">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px; background-color:#0D0D16; border:1px solid #1F1F2E; border-radius:20px; overflow:hidden; box-shadow: 0 20px 50px rgba(0, 0, 0, 0.8);">
                                    <tr>
                                        <td style="background: linear-gradient(135deg, #161626 0%, #0D0D16 100%); padding: 32px 40px; border-bottom:1px solid #1F1F2E; text-align:center;">
                                            <div style="display:inline-block; background:linear-gradient(90deg, #64748B, #475569); color:#FFFFFF; font-weight:800; font-size:14px; letter-spacing:3px; padding:6px 18px; border-radius:30px; text-transform:uppercase; margin-bottom:12px;">FLASHSHOP</div>
                                            <h1 style="margin:10px 0 0; color:#FFFFFF; font-size:24px; font-weight:700;">Đơn Hàng Đã Hủy</h1>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 36px 40px;">
                                            <p style="font-size:16px; line-height:1.6; color:#CBD5E1; margin-top:0;">Kính gửi <strong>Quý khách hàng</strong>,</p>
                                            <p style="font-size:15px; line-height:1.6; color:#94A3B8;">Đơn hàng <strong style="color:#F87171; font-family: monospace;">{{OrderNumber}}</strong> đã được hủy thành công theo yêu cầu hoặc do quá thời hạn thanh toán.</p>
                                            <div style="background-color:#131320; border:1px solid #EF4444; border-radius:14px; padding:24px; margin: 28px 0;">
                                                <p style="margin:0; color:#F87171; font-weight:600;">Số lượng sản phẩm trong đơn đã được tự động hoàn lại kho hàng.</p>
                                            </div>
                                            <div style="text-align:center; margin-top:32px;">
                                                <a href="https://fe-flash-sale-prn-232-ir31.vercel.app/" style="display:inline-block; background:linear-gradient(135deg, #FF1E27 0%, #D00015 100%); color:#FFFFFF; font-weight:700; font-size:15px; text-decoration:none; padding:14px 36px; border-radius:30px; box-shadow:0 8px 25px rgba(255, 30, 39, 0.4);">Tiếp Tục Mua Sắm</a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="background-color:#0A0A12; padding: 24px 40px; border-top:1px solid #1F1F2E; text-align:center; color:#64748B; font-size:12px; line-height:1.6;">
                                            <p style="margin:0;">Cần hỗ trợ thêm? Liên hệ hotline <strong>1900-FLASH</strong>.</p>
                                            <p style="margin:6px 0 0;">© 2026 FlashShop E-Commerce. All rights reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """
            },
            new NotificationTemplate
            {
                Id = existingTemplates.FirstOrDefault(t => t.Key == "DailySalesReport")?.Id ?? Guid.NewGuid(),
                Key = "DailySalesReport",
                Subject = "📊 [FlashShop Admin] Báo cáo doanh thu ngày {{Date}}",
                IsActive = true,
                Body = """
                <!DOCTYPE html>
                <html lang="vi">
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Báo cáo doanh thu ngày</title>
                </head>
                <body style="margin:0; padding:0; background-color:#07070C; font-family:'Plus Jakarta Sans', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color:#E2E8F0; -webkit-font-smoothing:antialiased;">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="background-color:#07070C; padding: 40px 10px;">
                        <tr>
                            <td align="center">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" border="0" style="max-width:600px; background-color:#0D0D16; border:1px solid #1F1F2E; border-radius:20px; overflow:hidden; box-shadow: 0 20px 50px rgba(0, 0, 0, 0.8);">
                                    <tr>
                                        <td style="background: linear-gradient(135deg, #161626 0%, #0D0D16 100%); padding: 32px 40px; border-bottom:1px solid #1F1F2E; text-align:center;">
                                            <div style="display:inline-block; background:linear-gradient(90deg, #FFB800, #00FF87); color:#07070C; font-weight:800; font-size:14px; letter-spacing:3px; padding:6px 18px; border-radius:30px; text-transform:uppercase; margin-bottom:12px;">FLASHSHOP ADMIN</div>
                                            <h1 style="margin:10px 0 0; color:#FFFFFF; font-size:24px; font-weight:700;">Báo Cáo Doanh Thu Ngày</h1>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="padding: 36px 40px;">
                                            <p style="font-size:16px; line-height:1.6; color:#CBD5E1; margin-top:0;">Kính gửi <strong>Quản trị viên Hệ thống</strong>,</p>
                                            <p style="font-size:15px; line-height:1.6; color:#94A3B8;">Dưới đây là báo cáo tổng quan hoạt động kinh doanh ngày <strong>{{Date}}</strong>:</p>
                                            <div style="background-color:#131320; border:1px solid #2A2A3D; border-radius:14px; padding:24px; margin: 28px 0;">
                                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                                    <tr>
                                                        <td style="padding-bottom:12px; color:#64748B; font-size:13px; text-transform:uppercase; font-weight:600; letter-spacing:1px;">Chỉ Số Kinh Doanh</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Tổng số đơn hàng:</td>
                                                        <td align="right" style="padding: 8px 0; color:#FFFFFF; font-weight:700; font-family:monospace;">{{TotalOrders}} đơn</td>
                                                    </tr>
                                                    <tr>
                                                        <td style="padding: 8px 0; color:#94A3B8; font-size:14px;">Tổng doanh thu:</td>
                                                        <td align="right" style="padding: 8px 0; color:#00FF87; font-weight:800; font-size:18px; font-family:monospace;">{{TotalRevenue}} VNĐ</td>
                                                    </tr>
                                                </table>
                                            </div>
                                            <div style="text-align:center; margin-top:32px;">
                                                <a href="https://fe-flash-sale-prn-232-ir31.vercel.app/admin" style="display:inline-block; background:linear-gradient(135deg, #FFB800 0%, #D97706 100%); color:#07070C; font-weight:800; font-size:15px; text-decoration:none; padding:14px 36px; border-radius:30px; box-shadow:0 8px 25px rgba(255, 184, 0, 0.3);">Truy Cập Trang Admin</a>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="background-color:#0A0A12; padding: 24px 40px; border-top:1px solid #1F1F2E; text-align:center; color:#64748B; font-size:12px; line-height:1.6;">
                                            <p style="margin:0;">Báo cáo được khởi tạo tự động bởi Hangfire Scheduler.</p>
                                            <p style="margin:6px 0 0;">© 2026 FlashShop E-Commerce. All rights reserved.</p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
                """
            }
        };

        foreach (var t in templatesToUpsert)
        {
            var existing = existingTemplates.FirstOrDefault(e => e.Key == t.Key);
            if (existing != null)
            {
                existing.Subject = t.Subject;
                existing.Body = t.Body;
                existing.IsActive = t.IsActive;
            }
            else
            {
                context.NotificationTemplates.Add(t);
            }
        }

        await context.SaveChangesAsync();
    }
}
