using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private const decimal StandardShippingFee = 15000m;
        private const decimal ExpressShippingFee = 30000m;

        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantPaymentSettingRepository _tenantPaymentSettingRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductSkuRepository _productSkuRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            ITenantRepository tenantRepository,
            ITenantPaymentSettingRepository tenantPaymentSettingRepository,
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductSkuRepository productSkuRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _tenantRepository = tenantRepository;
            _tenantPaymentSettingRepository = tenantPaymentSettingRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productSkuRepository = productSkuRepository;
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersAsync(Guid tenantId, Guid platformUserId, QueryOrder query)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<IEnumerable<OrderDto>>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            if (query.TotalFrom.HasValue && query.TotalTo.HasValue && query.TotalFrom.Value > query.TotalTo.Value)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, "totalFrom không được lớn hơn totalTo");

            if (query.CreatedFrom.HasValue && query.CreatedTo.HasValue && query.CreatedFrom.Value > query.CreatedTo.Value)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, "createdFrom không được lớn hơn createdTo");

            var orderQuery = _orderRepository.GetOrdersByTenantQuery(tenantId);

            var searchTerm = query.SearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var orderOrCustomerId))
                {
                    orderQuery = orderQuery.Where(o =>
                        o.Id == orderOrCustomerId ||
                        o.CustomerId == orderOrCustomerId ||
                        (o.Address != null && o.Address.StreetAddress.Contains(searchTerm)) ||
                        (o.Status != null && o.Status.Contains(searchTerm)));
                }
                else
                {
                    orderQuery = orderQuery.Where(o =>
                        (o.Address != null && o.Address.StreetAddress.Contains(searchTerm)) ||
                        (o.Status != null && o.Status.Contains(searchTerm)) ||
                        (o.PaymentMethod != null && o.PaymentMethod.Contains(searchTerm)) ||
                        (o.PaymentStatus != null && o.PaymentStatus.Contains(searchTerm)));
                }
            }

            if (query.CustomerId.HasValue)
                orderQuery = orderQuery.Where(o => o.CustomerId == query.CustomerId.Value);

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var status = query.Status.Trim().ToLower();
                orderQuery = orderQuery.Where(o => o.Status != null && o.Status.ToLower() == status);
            }

            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
            {
                var paymentMethod = query.PaymentMethod.Trim().ToLower();
                orderQuery = orderQuery.Where(o => o.PaymentMethod != null && o.PaymentMethod.ToLower() == paymentMethod);
            }

            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
            {
                var paymentStatus = query.PaymentStatus.Trim().ToLower();
                orderQuery = orderQuery.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == paymentStatus);
            }

            if (query.TotalFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount >= query.TotalFrom.Value);

            if (query.TotalTo.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount <= query.TotalTo.Value);

            if (query.CreatedFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= query.CreatedFrom.Value);

            if (query.CreatedTo.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= query.CreatedTo.Value);

            var sortBy = query.SortBy;
            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            var normalizedSortBy = sortBy?.ToLowerInvariant();

            if (normalizedSortBy == "createdat" || normalizedSortBy == "created_at")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id);
            else if (normalizedSortBy == "totalamount" || normalizedSortBy == "total_amount")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.TotalAmount).ThenBy(o => o.Id);
            else if (normalizedSortBy == "status")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Status).ThenBy(o => o.Id);
            else if (normalizedSortBy == "paymentstatus" || normalizedSortBy == "payment_status")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.PaymentStatus).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.PaymentStatus).ThenBy(o => o.Id);
            else if (normalizedSortBy == "id")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Id);
            else
                orderQuery = orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id);

            var skipNumber = (query.Page - 1) * query.PageSize;
            var orders = await orderQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();
            return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(o => o.ToOrderDto()));
        }

        public async Task<ServiceResult<OrderDto>> GetOrderAsync(Guid tenantId, Guid platformUserId, Guid id)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<OrderDto>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var order = await _orderRepository.GetOrderAsync(tenantId, id);
            if (order == null)
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy đơn hàng");

            return ServiceResult<OrderDto>.Ok(order.ToOrderDto());
        }

        public async Task<ServiceResult<OrderDto>> CreateOrderAsync(Guid tenantId, Guid platformUserId, CreateOrderRequestDto createDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<OrderDto>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            if (createDto.CustomerId.HasValue)
            {
                var customerExists = await _customerRepository.GetCustomerAsync(tenantId, createDto.CustomerId.Value);
                if (customerExists == null)
                    return ServiceResult<OrderDto>.Fail(400, "Customer không tồn tại trong tenant này");
            }

            var order = createDto.ToOrderFromCreateDto(tenantId);
            var createdOrder = await _orderRepository.CreateOrderAsync(order);
            return ServiceResult<OrderDto>.Created(createdOrder.ToOrderDto());
        }

        public async Task<ServiceResult<object>> UpdateOrderStatusAsync(Guid tenantId, Guid platformUserId, Guid id, UpdateOrderStatusRequestDto updateDto)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var order = await _orderRepository.GetOrderAsync(tenantId, id);
            if (order == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy đơn hàng");

            updateDto.ToOrderFromUpdateStatusDto(order);
            await _orderRepository.UpdateOrderAsync(order);

            return new ServiceResult<object>
            {
                Success = true,
                StatusCode = 204,
                Data = null
            };
        }

        public async Task<ServiceResult<object>> DeleteOrderAsync(Guid tenantId, Guid platformUserId, Guid id)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");

            var order = await _orderRepository.DeleteOrderAsync(tenantId, id);
            if (order == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy đơn hàng");

            return new ServiceResult<object>
            {
                Success = true,
                StatusCode = 204,
                Data = null
            };
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetMyOrdersAsync(Guid tenantId, Guid customerId, QueryOrder query)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<IEnumerable<OrderDto>>.Fail(404, "Không tìm thấy khách hàng");

            query ??= new QueryOrder();
            var orderQuery = _orderRepository.GetOrdersByTenantQuery(tenantId)
                .Where(o => o.CustomerId == customerId);

            var searchTerm = query.SearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var orderId))
                {
                    orderQuery = orderQuery.Where(o =>
                        o.Id == orderId ||
                        (o.Address != null && o.Address.StreetAddress.Contains(searchTerm)) ||
                        (o.Status != null && o.Status.Contains(searchTerm)));
                }
                else
                {
                    orderQuery = orderQuery.Where(o =>
                        (o.Address != null && o.Address.StreetAddress.Contains(searchTerm)) ||
                        (o.Status != null && o.Status.Contains(searchTerm)) ||
                        (o.PaymentMethod != null && o.PaymentMethod.Contains(searchTerm)) ||
                        (o.PaymentStatus != null && o.PaymentStatus.Contains(searchTerm)));
                }
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var status = query.Status.Trim().ToLower();
                orderQuery = orderQuery.Where(o => o.Status != null && o.Status.ToLower() == status);
            }

            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
            {
                var paymentMethod = query.PaymentMethod.Trim().ToLower();
                orderQuery = orderQuery.Where(o => o.PaymentMethod != null && o.PaymentMethod.ToLower() == paymentMethod);
            }

            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
            {
                var paymentStatus = query.PaymentStatus.Trim().ToLower();
                orderQuery = orderQuery.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == paymentStatus);
            }

            if (query.TotalFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount >= query.TotalFrom.Value);

            if (query.TotalTo.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount <= query.TotalTo.Value);

            if (query.CreatedFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= query.CreatedFrom.Value);

            if (query.CreatedTo.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= query.CreatedTo.Value);

            var sortBy = query.SortBy;
            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            var normalizedSortBy = sortBy?.ToLowerInvariant();

            if (normalizedSortBy == "createdat" || normalizedSortBy == "created_at")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id);
            else if (normalizedSortBy == "totalamount" || normalizedSortBy == "total_amount")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.TotalAmount).ThenBy(o => o.Id);
            else if (normalizedSortBy == "status")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Status).ThenBy(o => o.Id);
            else if (normalizedSortBy == "paymentstatus" || normalizedSortBy == "payment_status")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.PaymentStatus).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.PaymentStatus).ThenBy(o => o.Id);
            else if (normalizedSortBy == "id")
                orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Id);
            else
                orderQuery = orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id);

            var skipNumber = (query.Page - 1) * query.PageSize;
            var orders = await orderQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();
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

            var normalizedShippingMethod = NormalizeShippingMethod(checkoutDto.ShippingMethod);
            if (normalizedShippingMethod == null)
                return ServiceResult<OrderDto>.Fail(400, "shippingMethod chỉ hỗ trợ standard hoặc express");

            var normalizedPaymentMethod = string.IsNullOrWhiteSpace(checkoutDto.PaymentMethod) ? "COD" : checkoutDto.PaymentMethod.Trim();

            var cart = await _cartRepository.GetCartAsync(tenantId, customerId);
            if (cart == null)
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy giỏ hàng");

            if (cart.CartItems == null || !cart.CartItems.Any())
                return ServiceResult<OrderDto>.Fail(400, "Giỏ hàng trống");

            var cartItems = cart.CartItems.ToList();

            var skuByCartItemId = new Dictionary<Guid, Models.ProductSku>();
            var orderItems = new List<Models.OrderItem>();
            decimal subtotal = 0;

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Quantity <= 0)
                    return ServiceResult<OrderDto>.Fail(400, "Có sản phẩm trong giỏ hàng có số lượng không hợp lệ");

                var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, cartItem.ProductSkuId);
                if (sku == null)
                    return ServiceResult<OrderDto>.Fail(404, $"Không tìm thấy SKU {cartItem.ProductSkuId}");

                if (sku.Stock < cartItem.Quantity)
                    return ServiceResult<OrderDto>.Fail(400, $"SKU {cartItem.ProductSkuId} chỉ còn {sku.Stock} trong kho");

                skuByCartItemId[cartItem.Id] = sku;

                orderItems.Add(new Models.OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductSkuId = cartItem.ProductSkuId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = sku.Price
                });

                subtotal += sku.Price * cartItem.Quantity;
            }

            var shippingFee = normalizedShippingMethod == "express" ? ExpressShippingFee : StandardShippingFee;
            var taxAmount = 0m;
            var totalAmount = subtotal + shippingFee + taxAmount;

            var now = DateTime.UtcNow;
            var orderCode = await BuildOrderCodeAsync(tenantId, now);
            var paymentReference = orderCode;
            var transferContent = orderCode;

            string? bankName = null;
            string? bankCode = null;
            string? bankAccountNumber = null;
            string? bankAccountName = null;

            if (string.Equals(normalizedPaymentMethod, "BankTransfer", StringComparison.OrdinalIgnoreCase))
            {
                var bankSettings = await _tenantPaymentSettingRepository.GetActiveByTenantIdAsync(tenantId);
                if (bankSettings == null)
                    return ServiceResult<OrderDto>.Fail(400, "Tenant chưa cấu hình thông tin nhận chuyển khoản");

                bankName = bankSettings.BankName;
                bankCode = bankSettings.BankCode;
                bankAccountNumber = bankSettings.BankAccountNumber;
                bankAccountName = bankSettings.BankAccountName;
            }

            var order = new Models.Order
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId,
                AddressId = checkoutDto.AddressId,
                Status = "Pending",
                PaymentMethod = normalizedPaymentMethod,
                PaymentStatus = "Pending",
                OrderCode = orderCode,
                PaymentReference = paymentReference,
                TransferContent = transferContent,
                OrderNote = string.IsNullOrWhiteSpace(checkoutDto.OrderNote) ? null : checkoutDto.OrderNote.Trim(),
                ShippingMethod = normalizedShippingMethod,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                PaidAt = null,
                CreatedAt = now,
                OrderItems = orderItems
            };

            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            foreach (var cartItem in cartItems)
            {
                var sku = skuByCartItemId[cartItem.Id];
                sku.Stock -= cartItem.Quantity;
                await _productSkuRepository.UpdateProductSkuAsync(sku);
                await _cartItemRepository.DeleteCartItemAsync(tenantId, customerId, cartItem.Id);
            }

            var response = createdOrder.ToOrderDto();
            response.BankName = bankName;
            response.BankCode = bankCode;
            response.BankAccountNumber = bankAccountNumber;
            response.BankAccountName = bankAccountName;

            return ServiceResult<OrderDto>.Created(response);
        }

        private static string? NormalizeShippingMethod(string? shippingMethod)
        {
            if (string.IsNullOrWhiteSpace(shippingMethod))
                return "standard";

            var normalized = shippingMethod.Trim().ToLowerInvariant();
            if (normalized == "standard" || normalized == "express")
                return normalized;

            return null;
        }

        private async Task<string> BuildOrderCodeAsync(Guid tenantId, DateTime now)
        {
            var date = now.Date;
            var nextSequence = await _orderRepository.GetOrdersByTenantQuery(tenantId)
                .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Date == date)
                .CountAsync() + 1;

            return $"ORD-{now:yyyyMMdd}-{nextSequence:D4}";
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

            var currentStatus = order.Status?.Trim().ToLowerInvariant();

            if (currentStatus == "cancelled" || currentStatus == "canceled")
                return ServiceResult<object>.Fail(400, "Đơn hàng đã bị hủy trước đó");

            if (currentStatus == "completed" ||
                currentStatus == "delivered" ||
                currentStatus == "shipping" ||
                currentStatus == "shipped")
                return ServiceResult<object>.Fail(400, "Không thể hủy đơn hàng ở trạng thái hiện tại");

            foreach (var item in order.OrderItems)
            {
                var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, item.ProductSkuId);
                if (sku == null)
                    continue;

                sku.Stock += item.Quantity;
                await _productSkuRepository.UpdateProductSkuAsync(sku);
            }

            order.Status = "Cancelled";
            await _orderRepository.UpdateOrderAsync(order);

            return ServiceResult<object>.Ok(new { message = "Hủy đơn hàng thành công" });
        }
    }
}





