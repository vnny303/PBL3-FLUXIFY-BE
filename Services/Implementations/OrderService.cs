using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace FluxifyAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private const double StandardShippingFee = 15000;
        private const double ExpressShippingFee = 30000;

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantPaymentSettingRepository _tenantPaymentSettingRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductSkuRepository _productSkuRepository;

        public OrderService(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ICustomerRepository customerRepository,
            ITenantRepository tenantRepository,
            ITenantPaymentSettingRepository tenantPaymentSettingRepository,
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductSkuRepository productSkuRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
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
            if (!string.IsNullOrEmpty(query.SearchTerm))
                if (Guid.TryParse(query.SearchTerm, out var orderOrCustomerId))
                    orderQuery = orderQuery.Where(o =>
                        o.Id == orderOrCustomerId ||
                        o.CustomerId == orderOrCustomerId ||
                        (o.Address != null && o.Address.StreetAddress.Contains(query.SearchTerm)) ||
                        (o.Status != null && o.Status.Contains(query.SearchTerm)));
                else
                    orderQuery = orderQuery.Where(o =>
                        (o.Address != null && o.Address.StreetAddress.Contains(query.SearchTerm)) ||
                        (o.Status != null && o.Status.Contains(query.SearchTerm)) ||
                        (o.PaymentMethod != null && o.PaymentMethod.Contains(query.SearchTerm)) ||
                        (o.PaymentStatus != null && o.PaymentStatus.Contains(query.SearchTerm)));

            if (query.CustomerId.HasValue)
                orderQuery = orderQuery.Where(o => o.CustomerId == query.CustomerId.Value);
            if (!string.IsNullOrWhiteSpace(query.Status))
                orderQuery = orderQuery.Where(o => o.Status != null && o.Status.ToLower() == query.Status);
            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
                orderQuery = orderQuery.Where(o => o.PaymentMethod != null && o.PaymentMethod.ToLower() == query.PaymentMethod);
            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
                orderQuery = orderQuery.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == query.PaymentStatus);
            if (query.TotalFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount >= query.TotalFrom.Value);
            if (query.TotalTo.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount <= query.TotalTo.Value);
            if (query.CreatedFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= query.CreatedFrom.Value);
            if (query.CreatedTo.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= query.CreatedTo.Value);

            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
            switch (query.SortBy)
            {
                case "createdat":
                case "created_at":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id);
                    break;
                case "totalamount":
                case "total_amount":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.TotalAmount).ThenBy(o => o.Id);
                    break;
                case "status":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Status).ThenBy(o => o.Id);
                    break;
                case "paymentstatus":
                case "payment_status":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.PaymentStatus).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.PaymentStatus).ThenBy(o => o.Id);
                    break;
                case "id":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Id);
                    break;
                default:
                    orderQuery = orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id);
                    break;
            }
            var skipNumber = (query.Page - 1) * query.PageSize;
            var orders = await orderQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();
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
            if (createDto.CustomerId.HasValue && await _customerRepository.GetCustomerAsync(tenantId, createDto.CustomerId.Value) == null)
                return ServiceResult<OrderDto>.Fail(400, "Customer không tồn tại trong tenant này");

            var order = createDto.ToOrderFromCreateDto(tenantId);

            if (order.OrderItems == null || order.OrderItems.Count == 0)
                return ServiceResult<OrderDto>.Fail(400, "Đơn hàng phải có ít nhất 1 sản phẩm");

            var skuByOrderItemId = new Dictionary<Guid, Models.ProductSku>();

            // Validate toàn bộ trước khi bắt đầu transaction ghi dữ liệu.
            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem.Quantity <= 0)
                    return ServiceResult<OrderDto>.Fail(400, "Số lượng sản phẩm trong đơn hàng phải lớn hơn 0");
                var sku = await _productSkuRepository.GetProductSkusAsync(tenantId, orderItem.ProductSkuId);
                if (sku == null)
                    return ServiceResult<OrderDto>.Fail(400, $"SKU {orderItem.ProductSkuId} không tồn tại trong tenant này");
                if (sku.Stock < orderItem.Quantity)
                    return ServiceResult<OrderDto>.Fail(400, $"SKU {orderItem.ProductSkuId} chỉ còn {sku.Stock} trong kho, không đủ để tạo đơn hàng");
                skuByOrderItemId[orderItem.Id] = sku;
            }

            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            double totalAmount = 0;
            foreach (var orderItem in order.OrderItems)
            {
                var sku = skuByOrderItemId[orderItem.Id];
                // Đơn giá phải chốt theo giá SKU hiện tại trong DB, không lấy từ client.
                orderItem.UnitPrice = sku.Price;
                totalAmount += sku.Price * orderItem.Quantity;

                // Trừ tồn kho trong cùng transaction với thao tác tạo order.
                sku.Stock -= orderItem.Quantity;
                await _productSkuRepository.UpdateProductSkuAsync(sku);
            }
            order.TotalAmount = totalAmount;
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            transaction.Complete();

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
            return ServiceResult<object>.Ok(new { message = "Cập nhật trạng thái đơn hàng thành công" });
        }

        public async Task<ServiceResult<object>> DeleteOrderAsync(Guid tenantId, Guid platformUserId, Guid orderId)
        {
            if (!await _tenantRepository.IsTenantOwner(tenantId, platformUserId))
                return ServiceResult<object>.Forbidden("Bạn không có quyền đối với đơn hàng của tenant này");
            if (await _orderRepository.GetOrderAsync(tenantId, orderId) == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy đơn hàng");
            foreach (var orderItem in await _orderItemRepository.GetOrderItemsByOrderAsync(tenantId, orderId))
                await _orderItemRepository.DeleteOrderItemAsync(tenantId, orderItem.Id);
            await _orderRepository.DeleteOrderAsync(tenantId, orderId);
            return ServiceResult<object>.Ok(new { message = "Xóa đơn hàng thành công" });
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetMyOrdersAsync(Guid tenantId, Guid customerId, QueryOrder query)
        {
            if (!await _customerRepository.CustomerExists(tenantId, customerId))
                return ServiceResult<IEnumerable<OrderDto>>.Fail(404, "Không tìm thấy khách hàng");
            var orderQuery = _orderRepository.GetOrdersByTenantQuery(tenantId)
                .Where(o => o.CustomerId == customerId);
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                if (Guid.TryParse(query.SearchTerm, out var orderId))
                {
                    orderQuery = orderQuery.Where(o =>
                        o.Id == orderId ||
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

            if (!string.IsNullOrWhiteSpace(query.Status))
                orderQuery = orderQuery.Where(o => o.Status != null && o.Status.ToLower() == query.Status);

            if (!string.IsNullOrWhiteSpace(query.PaymentMethod))
                orderQuery = orderQuery.Where(o => o.PaymentMethod != null && o.PaymentMethod.ToLower() == query.PaymentMethod);

            if (!string.IsNullOrWhiteSpace(query.PaymentStatus))
                orderQuery = orderQuery.Where(o => o.PaymentStatus != null && o.PaymentStatus.ToLower() == query.PaymentStatus);
            if (query.TotalFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount >= query.TotalFrom.Value);
            if (query.TotalTo.HasValue)
                orderQuery = orderQuery.Where(o => o.TotalAmount <= query.TotalTo.Value);
            if (query.CreatedFrom.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= query.CreatedFrom.Value);
            if (query.CreatedTo.HasValue)
                orderQuery = orderQuery.Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value <= query.CreatedTo.Value);

            var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            switch (query.SortBy)
            {
                case "createdat":
                case "created_at":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.CreatedAt).ThenBy(o => o.Id);
                    break;
                case "totalamount":
                case "total_amount":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.TotalAmount).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.TotalAmount).ThenBy(o => o.Id);
                    break;
                case "status":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Status).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Status).ThenBy(o => o.Id);
                    break;
                case "paymentstatus":
                case "payment_status":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.PaymentStatus).ThenByDescending(o => o.Id) : orderQuery.OrderBy(o => o.PaymentStatus).ThenBy(o => o.Id);
                    break;
                case "id":
                    orderQuery = isDescending ? orderQuery.OrderByDescending(o => o.Id) : orderQuery.OrderBy(o => o.Id);
                    break;
                default:
                    orderQuery = orderQuery.OrderByDescending(o => o.CreatedAt).ThenByDescending(o => o.Id);
                    break;
            }

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
            double subtotal = 0;

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
            var taxAmount = 0;
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





