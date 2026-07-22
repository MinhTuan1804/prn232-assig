using FlashShop.Catalog.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Catalog.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(CatalogDbContext dbContext)
    {
        // Force re-seed to update prices & images for all products in VND
        dbContext.FlashSaleItems.RemoveRange(dbContext.FlashSaleItems);
        dbContext.FlashSaleCampaigns.RemoveRange(dbContext.FlashSaleCampaigns);
        dbContext.Products.RemoveRange(dbContext.Products);
        dbContext.Categories.RemoveRange(dbContext.Categories);
        await dbContext.SaveChangesAsync();

        // 1. Categories (6 Categories)
        var categories = new List<Category>
        {
            new Category { Name = "Điện thoại & Máy tính bảng", Description = "Smartphone & Tablet chính hãng mới nhất", SortOrder = 1 },
            new Category { Name = "Máy tính & Laptop", Description = "Laptop đồ họa, gaming, văn phòng cao cấp", SortOrder = 2 },
            new Category { Name = "Tai nghe & Âm thanh", Description = "Tai nghe không dây, loa bluetooth âm thanh đỉnh cao", SortOrder = 3 },
            new Category { Name = "Đồng hồ thông minh", Description = "Smartwatch theo dõi sức khỏe và thể thao", SortOrder = 4 },
            new Category { Name = "Phụ kiện công nghệ", Description = "Sạc nhanh, bàn phím, chuột, cáp sạc cao cấp", SortOrder = 5 },
            new Category { Name = "Thiết bị nhà thông minh", Description = "Robot hút bụi, camera an ninh, đèn thông minh", SortOrder = 6 }
        };

        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();

        var catPhones = categories.First(c => c.Name == "Điện thoại & Máy tính bảng");
        var catLaptops = categories.First(c => c.Name == "Máy tính & Laptop");
        var catAudio = categories.First(c => c.Name == "Tai nghe & Âm thanh");
        var catWatches = categories.First(c => c.Name == "Đồng hồ thông minh");
        var catAccessories = categories.First(c => c.Name == "Phụ kiện công nghệ");
        var catSmartHome = categories.First(c => c.Name == "Thiết bị nhà thông minh");

        var products = new List<Product>();

        void AddItems(int catId, (string Name, decimal Price, string ImgUrl)[] items)
        {
            foreach (var item in items)
            {
                products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = item.Name,
                    Slug = item.Name.ToLower().Replace(" ", "-").Replace("\"", "").Replace("&", "and"),
                    Description = $"{item.Name} chính hãng, bảo hành 12 tháng, giao hàng hỏa tốc trong 2 giờ.",
                    Price = item.Price,
                    ImageUrl = item.ImgUrl,
                    CategoryId = catId,
                    IsActive = true
                });
            }
        }

        // 1. Điện thoại & Máy tính bảng (Giá chuẩn VND)
        AddItems(catPhones.Id, new[]
        {
            ("iPhone 17 Pro 256GB", 34990000m, "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&q=80&w=800"),
            ("iPhone 15 Pro Max 256GB Titan Tự Nhiên", 29990000m, "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&q=80&w=800"),
            ("Samsung Galaxy S24 Ultra 512GB", 31990000m, "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?auto=format&fit=crop&q=80&w=800"),
            ("iPad Pro 13 inch M4 Wifi 256GB", 31990000m, "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?auto=format&fit=crop&q=80&w=800"),
            ("Xiaomi 14 Ultra 512GB", 24990000m, "https://images.unsplash.com/photo-1598327105666-5b89351aff97?auto=format&fit=crop&q=80&w=800"),
            ("OPPO Find X7 Ultra 5G", 21990000m, "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&q=80&w=800"),
            ("iPad Air 11 inch M2 128GB", 14990000m, "https://images.unsplash.com/photo-1561154464-82e9adf32764?auto=format&fit=crop&q=80&w=800"),
            ("Samsung Galaxy Z Fold5 512GB", 41990000m, "https://images.unsplash.com/photo-1580910051074-3eb694886505?auto=format&fit=crop&q=80&w=800"),
            ("Vivo X100 Pro 5G", 20990000m, "https://images.unsplash.com/photo-1565849904461-04a58ad377e0?auto=format&fit=crop&q=80&w=800"),
            ("OnePlus 12 256GB", 19990000m, "https://images.unsplash.com/photo-1574944985070-8f3ebc6b79d2?auto=format&fit=crop&q=80&w=800"),
            ("Google Pixel 8 Pro 128GB", 21990000m, "https://images.unsplash.com/photo-1598327106026-d9521da673d1?auto=format&fit=crop&q=80&w=800"),
            ("Realme GT5 Pro 5G", 15990000m, "https://images.unsplash.com/photo-1546054454-aa26e2b734c7?auto=format&fit=crop&q=80&w=800"),
            ("Asus ROG Phone 8 Pro 512GB", 29990000m, "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9?auto=format&fit=crop&q=80&w=800"),
            ("Nubia RedMagic 9 Pro", 18490000m, "https://images.unsplash.com/photo-1598327105666-5b89351aff97?auto=format&fit=crop&q=80&w=800"),
            ("POCO X6 Pro 5G", 8490000m, "https://images.unsplash.com/photo-1565849904461-04a58ad377e0?auto=format&fit=crop&q=80&w=800"),
            ("iPad Mini 6 64GB Wifi", 11990000m, "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?auto=format&fit=crop&q=80&w=800"),
            ("Samsung Galaxy Tab S9 Ultra", 29990000m, "https://images.unsplash.com/photo-1561154464-82e9adf32764?auto=format&fit=crop&q=80&w=800"),
            ("Xiaomi Pad 6 Max 14", 14990000m, "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?auto=format&fit=crop&q=80&w=800"),
            ("Lenovo Legion Y700 2023", 10990000m, "https://images.unsplash.com/photo-1561154464-82e9adf32764?auto=format&fit=crop&q=80&w=800"),
            ("Honor Magic6 Pro 512GB", 24990000m, "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?auto=format&fit=crop&q=80&w=800"),
            ("Sony Xperia 1 VI 5G", 34990000m, "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&q=80&w=800")
        });

        // 2. Máy tính & Laptop (Giá chuẩn VND)
        AddItems(catLaptops.Id, new[]
        {
            ("MacBook Pro 16 inch M3 Max 36GB/1TB", 89990000m, "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&q=80&w=800"),
            ("Dell XPS 15 OLED Touch 32GB SSD 1TB", 59990000m, "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?auto=format&fit=crop&q=80&w=800"),
            ("Asus ROG Zephyrus G16 OLED 2024", 54990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("Lenovo ThinkPad X1 Carbon Gen 12", 46990000m, "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?auto=format&fit=crop&q=80&w=800"),
            ("HP Spectre x360 14 OLED", 39990000m, "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?auto=format&fit=crop&q=80&w=800"),
            ("MacBook Air 15 inch M3 16GB/512GB", 36990000m, "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?auto=format&fit=crop&q=80&w=800"),
            ("Acer Predator Helios 18 i9-14900HX", 61990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("MSI Raider GE78 HX RTX 4090", 81990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("Razer Blade 16 Dual Mini-LED", 89990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("Microsoft Surface Laptop 6 15 inch", 41990000m, "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?auto=format&fit=crop&q=80&w=800"),
            ("LG Gram 17 2024 Ultra 7", 44990000m, "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?auto=format&fit=crop&q=80&w=800"),
            ("Gigabyte AORUS 17X RTX 4090", 74990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("Alienware m18 R2 i9 RTX 4090", 94990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("Samsung Galaxy Book4 Ultra Touch", 59990000m, "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?auto=format&fit=crop&q=80&w=800"),
            ("Framework Laptop 16 DIY", 34990000m, "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?auto=format&fit=crop&q=80&w=800"),
            ("Dell Alienware x16 R2", 69990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("Asus Zenbook Duo 2024 Màn Hình Kép", 51990000m, "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?auto=format&fit=crop&q=80&w=800"),
            ("Lenovo Yoga Book 9i Dual Screen", 49990000m, "https://images.unsplash.com/photo-1541807084-5c52b6b3adef?auto=format&fit=crop&q=80&w=800"),
            ("HP OMEN 17 Gaming 2024", 46990000m, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"),
            ("MSI Stealth 16 Studio AI", 54990000m, "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?auto=format&fit=crop&q=80&w=800")
        });

        // 3. Tai nghe & Âm thanh (Giá chuẩn VND)
        AddItems(catAudio.Id, new[]
        {
            ("AirPods Pro 2nd Gen USB-C", 5990000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Sony WH-1000XM5 Chống Ồn Cao Cấp", 8490000m, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=800"),
            ("Sennheiser Momentum 4 Wireless", 7990000m, "https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&q=80&w=800"),
            ("Bose QuietComfort Ultra Headphones", 9490000m, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=800"),
            ("Apple AirPods Max Bluetooth", 12990000m, "https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&q=80&w=800"),
            ("Samsung Galaxy Buds2 Pro", 4490000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Sony WF-1000XM5 Tainghe True Wireless", 6490000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("JBL Tour Pro 2 Màn Hình Cảm Ứng", 5490000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Shure AONIC 50 Gen 2", 7990000m, "https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&q=80&w=800"),
            ("Technics EAH-AZ80 Hi-Fi", 6990000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Marshall Monitor II A.N.C", 6990000m, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=800"),
            ("Audio-Technica ATH-M50xBT2", 4490000m, "https://images.unsplash.com/photo-1484704849700-f032a568e944?auto=format&fit=crop&q=80&w=800"),
            ("Bang & Olufsen Beoplay H95 Luxury", 24990000m, "https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&q=80&w=800"),
            ("Bowers & Wilkins Px8 Flagship", 16990000m, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&q=80&w=800"),
            ("Master & Dynamic MW75", 14990000m, "https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&q=80&w=800"),
            ("Jabra Elite 10 Spatial Audio", 5490000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Beats Fit Pro Chống Nước", 4490000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Anker Soundcore Liberty 4 NC", 2290000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Nothing Ear (2) Transparent", 3490000m, "https://images.unsplash.com/photo-1572536147248-ac59a8abfa4b?auto=format&fit=crop&q=80&w=800"),
            ("Loa Marshall Emberton II 20W", 3990000m, "https://images.unsplash.com/photo-1545454675-3531b543be5d?auto=format&fit=crop&q=80&w=800")
        });

        // 4. Đồng hồ thông minh (Giá chuẩn VND)
        AddItems(catWatches.Id, new[]
        {
            ("Apple Watch Ultra 2 Titanium 49mm", 19990000m, "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?auto=format&fit=crop&q=80&w=800"),
            ("Samsung Galaxy Watch6 Classic 47mm", 8990000m, "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?auto=format&fit=crop&q=80&w=800"),
            ("Garmin Fenix 7X Pro Sapphire Solar", 21990000m, "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800"),
            ("Huawei Watch GT 4 Dây Thép", 6490000m, "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&q=80&w=800"),
            ("Apple Watch Series 9 GPS 45mm", 10490000m, "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?auto=format&fit=crop&q=80&w=800"),
            ("Fitbit Sense 2 Theo Dõi Sức Khỏe", 5490000m, "https://images.unsplash.com/photo-1510017803434-a899398421b3?auto=format&fit=crop&q=80&w=800"),
            ("Amazfit Balance Màn Hình AMOLED", 5490000m, "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&q=80&w=800"),
            ("TicWatch Pro 5 Màn Hình Kép", 7990000m, "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?auto=format&fit=crop&q=80&w=800"),
            ("Xiaomi Watch 2 Pro LTE", 6490000m, "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&q=80&w=800"),
            ("Suunto Race GPS Thể Thao", 10990000m, "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800"),
            ("Polar Vantage V3 Chuyên Nghiệp", 14990000m, "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800"),
            ("Garmin Forerunner 965 AMOLED", 14990000m, "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800"),
            ("Google Pixel Watch 2", 7990000m, "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?auto=format&fit=crop&q=80&w=800"),
            ("OnePlus Watch 2 Pin 100 Giờ", 6990000m, "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&q=80&w=800"),
            ("Coros PACE 3 Thể Thao Nhẹ", 5490000m, "https://images.unsplash.com/photo-1510017803434-a899398421b3?auto=format&fit=crop&q=80&w=800"),
            ("Honor Watch GS 4 Sang Trọng", 4490000m, "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&q=80&w=800"),
            ("Noise ColorFit Ultra 3", 2290000m, "https://images.unsplash.com/photo-1510017803434-a899398421b3?auto=format&fit=crop&q=80&w=800"),
            ("Fire-Boltt Invincible AMOLED", 2990000m, "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&q=80&w=800"),
            ("Amazfit T-Rex Ultra Siêu Bền", 8990000m, "https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&q=80&w=800"),
            ("Apple Watch SE 2023 44mm", 6490000m, "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?auto=format&fit=crop&q=80&w=800")
        });

        // 5. Phụ kiện công nghệ (Giá chuẩn VND)
        AddItems(catAccessories.Id, new[]
        {
            ("Củ Sạc Anker Prime 100W GaN 3 Cổng", 1890000m, "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?auto=format&fit=crop&q=80&w=800"),
            ("Bàn Phím Cơ Keychron Q1 Max Wireless", 4990000m, "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&q=80&w=800"),
            ("Chuột Logitech MX Master 3S Quiet", 2390000m, "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&q=80&w=800"),
            ("Pin Dự Phòng Anker 737 24000mAh 140W", 3490000m, "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?auto=format&fit=crop&q=80&w=800"),
            ("Đế Sạc Không Dây Belkin BoostCharge Pro", 2890000m, "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?auto=format&fit=crop&q=80&w=800"),
            ("Cáp Ugreen Thunderbolt 4 100W 40Gbps", 890000m, "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&q=80&w=800"),
            ("Hub Chuyển Đổi Satechi USB-C 10-in-1", 2290000m, "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&q=80&w=800"),
            ("Bàn Di Chuột Razer Strata Chroma RGB", 1190000m, "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&q=80&w=800"),
            ("Bút Cảm Ứng Apple Pencil Pro 2024", 3190000m, "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?auto=format&fit=crop&q=80&w=800"),
            ("Tay Cầm Xbox Wireless Controller Special", 1590000m, "https://images.unsplash.com/photo-1600080972464-8e5f35f63d08?auto=format&fit=crop&q=80&w=800"),
            ("Tay Cầm Sony DualSense Edge PS5", 4890000m, "https://images.unsplash.com/photo-1600080972464-8e5f35f63d08?auto=format&fit=crop&q=80&w=800"),
            ("Thẻ Nhớ SanDisk Extreme Pro 1TB 200MB/s", 3190000m, "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&q=80&w=800"),
            ("Ổ Cứng SSD M.2 NVMe Samsung 990 Pro 2TB", 4590000m, "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&q=80&w=800"),
            ("Giá Đỡ Laptop Nulaxy Xoay 360 Độ", 890000m, "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?auto=format&fit=crop&q=80&w=800"),
            ("Đèn Treo Màn Hình BenQ ScreenBar Halo", 4290000m, "https://images.unsplash.com/photo-1565814636199-ae8133055c1c?auto=format&fit=crop&q=80&w=800"),
            ("Bàn Phím Apple Magic Keyboard Touch ID", 4290000m, "https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&q=80&w=800"),
            ("Chuột Gaming Razer DeathAdder V3 Pro", 3490000m, "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&q=80&w=800"),
            ("Pin Sạc Baseus Blade 100W Siêu Mỏng", 1890000m, "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?auto=format&fit=crop&q=80&w=800"),
            ("Túi Chống Sốc Tomtoc 15.6 inch", 890000m, "https://images.unsplash.com/photo-1588872657578-7efd1f1555ed?auto=format&fit=crop&q=80&w=800"),
            ("Củ Sạc Siêu Nhanh Apple 140W USB-C", 2390000m, "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?auto=format&fit=crop&q=80&w=800")
        });

        // 6. Thiết bị nhà thông minh (Giá chuẩn VND)
        AddItems(catSmartHome.Id, new[]
        {
            ("Robot Hút Bụi Roborock S8 MaxV Ultra", 31990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Camera An Ninh Aqara G3 AI Hub 2K", 2490000m, "https://images.unsplash.com/photo-1557324232-b8917d3c3dcb?auto=format&fit=crop&q=80&w=800"),
            ("Đèn Philips Hue Gradient Lightstrip", 4290000m, "https://images.unsplash.com/photo-1565814636199-ae8133055c1c?auto=format&fit=crop&q=80&w=800"),
            ("Loa Thông Minh Apple HomePod 2nd Gen", 7290000m, "https://images.unsplash.com/photo-1589003077984-894e133dabab?auto=format&fit=crop&q=80&w=800"),
            ("Công Tắc Thông Minh Tuya Zigbee 4 Nút", 690000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Khóa Cửa Thông Minh Yale Lunar Pro", 11990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Máy Lọc Không Khí Xiaomi Smart Air Purifier 4", 4990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Chuông Cửa Thông Minh Google Nest Doorbell", 4290000m, "https://images.unsplash.com/photo-1557324232-b8917d3c3dcb?auto=format&fit=crop&q=80&w=800"),
            ("Cảm Biến Hiện Diện Aqara FP2 Radar", 1990000m, "https://images.unsplash.com/photo-1557324232-b8917d3c3dcb?auto=format&fit=crop&q=80&w=800"),
            ("Robot Lau Nhà Ecovacs Deebot X2 Omni", 28990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Loa Thông Minh Sonos Era 300 Dolby Atmos", 10990000m, "https://images.unsplash.com/photo-1589003077984-894e133dabab?auto=format&fit=crop&q=80&w=800"),
            ("Đèn LED Trang Trí Nanoleaf Lines RGB", 4890000m, "https://images.unsplash.com/photo-1565814636199-ae8133055c1c?auto=format&fit=crop&q=80&w=800"),
            ("Máy Chiếu XGIMI Horizon Ultra 4K", 39990000m, "https://images.unsplash.com/photo-1589003077984-894e133dabab?auto=format&fit=crop&q=80&w=800"),
            ("Công Tắc Tự Động SwitchBot Bot 3", 690000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Khóa Vân Tay Kaadas K9 Cao Cấp", 8990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Cảm Biến Báo Khói Google Nest Protect", 2890000m, "https://images.unsplash.com/photo-1557324232-b8917d3c3dcb?auto=format&fit=crop&q=80&w=800"),
            ("Máy Tạo Độ Ẩm Dyson Purifier Humidify", 21990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Động Cơ Rèm Tự Động SwitchBot Curtain 3", 2090000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Robot Hút Bụi Lau Nhà Dreame L20 Ultra", 26990000m, "https://images.unsplash.com/photo-1558002038-1055907df827?auto=format&fit=crop&q=80&w=800"),
            ("Trung Tâm Điều Khiển Aqara Hub M3", 2990000m, "https://images.unsplash.com/photo-1557324232-b8917d3c3dcb?auto=format&fit=crop&q=80&w=800")
        });

        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync();

        // 3. Flash Sale Campaign
        var flashSaleProducts = products.Take(6).ToList();
        var campaign = new FlashSaleCampaign
        {
            Id = Guid.NewGuid(),
            Name = "Siêu Sale Bùng Nổ 2026",
            StartTime = DateTime.UtcNow.AddHours(-2),
            EndTime = DateTime.UtcNow.AddDays(7),
            Status = "Active"
        };

        dbContext.FlashSaleCampaigns.Add(campaign);

        foreach (var prod in flashSaleProducts)
        {
            dbContext.FlashSaleItems.Add(new FlashSaleItem
            {
                CampaignId = campaign.Id,
                ProductId = prod.Id,
                FlashSalePrice = prod.Price >= 30000000m ? prod.Price - 1000000m : Math.Round(prod.Price * 0.75m, 2),
                FlashSaleQuantity = 50,
                SoldQuantity = 12
            });
        }

        await dbContext.SaveChangesAsync();
    }
}
