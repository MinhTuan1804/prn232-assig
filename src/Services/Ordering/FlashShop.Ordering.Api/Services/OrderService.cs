using FlashShop.MessageContracts.Events;
using FlashShop.Ordering.Api.Data;
using FlashShop.Ordering.Api.DTOs.Requests;
using FlashShop.Ordering.Api.DTOs.Responses;
using FlashShop.Ordering.Api.Entities;
using FlashShop.Shared.Common;
using FlashShop.Shared.Constants;
using FlashShop.Shared.Exceptions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FlashShop.Ordering.Api.Services;

public class OrderService : IOrderService
{
    private readonly OrderingDbContext _dbContext;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly HttpClient _identityClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        OrderingDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        IHttpClientFactory httpClientFactory,
        ILogger<OrderService> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _identityClient = httpClientFactory.CreateClient("IdentityService");
        _logger = logger;
    }

    public async Task<OrderResponse> CheckoutAsync(Guid userId, string userEmail, CheckoutRequest request)
    {
        var cartItems = await _dbContext.CartItems
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0)
            throw new BadRequestException("Cart is empty.");

        var orderId = Guid.NewGuid();
        var orderNumber = $"FS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        var totalAmount = cartItems.Sum(c => c.UnitPrice * c.Quantity);
        var paymentDeadline = DateTime.UtcNow.AddMinutes(15);

        var order = new Order
        {
            Id = orderId,
            OrderNumber = orderNumber,
            UserId = userId,
            UserEmail = userEmail,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            ShippingAddress = request.ShippingAddress,
            RecipientName = request.RecipientName,
            RecipientPhone = request.RecipientPhone,
            PaymentMethod = string.IsNullOrWhiteSpace(request.PaymentMethod) ? "FlashPay Wallet" : request.PaymentMethod,
            IsFlashSaleOrder = request.IsFlashSaleOrder,
            Notes = request.Notes,
            PaymentDeadline = paymentDeadline,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var cartItem in cartItems)
        {
            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.ProductName,
                UnitPrice = cartItem.UnitPrice,
                Quantity = cartItem.Quantity,
                SubTotal = cartItem.UnitPrice * cartItem.Quantity,
                ImageUrl = cartItem.ImageUrl
            });
        }

        _dbContext.Orders.Add(order);
        _dbContext.CartItems.RemoveRange(cartItems);
        await _dbContext.SaveChangesAsync();

        await _publishEndpoint.Publish(new OrderCreatedEvent
        {
            OrderId = orderId,
            OrderNumber = orderNumber,
            UserId = userId,
            UserEmail = userEmail,
            IsFlashSaleOrder = request.IsFlashSaleOrder,
            Items = cartItems.Select(c => new OrderItemDetail
            {
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                Quantity = c.Quantity,
                UnitPrice = c.UnitPrice
            }).ToList()
        });

        return MapToResponse(order);
    }

    public async Task<PagedResult<OrderResponse>> GetMyOrdersAsync(Guid userId, int page, int pageSize)
    {
        var query = _dbContext.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .AsNoTracking();

        var totalCount = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderResponse>
        {
            Items = orders.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<OrderResponse> GetOrderByIdAsync(Guid orderId, Guid? userId = null)
    {
        var query = _dbContext.Orders.Include(o => o.Items).AsNoTracking();
        if (userId.HasValue)
            query = query.Where(o => o.UserId == userId.Value);

        var order = await query.FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException("Order", orderId);

        return MapToResponse(order);
    }

    public async Task<OrderResponse> PayOrderAsync(Guid orderId, Guid userId)
    {
        var order = await _dbContext.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
            ?? throw new NotFoundException("Order", orderId);

        if (order.Status != OrderStatus.AwaitingPayment)
            throw new BadRequestException($"Order cannot be paid in status '{order.Status}'.");

        if (order.PaymentDeadline.HasValue && order.PaymentDeadline.Value < DateTime.UtcNow)
            throw new BadRequestException("Payment deadline has expired.");

        var paymentRequest = new
        {
            UserId = userId,
            Amount = order.TotalAmount,
            OrderId = order.Id.ToString(),
            Description = $"Payment for order {order.OrderNumber}"
        };

        var response = await _identityClient.PostAsJsonAsync("/api/wallets/pay", paymentRequest);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new BadRequestException($"Payment failed: {error}");
        }

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        await _publishEndpoint.Publish(new OrderPaidEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = userId,
            UserEmail = order.UserEmail,
            TotalAmount = order.TotalAmount
        });

        return MapToResponse(order);
    }

    public async Task<OrderResponse> CancelOrderAsync(Guid orderId, Guid userId, string? reason)
    {
        var order = await _dbContext.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId)
            ?? throw new NotFoundException("Order", orderId);

        if (order.Status is OrderStatus.Paid or OrderStatus.Shipping or OrderStatus.Completed)
            throw new BadRequestException($"Order cannot be cancelled in status '{order.Status}'.");

        order.Status = OrderStatus.Cancelled;
        order.CancelledAt = DateTime.UtcNow;
        order.CancelReason = reason ?? "Cancelled by user";
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        await _publishEndpoint.Publish(new OrderCancelledEvent
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            UserEmail = order.UserEmail,
            Reason = order.CancelReason
        });

        return MapToResponse(order);
    }

    public async Task<OrderResponse> UpdateStatusAsync(Guid orderId, string status)
    {
        var order = await _dbContext.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new NotFoundException("Order", orderId);

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        return MapToResponse(order);
    }

    public async Task<PagedResult<OrderResponse>> GetAllOrdersAsync(int page, int pageSize, string? status)
    {
        var query = _dbContext.Orders.Include(o => o.Items).AsNoTracking();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.Status == status);

        var totalCount = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<OrderResponse>
        {
            Items = orders.Select(MapToResponse).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task CancelExpiredOrdersAsync()
    {
        var expiredOrders = await _dbContext.Orders
            .Where(o => o.Status == OrderStatus.AwaitingPayment
                     && o.PaymentDeadline < DateTime.UtcNow)
            .ToListAsync();

        foreach (var order in expiredOrders)
        {
            order.Status = OrderStatus.Cancelled;
            order.CancelledAt = DateTime.UtcNow;
            order.CancelReason = "Payment deadline exceeded";
            order.UpdatedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(new OrderCancelledEvent
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                UserEmail = order.UserEmail,
                Reason = "Payment deadline exceeded"
            });
        }

        if (expiredOrders.Count > 0)
        {
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Cancelled {Count} expired orders", expiredOrders.Count);
        }
    }

    private static OrderResponse MapToResponse(Order order) => new()
    {
        Id = order.Id,
        OrderNumber = order.OrderNumber,
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        ShippingAddress = order.ShippingAddress,
        RecipientName = order.RecipientName,
        RecipientPhone = order.RecipientPhone,
        PaymentMethod = order.PaymentMethod,
        IsFlashSaleOrder = order.IsFlashSaleOrder,
        Notes = order.Notes,
        PaymentDeadline = order.PaymentDeadline,
        PaidAt = order.PaidAt,
        CancelledAt = order.CancelledAt,
        CancelReason = order.CancelReason,
        CreatedAt = order.CreatedAt,
        Items = order.Items.Select(i => new OrderItemResponse
        {
            ProductId = i.ProductId,
            ProductName = i.ProductName,
            UnitPrice = i.UnitPrice,
            Quantity = i.Quantity,
            SubTotal = i.SubTotal,
            ImageUrl = i.ImageUrl
        }).ToList()
    };
}
