from PIL import Image, ImageDraw, ImageFont
import os

def create_conceptual_diagram(output_path):
    # Image dimensions
    width, height = 1600, 1100
    img = Image.new('RGB', (width, height), color='#FAF8F5') # Warm off-white background
    draw = ImageDraw.Draw(img)

    # Load fonts
    try:
        font_title = ImageFont.truetype("arial.ttf", 32)
        font_subtitle = ImageFont.truetype("arial.ttf", 18)
        font_header = ImageFont.truetype("arialbd.ttf", 22)
        font_box_title = ImageFont.truetype("arialbd.ttf", 18)
        font_text = ImageFont.truetype("arial.ttf", 14)
        font_subtext = ImageFont.truetype("ariali.ttf", 12)
    except:
        font_title = font_subtitle = font_header = font_box_title = font_text = font_subtext = ImageFont.load_default()

    # Draw Header Box
    draw.rectangle([(0, 0), (width, 90)], fill='#0F2043') # Deep Navy
    draw.text((width // 2, 30), "FLASCSHOP - CONCEPTUAL ARCHITECTURE DIAGRAM", font=font_title, fill='#FFFFFF', anchor="mm")
    draw.text((width // 2, 65), "High-Level System Boundaries, Core Microservices & External Infrastructure", font=font_subtitle, fill='#CBD5E1', anchor="mm")

    # Helper function for rounded rectangles with shadow
    def draw_card(rect, fill_color, border_color, title, subtitle="", radius=15):
        x1, y1, x2, y2 = rect
        # Draw shadow
        draw.rounded_rectangle([x1+4, y1+4, x2+4, y2+4], radius=radius, fill='#E2E8F0')
        # Draw main card
        draw.rounded_rectangle([x1, y1, x2, y2], radius=radius, fill=fill_color, outline=border_color, width=2)
        # Title & Subtitle
        if title:
            draw.text((x1 + (x2 - x1)//2, y1 + 22), title, font=font_box_title, fill='#0F2043', anchor="mm")
        if subtitle:
            draw.text((x1 + (x2 - x1)//2, y1 + 46), subtitle, font=font_subtext, fill='#475569', anchor="mm")

    # 1. External Actors (Top Layer)
    draw_card((100, 130, 450, 210), '#E0F2FE', '#0284C7', "Web & Mobile Clients", "Customers & Admin Users")
    draw_card((1150, 130, 1500, 210), '#F0FDF4', '#16A34A', "External Services", "Payment Gateways & Mailers")

    # 2. Main System Boundary (Middle Large Box)
    draw.rounded_rectangle([80, 250, 1520, 840], radius=20, fill='#FFFFFF', outline='#94A3B8', width=3)
    draw.text((120, 275), "CORE SYSTEM BOUNDARY: FlashShop E-Commerce Platform", font=font_header, fill='#1E293B')

    # Gateway Layer (Inside System)
    draw_card((120, 310, 1480, 385), '#FEF3C7', '#D97706', "API Gateway (YARP Reverse Proxy & Aggregated Swagger)", "Central Request Routing, JWT Validation & System Health Monitoring")

    # Microservices Grid (Inside System Boundary)
    services = [
        ("Identity & Wallet Service", "Auth, Users, Wallets", (120, 430, 540, 530), '#EFF6FF', '#2563EB'),
        ("Catalog & FlashSale Service", "Products, Flash Sale Campaigns", (560, 430, 1040, 530), '#EFF6FF', '#2563EB'),
        ("Inventory Service", "Stock Management & Reservations", (1060, 430, 1480, 530), '#EFF6FF', '#2563EB'),
        ("Ordering & Cart Service", "Shopping Cart & Order Checkout", (340, 560, 840, 660), '#EEF2FF', '#4F46E5'),
        ("Notification & Job Service", "User Notifications & Quartz Reports", (860, 560, 1360, 660), '#EEF2FF', '#4F46E5')
    ]

    for title, sub, rect, fill, border in services:
        draw_card(rect, fill, border, title, sub)

    # Inter-Service Communication Sub-Boundary inside Core System
    draw.rounded_rectangle([120, 690, 1480, 810], radius=12, fill='#F8FAFC', outline='#CBD5E1', width=2)
    draw.text((140, 710), "Inter-Service Communication Fabric", font=font_box_title, fill='#334155')
    draw.text((140, 735), "• Asynchronous Event-Driven Messaging: MassTransit over RabbitMQ Event Bus", font=font_text, fill='#475569')
    draw.text((140, 765), "• Synchronous REST API: Direct HttpClient calls (Order -> Identity, Notification -> Services)", font=font_text, fill='#475569')

    # 3. Data & Infrastructure Layer (Bottom Layer)
    draw_card((100, 880, 750, 990), '#FAF5FF', '#9333EA', "MS SQL Server 2022 Cluster", "Database-per-service (IdentityDb, CatalogDb, InventoryDb, OrderingDb, NotificationDb)")
    draw_card((850, 880, 1500, 990), '#FFF7ED', '#EA580C', "RabbitMQ Message Broker", "Exchanges, Routing Queues & Event Handlers")

    # Arrows / Connecting Lines
    # Clients -> Gateway
    draw.line([(275, 210), (275, 310)], fill='#0284C7', width=3)
    draw.polygon([(270, 305), (280, 305), (275, 315)], fill='#0284C7')
    draw.text((290, 245), "HTTPS Request", font=font_text, fill='#0284C7')

    # Gateway -> Microservices
    draw.line([(800, 385), (800, 430)], fill='#D97706', width=3)
    draw.polygon([(795, 425), (805, 425), (800, 435)], fill='#D97706')

    # Services -> Inter-service Fabric
    draw.line([(800, 660), (800, 690)], fill='#4F46E5', width=3)

    # Core System -> Infrastructure
    draw.line([(425, 840), (425, 880)], fill='#9333EA', width=3)
    draw.polygon([(420, 875), (430, 875), (425, 885)], fill='#9333EA')
    draw.text((435, 850), "EF Core Sql Data Access", font=font_subtext, fill='#9333EA')

    draw.line([(1175, 840), (1175, 880)], fill='#EA580C', width=3)
    draw.polygon([(1170, 875), (1180, 875), (1175, 885)], fill='#EA580C')
    draw.text((1185, 850), "Publish & Consume Events", font=font_subtext, fill='#EA580C')

    # Footer
    draw.text((width // 2, 1050), "Conceptual Architecture Diagram based on GeeksforGeeks System Design Standard", font=font_subtext, fill='#64748B', anchor="mm")

    img.save(output_path, "PNG")
    print(f"Diagram saved at: {output_path}")

if __name__ == "__main__":
    create_conceptual_diagram(r"e:\Ki 8\PRN232\PRN232-ASSIGMENT\prn232-assig\scratch\conceptual_architecture_diagram.png")
