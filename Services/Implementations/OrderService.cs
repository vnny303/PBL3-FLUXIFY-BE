using FluxifyAPI.Data;
using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Common;
using FluxifyAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FluxifyAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private const decimal StandardShippingFee = 15000m;
        private const decimal ExpressShippingFee = 30000m;
        private const string StatusPending = "Pending";
        private const string StatusConfirmed = "Confirmed";
        private const string StatusProcessing = "Processing";
        private const string StatusShipping = "Shipping";
        private const string StatusShipped = "Shipped";
        private const string StatusDelivered = "Delivered";
        private const string StatusCompleted = "Completed";
        private const string StatusCancelled = "Cancelled";
        private const string PaymentStatusPending = "Pending";
        private const string PaymentStatusPaid = "paid";
        private const string PaymentMethodCod = "COD";

        private static readonly Dictionary<string, string> CanonicalStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            ["pending"] = StatusPending,
            ["confirmed"] = StatusConfirmed,
            ["processing"] = StatusProcessing,
            ["shipping"] = StatusShipping,
            ["shipped"] = StatusShipped,
            ["delivered"] = StatusDelivered,
            ["completed"] = StatusCompleted,
            ["cancelled"] = StatusCancelled,
            ["canceled"] = StatusCancelled
        };

        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerAddressRepository _customerAddressRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ICartRepository _cartRepository;

        public OrderService(
            AppDbContext context,
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            ICustomerAddressRepository customerAddressRepository,
            ITenantRepository tenantRepository,
            ICartRepository cartRepository)
        {
            _context = context;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _customerAddressRepository = customerAddressRepository;
            _tenantRepository = tenantRepository;
            _cartRepository = cartRepository;
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersAsync(Guid tenantId, Guid platformUserId, QueryOrder query)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<IEnumerable<OrderDto>>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var validationError = ValidateOrderQuery(query);
            if (validationError != null)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, validationError);

            var orderQuery = ApplyOrderQuery(_orderRepository.GetOrdersByTenantQuery(tenantId), query);
            var orders = await PageOrderQuery(orderQuery, query).ToListAsync();
            return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(o => o.ToOrderDto()));
        }

        public async Task<ServiceResult<OrderDto>> GetOrderAsync(Guid tenantId, Guid platformUserId, Guid orderId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<OrderDto>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var order = await _orderRepository.GetOrderAsync(tenantId, orderId);
            if (order == null)
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy đơn hàng");

            return ServiceResult<OrderDto>.Ok(order.ToOrderDto());
        }

        public async Task<ServiceResult<OrderDto>> CreateOrderAsync(Guid tenantId, Guid platformUserId, CreateOrderRequestDto createDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<OrderDto>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            if (createDto.OrderItems == null || createDto.OrderItems.Count == 0)
                return ServiceResult<OrderDto>.Fail(400, "Đơn hàng phải có ít nhất 1 sản phẩm");

            var address = await _customerAddressRepository.GetAddressByIdAsync(tenantId, createDto.AddressId);
            if (address == null)
                return ServiceResult<OrderDto>.Fail(400, "Địa chỉ không tồn tại trong tenant này");

            if (createDto.CustomerId.HasValue)
            {
                if (address.CustomerId != createDto.CustomerId.Value)
                    return ServiceResult<OrderDto>.Fail(400, "Địa chỉ không thuộc customer của đơn hàng");

                if (await _customerRepository.GetCustomerAsync(tenantId, createDto.CustomerId.Value) == null)
                    return ServiceResult<OrderDto>.Fail(400, "Customer không tồn tại trong tenant này");
            }

            var normalizedPaymentMethod = NormalizePaymentMethod(createDto.PaymentMethod);
            if (normalizedPaymentMethod == null)
                return ServiceResult<OrderDto>.Fail(400, "paymentMethod chỉ hỗ trợ COD");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var now = DateTime.UtcNow;
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                TenantId = tenantId,
                CustomerId = createDto.CustomerId ?? address.CustomerId,
                AddressId = createDto.AddressId,
                Status = StatusPending,
                PaymentMethod = normalizedPaymentMethod,
                PaymentStatus = PaymentStatusPending,
                OrderCode = BuildOrderCode(now),
                ShippingMethod = "standard",
                Subtotal = 0m,
                ShippingFee = 0m,
                TaxAmount = 0m,
                TotalAmount = 0m,
                CreatedAt = now,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in createDto.OrderItems)
            {
                if (item.Quantity <= 0)
                    return ServiceResult<OrderDto>.Fail(400, "Số lượng sản phẩm trong đơn hàng phải lớn hơn 0");

                var sku = await GetSkuForTenantAsync(tenantId, item.ProductSkuId);
                if (sku == null)
                    return ServiceResult<OrderDto>.Fail(400, $"SKU {item.ProductSkuId} không tồn tại trong tenant này");

                var stockReserved = await TryDecreaseStockAsync(tenantId, item.ProductSkuId, item.Quantity);
                if (!stockReserved)
                    return ServiceResult<OrderDto>.Fail(409, $"SKU {item.ProductSkuId} không đủ tồn kho để tạo đơn hàng");

                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductSkuId = item.ProductSkuId,
                    Quantity = item.Quantity,
                    UnitPrice = sku.Price,
                    SelectedOptions = sku.AttributesJson
                };

                order.OrderItems.Add(orderItem);
                order.Subtotal += sku.Price * item.Quantity;
            }

            order.TotalAmount = order.Subtotal + order.ShippingFee + order.TaxAmount;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult<OrderDto>.Created(order.ToOrderDto());
        }

        public async Task<ServiceResult<object>> UpdateOrderStatusAsync(Guid tenantId, Guid platformUserId, Guid id, UpdateOrderStatusRequestDto updateDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var nextStatus = NormalizeOrderStatus(updateDto.Status);
            if (nextStatus == null)
                return ServiceResult<object>.Fail(400, "Trạng thái đơn hàng không hợp lệ");

            var order = await _orderRepository.GetOrderAsync(tenantId, id);
            if (order == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy đơn hàng");

            var currentStatus = NormalizeOrderStatus(order.Status) ?? StatusPending;
            if (!CanTransition(currentStatus, nextStatus))
                return ServiceResult<object>.Fail(400, $"Không thể chuyển đơn hàng từ {currentStatus} sang {nextStatus}");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (string.Equals(nextStatus, StatusCancelled, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(currentStatus, StatusCancelled, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in order.OrderItems)
                {
                    await IncreaseStockAsync(tenantId, item.ProductSkuId, item.Quantity);
                }
            }

            order.Status = nextStatus;

            if (string.Equals(order.PaymentMethod, PaymentMethodCod, StringComparison.OrdinalIgnoreCase)
                && (string.Equals(nextStatus, StatusDelivered, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(nextStatus, StatusCompleted, StringComparison.OrdinalIgnoreCase))
                && !string.Equals(order.PaymentStatus, PaymentStatusPaid, StringComparison.OrdinalIgnoreCase))
            {
                order.PaymentStatus = PaymentStatusPaid;
                order.PaidAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return ServiceResult<object>.Ok(new { message = "Cập nhật trạng thái đơn hàng thành công" });
        }

        public async Task<ServiceResult<object>> DeleteOrderAsync(Guid tenantId, Guid platformUserId, Guid orderId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var order = await _orderRepository.GetOrderAsync(tenantId, orderId);
            if (order == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy đơn hàng");

            var normalizedStatus = NormalizeOrderStatus(order.Status) ?? StatusPending;
            if (!string.Equals(normalizedStatus, StatusPending, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(normalizedStatus, StatusCancelled, StringComparison.OrdinalIgnoreCase))
                return ServiceResult<object>.Fail(400, "Không nên xóa đơn hàng đã xử lý. Hãy chuyển sang trạng thái hủy hoặc lưu trữ thay vì xóa vật lý.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            if (string.Equals(normalizedStatus, StatusPending, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in order.OrderItems)
                {
                    await IncreaseStockAsync(tenantId, item.ProductSkuId, item.Quantity);
                }
            }

            _context.OrderItems.RemoveRange(order.OrderItems);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult<object>.Ok(new { message = "Xóa đơn hàng thành công" });
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetMyOrdersAsync(Guid tenantId, Guid customerId, QueryOrder query)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<IEnumerable<OrderDto>>.Fail(404, "Không tìm thấy khách hàng");

            var validationError = ValidateOrderQuery(query);
            if (validationError != null)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, validationError);

            var orderQuery = ApplyOrderQuery(_orderRepository.GetOrdersByTenantQuery(tenantId), query)
                .Where(o => o.CustomerId == customerId);

            var orders = await PageOrderQuery(orderQuery, query).ToListAsync();
            return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(o => o.ToOrderDto()));
        }

        public async Task<ServiceResult<OrderDto>> GetMyOrderAsync(Guid tenantId, Guid customerId, Guid orderId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy khách hàng");

            var order = await _orderRepository.GetOrderAsync(tenantId, orderId);
            if (order == null)
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy đơn hàng");

            if (order.CustomerId != customerId)
                return ServiceResult<OrderDto>.Forbidden("Bạn không có quyền truy cập đơn hàng này");

            return ServiceResult<OrderDto>.Ok(order.ToOrderDto());
        }

        public async Task<ServiceResult<OrderDto>> CheckoutAsync(Guid tenantId, Guid customerId, CheckoutOrderRequestDto checkoutDto)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy khách hàng");

            var address = await _customerAddressRepository.GetAddressByIdAsync(tenantId, checkoutDto.AddressId);
            if (address == null || address.CustomerId != customerId)
                return ServiceResult<OrderDto>.Fail(400, "Địa chỉ giao hàng không tồn tại hoặc không thuộc về bạn");

            var normalizedShippingMethod = NormalizeShippingMethod(checkoutDto.ShippingMethod);
            if (normalizedShippingMethod == null)
                return ServiceResult<OrderDto>.Fail(400, "shippingMethod chỉ hỗ trợ standard hoặc express");

            var normalizedPaymentMethod = NormalizePaymentMethod(checkoutDto.PaymentMethod);
            if (normalizedPaymentMethod == null)
                return ServiceResult<OrderDto>.Fail(400, "paymentMethod chỉ hỗ trợ COD");

            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var cart = await _cartRepository.GetCartAsync(tenantId, customerId);
            if (cart == null)
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy giỏ hàng");

            var cartItems = cart.CartItems?.ToList() ?? new List<CartItem>();
            if (cartItems.Count == 0)
                return ServiceResult<OrderDto>.Fail(400, "Giỏ hàng trống");

            var now = DateTime.UtcNow;
            var orderCode = BuildOrderCode(now);
            var order = new Order
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId,
                AddressId = checkoutDto.AddressId,
                Status = StatusPending,
                PaymentMethod = normalizedPaymentMethod,
                PaymentStatus = PaymentStatusPending,
                OrderCode = orderCode,
                OrderNote = string.IsNullOrWhiteSpace(checkoutDto.OrderNote) ? null : checkoutDto.OrderNote.Trim(),
                ShippingMethod = normalizedShippingMethod,
                Subtotal = 0m,
                ShippingFee = normalizedShippingMethod == "express" ? ExpressShippingFee : StandardShippingFee,
                TaxAmount = 0m,
                TotalAmount = 0m,
                PaidAt = null,
                CreatedAt = now,
                OrderItems = new List<OrderItem>()
            };

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Quantity <= 0)
                    return ServiceResult<OrderDto>.Fail(400, "Có sản phẩm trong giỏ hàng có số lượng không hợp lệ");

                var sku = await GetSkuForTenantAsync(tenantId, cartItem.ProductSkuId);
                if (sku == null)
                    return ServiceResult<OrderDto>.Fail(404, $"Không tìm thấy SKU {cartItem.ProductSkuId}");

                var stockReserved = await TryDecreaseStockAsync(tenantId, cartItem.ProductSkuId, cartItem.Quantity);
                if (!stockReserved)
                    return ServiceResult<OrderDto>.Fail(409, $"SKU {cartItem.ProductSkuId} không đủ tồn kho để thanh toán");

                order.OrderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductSkuId = cartItem.ProductSkuId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = sku.Price,
                    SelectedOptions = sku.AttributesJson
                });

                order.Subtotal += sku.Price * cartItem.Quantity;
            }

            order.TotalAmount = order.Subtotal + order.ShippingFee + order.TaxAmount;

            await _context.Orders.AddAsync(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult<OrderDto>.Created(order.ToOrderDto());
        }

        public async Task<ServiceResult<object>> CancelMyOrderAsync(Guid tenantId, Guid customerId, Guid orderId)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<object>.Fail(404, "Không tìm thấy khách hàng");

            var order = await _orderRepository.GetOrderAsync(tenantId, orderId);
            if (order == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy đơn hàng");

            if (order.CustomerId != customerId)
                return ServiceResult<object>.Forbidden("Bạn không có quyền truy cập đơn hàng này");

            var currentStatus = NormalizeOrderStatus(order.Status) ?? StatusPending;
            if (!CanCancel(currentStatus))
                return ServiceResult<object>.Fail(400, "Không thể hủy đơn hàng ở trạng thái hiện tại");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            foreach (var item in order.OrderItems)
            {
                await IncreaseStockAsync(tenantId, item.ProductSkuId, item.Quantity);
            }

            order.Status = StatusCancelled;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return ServiceResult<object>.Ok(new { message = "Hủy đơn hàng thành công" });
        }

        private static string? ValidateOrderQuery(QueryOrder query)
        {
            if (query.TotalFrom.HasValue && query.TotalTo.HasValue && query.TotalFrom.Value > query.TotalTo.Value)
                return "totalFrom không được lớn hơn totalTo";

            if (query.CreatedFrom.HasValue && query.CreatedTo.HasValue && query.CreatedFrom.Value > query.CreatedTo.Value)
                return "createdFrom không được lớn hơn createdTo";

            return null;
        }

        private static IQueryable<Order> ApplyOrderQuery(IQueryable<Order> orderQuery, QueryOrder query)
        {
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                if (Guid.TryParse(query.SearchTerm, out var orderOrCustomerId))
                {
                    orderQuery = orderQuery.Where(o =>
                        o.Id == orderOrCustomerId ||
                        o.CustomerId == orderOrCustomerId ||
                        (o.Address != null && o.Address.StreetAddress.Contains(query.SearchTerm)) ||
                        (o.Status != null && o.Status.Contains(query.SearchTerm)));
                }
                else
                {
                    orderQuery = orderQuery.Where(o =>
                        (o.Address != null && o.Address.StreetAddress.Contains(query.SearchTerm)) ||
                        (o.Status != null && o.Status.Contains(query.SearchTerm)) ||
                        (o.PaymentMethod != null && o.PaymentMethod.Contains(query.SearchTerm)) ||
                        (o.PaymentStatus != null && o.PaymentStatus.Contains(query.SearchTerm)));
                }
            }

            if (query.CustomerId.HasValue)
                orderQuery = orderQuery.Where(o => o.CustomerId == query.CustomerId.Value);
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var normalizedStatus = query.Status.Trim().ToLowerInvariant();
                orderQuery = orderQuery.Where(o => o.Status != null && o.Status.ToLower() == normalizedStatus);
            }
            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
            {
                var normalizedPaymentMethod = query.PaymentMethod.Trim().ToLowerInvariant();
                orderQuery = orderQuery.Where(o => o.PaymentMethod != null && o.PaymentMethod.ToLower() == normalizedPaymentMethod);
            }
            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
            {
                var normalizedPaymentStatus = query.PaymentStatus.Trim().ToLowerInvariant();
                orderQuery = orderQuery.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == normalizedPaymentStatus);
            }
            if (query.TotalFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount >= query.TotalFrom.Value);
            if (query.TotalTo.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount <= query.TotalTo.Value);
            if (query.CreatedFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= query.CreatedFrom.Value);
            if (query.CreatedTo.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= query.CreatedTo.Value);

            return orderQuery;
        }

        private static IQueryable<Order> PageOrderQuery(IQueryable<Order> orderQuery, QueryOrder query)
        {
            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            orderQuery = query.SortBy switch
            {
                "createdat" or "created_at" => isDescending ? orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id),
                "totalamount" or "total_amount" => isDescending ? orderQuery.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.TotalAmount).ThenBy(o => o.Id),
                "status" => isDescending ? orderQuery.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Status).ThenBy(o => o.Id),
                "paymentstatus" or "payment_status" => isDescending ? orderQuery.OrderByDescending(o => o.PaymentStatus).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.PaymentStatus).ThenBy(o => o.Id),
                "id" => isDescending ? orderQuery.OrderByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Id),
                _ => orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id)
            };

            var skipNumber = (query.Page - 1) * query.PageSize;
            return orderQuery.Skip(skipNumber).Take(query.PageSize);
        }

        private async Task<ProductSku?> GetSkuForTenantAsync(Guid tenantId, Guid productSkuId)
        {
            return await _context.ProductSkus
                .AsNoTracking()
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == productSkuId && ps.Product.TenantId == tenantId);
        }

        private async Task<bool> TryDecreaseStockAsync(Guid tenantId, Guid productSkuId, int quantity)
        {
            var affectedRows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE ps
                SET stock = stock - {quantity}
                FROM product_skus AS ps
                INNER JOIN products AS p ON p.id = ps.product_id
                WHERE ps.id = {productSkuId}
                  AND p.tenant_id = {tenantId}
                  AND ps.stock >= {quantity}");

            return affectedRows == 1;
        }

        private async Task IncreaseStockAsync(Guid tenantId, Guid productSkuId, int quantity)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE ps
                SET stock = stock + {quantity}
                FROM product_skus AS ps
                INNER JOIN products AS p ON p.id = ps.product_id
                WHERE ps.id = {productSkuId}
                  AND p.tenant_id = {tenantId}");
        }

        private static string BuildOrderCode(DateTime now)
        {
            return $"ORD-{now:yyyyMMdd-HHmmssfff}-{Guid.NewGuid():N}"[..31];
        }

        private static string? NormalizeShippingMethod(string? shippingMethod)
        {
            if (string.IsNullOrWhiteSpace(shippingMethod))
                return "standard";

            var normalized = shippingMethod.Trim().ToLowerInvariant();
            return normalized is "standard" or "express" ? normalized : null;
        }

        private static string? NormalizePaymentMethod(string? paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                return PaymentMethodCod;

            var normalized = paymentMethod.Trim();
            if (string.Equals(normalized, PaymentMethodCod, StringComparison.OrdinalIgnoreCase))
                return PaymentMethodCod;

            return null;
        }

        private static string? NormalizeOrderStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return null;

            var key = status.Trim().ToLowerInvariant();
            return CanonicalStatuses.TryGetValue(key, out var canonical) ? canonical : null;
        }

        private static bool CanCancel(string currentStatus)
        {
            return string.Equals(currentStatus, StatusPending, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentStatus, StatusConfirmed, StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentStatus, StatusProcessing, StringComparison.OrdinalIgnoreCase);
        }

        private static bool CanTransition(string currentStatus, string nextStatus)
        {
            if (string.Equals(currentStatus, nextStatus, StringComparison.OrdinalIgnoreCase))
                return true;

            if (IsFinalStatus(currentStatus))
                return false;

            return currentStatus switch
            {
                StatusPending => nextStatus == StatusConfirmed || nextStatus == StatusProcessing || nextStatus == StatusCancelled,
                StatusConfirmed => nextStatus == StatusProcessing || nextStatus == StatusShipping || nextStatus == StatusCancelled,
                StatusProcessing => nextStatus == StatusShipping || nextStatus == StatusShipped || nextStatus == StatusCancelled,
                StatusShipping => nextStatus == StatusShipped || nextStatus == StatusDelivered,
                StatusShipped => nextStatus == StatusDelivered || nextStatus == StatusCompleted,
                _ => false
            };
        }

        private static bool IsFinalStatus(string status)
        {
            return string.Equals(status, StatusCancelled, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, StatusDelivered, StringComparison.OrdinalIgnoreCase)
                || string.Equals(status, StatusCompleted, StringComparison.OrdinalIgnoreCase);
        }
    }
}