from PIL import Image, ImageDraw, ImageFont
import os

def draw_rounded_card(draw, rect, fill_color, border_color, title, subtitle="", radius=12, width=2, title_color='#0F2043', sub_color='#475569', font_title=None, font_sub=None):
    x1, y1, x2, y2 = rect
    # Shadow
    draw.rounded_rectangle([x1+3, y1+3, x2+3, y2+3], radius=radius, fill='#E2E8F0')
    # Card
    draw.rounded_rectangle([x1, y1, x2, y2], radius=radius, fill=fill_color, outline=border_color, width=width)
    
    if title:
        ty = y1 + (22 if subtitle else (y2 - y1)//2)
        draw.text((x1 + (x2 - x1)//2, ty), title, font=font_title, fill=title_color, anchor="mm")
    if subtitle:
        draw.text((x1 + (x2 - x1)//2, y1 + 46), subtitle, font=font_sub, fill=sub_color, anchor="mm")

def draw_arrow(draw, p1, p2, color='#0284C7', width=3, arrow_size=8, label="", font_label=None):
    x1, y1 = p1
    x2, y2 = p2
    draw.line([p1, p2], fill=color, width=width)
    
    # Arrow head
    import math
    angle = math.atan2(y2 - y1, x2 - x1)
    px1 = x2 - arrow_size * math.cos(angle - math.pi / 6)
    py1 = y2 - arrow_size * math.sin(angle - math.pi / 6)
    px2 = x2 - arrow_size * math.cos(angle + math.pi / 6)
    py2 = y2 - arrow_size * math.sin(angle + math.pi / 6)
    draw.polygon([(x2, y2), (px1, py1), (px2, py2)], fill=color)

    if label:
        mx, my = (x1 + x2) // 2, (y1 + y2) // 2
        draw.text((mx, my - 12), label, font=font_label, fill=color, anchor="mm")

def draw_database(draw, center_x, top_y, width, height, fill_color, border_color, name, font_title=None, font_sub=None):
    x1 = center_x - width // 2
    x2 = center_x + width // 2
    y1 = top_y
    y2 = top_y + height
    
    # Draw cylinder base body
    draw.rectangle([x1, y1 + 12, x2, y2 - 12], fill=fill_color, outline=border_color, width=2)
    # Bottom ellipse
    draw.ellipse([x1, y2 - 24, x2, y2], fill=fill_color, outline=border_color, width=2)
    # Top ellipse
    draw.ellipse([x1, y1, x2, y1 + 24], fill=fill_color, outline=border_color, width=2)
    # Label
    draw.text((center_x, y1 + height // 2 + 2), name, font=font_title, fill='#1E293B', anchor="mm")

def generate_v2_diagram(output_path):
    w, h = 1800, 1200
    img = Image.new('RGB', (w, h), color='#F8FAFC')
    draw = ImageDraw.Draw(img)

    try:
        font_main_title = ImageFont.truetype("arial.ttf", 34)
        font_sub_header = ImageFont.truetype("arial.ttf", 18)
        font_header = ImageFont.truetype("arialbd.ttf", 22)
        font_box_b = ImageFont.truetype("arialbd.ttf", 16)
        font_box_n = ImageFont.truetype("arial.ttf", 14)
        font_small_i = ImageFont.truetype("ariali.ttf", 12)
        font_small_b = ImageFont.truetype("arialbd.ttf", 12)
    except:
        font_main_title = font_sub_header = font_header = font_box_b = font_box_n = font_small_i = font_small_b = ImageFont.load_default()

    # Header Banner
    draw.rectangle([(0, 0), (w, 100)], fill='#0F2043')
    draw.text((w // 2, 35), "FLASCSHOP - CONCEPTUAL ARCHITECTURE DIAGRAM (COMPLETE)", font=font_main_title, fill='#FFFFFF', anchor="mm")
    draw.text((w // 2, 75), "Comprehensive Microservices Topology, Gateway Routes, DBs, Hangfire & Event Bus", font=font_sub_header, fill='#94A3B8', anchor="mm")

    # 1. CLIENTS (Left)
    draw_rounded_card(draw, (60, 200, 260, 1050), '#F0F9FF', '#0284C7', "Clients Layer", font_title=font_header, width=3)
    clients = [
        ("Web Browser (React/Vue)", (80, 260, 240, 360), '#FFFFFF', '#0EA5E9'),
        ("Mobile App (iOS/Android)", (80, 430, 240, 530), '#FFFFFF', '#0EA5E9'),
        ("Admin Portal (Dashboard)", (80, 600, 240, 700), '#FFFFFF', '#0EA5E9'),
        ("External API Clients", (80, 770, 240, 870), '#FFFFFF', '#0EA5E9'),
    ]
    for cname, crect, cbg, cbd in clients:
        draw_rounded_card(draw, crect, cbg, cbd, cname, font_title=font_box_b, font_sub=font_small_i)

    # 2. API GATEWAY (Middle Left)
    draw_rounded_card(draw, (360, 200, 560, 1050), '#FEF3C7', '#D97706', "API Gateway", "YARP Reverse Proxy\nJWT Validation\nSwagger Aggregator\nHealth Monitoring", width=3, font_title=font_header, font_sub=font_box_n)

    # 3. CORE SERVICES BOUNDARY (Middle Container)
    draw.rounded_rectangle([640, 140, 1380, 1120], radius=20, fill='#FFFFFF', outline='#22C55E', width=4)
    draw.text((670, 165), "FlashShop Microservices Boundary", font=font_header, fill='#15803D')

    # 5 Microservices + Databases
    services_spec = [
        # (Name, DB_Name, Y_pos, HasHangfire, Color_Fill, Color_Border)
        ("Catalog Service", "CatalogDB", 200, False, '#F0FDF4', '#16A34A'),
        ("Inventory Service", "InventoryDB", 380, False, '#FFF7ED', '#EA580C'),
        ("Notification Service", "NotificationDB", 560, True, '#FEF2F2', '#EF4444'),
        ("Ordering Service", "OrderingDB", 740, True, '#EEF2FF', '#6366F1'),
        ("Identity Service", "IdentityDB", 920, False, '#F0FDF4', '#0D9488')
    ]

    gw_x_out = 560
    svc_x_in = 670

    for sname, dbname, sy, hashf, sfill, sborder in services_spec:
        # Microservice Box
        s_rect = (670, sy, 980, sy + 130)
        draw_rounded_card(draw, s_rect, sfill, sborder, sname, font_title=font_header)
        
        # Hangfire sub-badge inside Notification & Ordering
        if hashf:
            hf_rect = (730, sy + 65, 920, sy + 115)
            draw_rounded_card(draw, hf_rect, '#FFFFFF', '#DC2626', "⚡ Hangfire Worker", "Background Recurring Jobs", font_title=font_small_b, font_sub=font_small_i, width=1)
        else:
            draw.text((825, sy + 75), "Core Business APIs", font=font_small_i, fill='#475569', anchor="mm")

        # Arrow from Gateway to Microservice
        draw_arrow(draw, (gw_x_out, sy + 65), (svc_x_in, sy + 65), color='#D97706', width=2)

        # DB Connection
        db_center_x = 1150
        draw_database(draw, db_center_x, sy + 10, 150, 110, sfill, sborder, dbname, font_title=font_box_b)
        draw_arrow(draw, (980, sy + 65), (db_center_x - 75, sy + 65), color=sborder, width=2)

    # Direct Synchronous REST Call: Ordering -> Identity (Wallet)
    # Draw curved arrow from Ordering (sy=740) down to Identity (sy=920)
    draw.line([(670, 805), (620, 805), (620, 985), (670, 985)], fill='#6366F1', width=3)
    draw_arrow(draw, (620, 985), (670, 985), color='#6366F1', width=3)
    draw.text((605, 895), "REST HTTP\nWallet Payment", font=font_small_b, fill='#6366F1', anchor="mm")

    # 4. RABBITMQ MESSAGE BROKER (Right)
    draw_rounded_card(draw, (1460, 200, 1740, 1050), '#FFF7ED', '#F97316', "RabbitMQ\n(MassTransit)", "Asynchronous Event Bus\nExchange & Routing Queues\nMessage Distribution", width=4, font_title=font_header, font_sub=font_box_n)

    # Bi-directional arrows between Services and RabbitMQ
    # Inventory (sy=380), Notification (sy=560), Ordering (sy=740)
    event_services_y = [445, 625, 805]
    for ey in event_services_y:
        # Service -> RabbitMQ (Publish)
        draw_arrow(draw, (1380, ey - 15), (1460, ey - 15), color='#EA580C', width=3, label="Publish", font_label=font_small_b)
        # RabbitMQ -> Service (Consume)
        draw_arrow(draw, (1460, ey + 15), (1380, ey + 15), color='#16A34A', width=3, label="Consume", font_label=font_small_b)

    # Client -> Gateway arrows
    for y_c in [310, 480, 650, 820]:
        draw_arrow(draw, (260, y_c), (360, 525), color='#0284C7', width=2)

    # Footer note
    draw.text((w // 2, 1160), "Fully Aligned with FlashShop Solution Architecture (.NET 8, YARP, EF Core, MassTransit/RabbitMQ, Hangfire)", font=font_small_i, fill='#64748B', anchor="mm")

    img.save(output_path, "PNG")
    print(f"Updated diagram PNG saved at: {output_path}")

if __name__ == "__main__":
    generate_v2_diagram(r"e:\Ki 8\PRN232\PRN232-ASSIGMENT\prn232-assig\scratch\conceptual_architecture_diagram_v2.png")
