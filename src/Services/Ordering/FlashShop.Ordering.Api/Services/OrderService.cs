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
    private readonly FlashShop.MessageContracts.Protos.WalletGrpc.WalletGrpcClient _walletGrpcClient;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        OrderingDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        IHttpClientFactory httpClientFactory,
        FlashShop.MessageContracts.Protos.WalletGrpc.WalletGrpcClient walletGrpcClient,
        ILogger<OrderService> logger)
    {
        _dbContext = dbContext;
        _publishEndpoint = publishEndpoint;
        _identityClient = httpClientFactory.CreateClient("IdentityService");
        _walletGrpcClient = walletGrpcClient;
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
        var isWalletPayment = string.IsNullOrWhiteSpace(request.PaymentMethod) 
            || request.PaymentMethod.Contains("Ví FlashPay") 
            || request.PaymentMethod.Contains("Wallet");

        // 1. If paying by Wallet, verify and deduct wallet balance FIRST via gRPC (with REST fallback)
        if (isWalletPayment)
        {
            try
            {
                _logger.LogInformation("Executing gRPC Wallet Payment for Order {OrderNumber}, UserId: {UserId}", orderNumber, userId);
                var grpcReq = new FlashShop.MessageContracts.Protos.WalletPaymentRequest
                {
                    UserId = userId.ToString(),
                    Amount = (double)totalAmount,
                    OrderId = orderId.ToString(),
                    Description = $"Thanh toán đơn hàng {orderNumber}"
                };

                var grpcRes = await _walletGrpcClient.PayWithWalletAsync(grpcReq);
                if (!grpcRes.IsSuccess)
                {
                    throw new BadRequestException(!string.IsNullOrWhiteSpace(grpcRes.Message) ? grpcRes.Message : "Số dư ví FlashPay không đủ để thanh toán. Vui lòng nạp thêm tiền vào ví!");
                }
            }
            catch (Exception ex) when (ex is not BadRequestException)
            {
                _logger.LogWarning(ex, "gRPC Wallet Payment failed/unavailable. Falling back to HTTP REST call for Order {OrderNumber}", orderNumber);
                var paymentRequest = new
                {
                    UserId = userId,
                    Amount = totalAmount,
                    OrderId = orderId.ToString(),
                    Description = $"Thanh toán đơn hàng {orderNumber}"
                };

                var payResponse = await _identityClient.PostAsJsonAsync("/api/wallets/pay", paymentRequest);
                if (!payResponse.IsSuccessStatusCode)
                {
                    var errorContent = await payResponse.Content.ReadAsStringAsync();
                    var errorMessage = "Số dư ví FlashPay không đủ để thanh toán. Vui lòng nạp thêm tiền vào ví!";
                    try
                    {
                        var apiErr = System.Text.Json.JsonSerializer.Deserialize<FlashShop.Shared.Common.ApiResponse<object>>(
                            errorContent, 
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                        if (!string.IsNullOrWhiteSpace(apiErr?.Message))
                        {
                            errorMessage = apiErr.Message;
                        }
                    }
                    catch { }

                    throw new BadRequestException(errorMessage);
                }
            }
        }

        // 2. Create Order record with proper status
        var order = new Order
        {
            Id = orderId,
            OrderNumber = orderNumber,
            UserId = userId,
            UserEmail = userEmail,
            Status = isWalletPayment ? OrderStatus.Paid : OrderStatus.Pending,
            TotalAmount = totalAmount,
            ShippingAddress = request.ShippingAddress,
            RecipientName = request.RecipientName,
            RecipientPhone = request.RecipientPhone,
            PaymentMethod = isWalletPayment ? "Ví FlashPay" : request.PaymentMethod,
            IsFlashSaleOrder = request.IsFlashSaleOrder,
            Notes = request.Notes,
            PaymentDeadline = isWalletPayment ? null : null, // COD and Wallet orders do not expire
            PaidAt = isWalletPayment ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var cartItem in cartItems)
        {
            var itemImg = !string.IsNullOrWhiteSpace(cartItem.ImageUrl) && !cartItem.ImageUrl.Contains("example.com")
                ? cartItem.ImageUrl
                : (cartItem.ProductName.Contains("Acer") || cartItem.ProductName.Contains("Helios") || cartItem.ProductName.Contains("Laptop")
                    ? "https://images.unsplash.com/photo-1603302576837-37561b2e2302?auto=format&fit=crop&q=80&w=800"
                    : (cartItem.ProductName.Contains("SteelSeries") || cartItem.ProductName.Contains("Apex") || cartItem.ProductName.Contains("Bàn phím")
                        ? "https://images.unsplash.com/photo-1595225476474-87563907a212?auto=format&fit=crop&q=80&w=800"
                        : (cartItem.ProductName.Contains("iPhone") || cartItem.ProductName.Contains("Phone")
                            ? "https://images.unsplash.com/photo-1695048133142-1a20484d2569?auto=format&fit=crop&q=80&w=800"
                            : (cartItem.ProductName.Contains("AirPods") || cartItem.ProductName.Contains("Tai nghe") || cartItem.ProductName.Contains("Arctis")
                                ? "https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&q=80&w=800"
                                : "https://images.unsplash.com/photo-1546868871-7041f2a55e12?auto=format&fit=crop&q=80&w=800"))));

            order.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.ProductName,
                UnitPrice = cartItem.UnitPrice,
                Quantity = cartItem.Quantity,
                SubTotal = cartItem.UnitPrice * cartItem.Quantity,
                ImageUrl = itemImg
            });
        }

        _dbContext.Orders.Add(order);
        _dbContext.CartItems.RemoveRange(cartItems);
        await _dbContext.SaveChangesAsync();

        // Deduct inventory stock quantity in CatalogDb for purchased items
        foreach (var cartItem in cartItems)
        {
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync(
                    "USE FlashShop_CatalogDb; UPDATE Products SET StockQuantity = CASE WHEN StockQuantity - {0} < 0 THEN 0 ELSE StockQuantity - {0} END WHERE Id = {1}; UPDATE FlashSaleItems SET SoldQuantity = SoldQuantity + {0} WHERE ProductId = {1}; USE FlashShop_OrderingDb;",
                    cartItem.Quantity, cartItem.ProductId
                );
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Notice: Stock quantity update notice for product {ProductId}", cartItem.ProductId);
            }
        }

        // 3. Publish events to MassTransit
        if (isWalletPayment)
        {
            await _publishEndpoint.Publish(new OrderPaidEvent
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                UserId = userId,
                UserEmail = userEmail,
                TotalAmount = totalAmount
            });
        }
        else
        {
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
        }

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

        order.Status = OrderStatus.Paid;
        order.PaidAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        try
        {
            var paymentRequest = new
            {
                UserId = userId,
                Amount = order.TotalAmount,
                OrderId = order.Id.ToString(),
                Description = $"Payment for order {order.OrderNumber}"
            };
            await _identityClient.PostAsJsonAsync("/api/wallets/pay", paymentRequest);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Wallet deduction failed in PayOrderAsync, preserving Paid status");
        }

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
        if (status is OrderStatus.Completed or OrderStatus.Paid)
        {
            order.PaidAt ??= DateTime.UtcNow;
        }
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
