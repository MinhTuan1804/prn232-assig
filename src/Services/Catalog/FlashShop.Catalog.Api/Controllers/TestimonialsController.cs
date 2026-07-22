using FlashShop.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace FlashShop.Catalog.Api.Controllers;

[ApiController]
[Route("api/testimonials")]
public class TestimonialsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTestimonials()
    {
        var testimonials = new[]
        {
            new
            {
                Id = "testim-1",
                CustomerName = "Hoàng Anh Tuấn",
                AvatarUrl = "https://images.unsplash.com/photo-1534528741775-53994a69daeb?auto=format&fit=crop&q=80&w=200",
                Role = "VIP Member / Verified Buyer",
                Rating = 5,
                ProductName = "iPhone 17 Pro 256GB",
                Comment = "Săn Flash Sale thành công iPhone 17 Pro giá siêu hời! Giao hàng hỏa tốc trong 2h, máy nguyên seal chính hãng VN/A. Quá hài lòng với dịch vụ!",
                CreatedDate = "2026-07-20"
            },
            new
            {
                Id = "testim-2",
                CustomerName = "Nguyễn Minh Châu",
                AvatarUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?auto=format&fit=crop&q=80&w=200",
                Role = "Pro Gamer",
                Rating = 5,
                ProductName = "Bàn Phím Cơ Apex Pro + Chuột Gaming Prime",
                Comment = "Bàn phím gõ cực êm, switch OmniPoint nhạy đét! Săn sale giảm hơn 1.6 triệu. Chuột Prime cầm đầm tay, bắn FPS mượt mà. 10/10 điểm!",
                CreatedDate = "2026-07-21"
            },
            new
            {
                Id = "testim-3",
                CustomerName = "Trần Thị Mai Phương",
                AvatarUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&q=80&w=200",
                Role = "Content Creator",
                Rating = 5,
                ProductName = "MacBook Pro 16 M3 Max",
                Comment = "Máy dựng video 4K siêu nhanh, xuất file 8K phơi phới không hề nóng. FlashShop giao hàng chuẩn seal, bảo hành 12 tháng tận tâm!",
                CreatedDate = "2026-07-22"
            },
            new
            {
                Id = "testim-4",
                CustomerName = "Đặng Quốc Huy",
                AvatarUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&q=80&w=200",
                Role = "Tech Enthusiast",
                Rating = 5,
                ProductName = "Samsung Galaxy S24 Ultra",
                Comment = "Màn hình phẳng chống lóa tuyệt đẹp, camera 200MP chụp siêu nét. Đặt hàng lúc 00:00 Flash Sale, sáng ra 9h đã có shipper giao hàng!",
                CreatedDate = "2026-07-22"
            },
            new
            {
                Id = "testim-5",
                CustomerName = "Lê Hoàng Bảo",
                AvatarUrl = "https://images.unsplash.com/photo-1522075469751-3a6694fb2f61?auto=format&fit=crop&q=80&w=200",
                Role = "Streamer / Reviewer",
                Rating = 5,
                ProductName = "Tai Nghe Sony WH-1000XM5",
                Comment = "Chống ồn đỉnh cao, mic lọc tạp âm quá tốt cho buổi livestream. FlashShop tư vấn hỗ trợ nhiệt tình, freeship nhận trong ngày!",
                CreatedDate = "2026-07-22"
            }
        };

        return Ok(ApiResponse<object>.SuccessResponse(testimonials));
    }
}
