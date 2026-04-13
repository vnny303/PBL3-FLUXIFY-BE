using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.IServices;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        public async Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersAsync(Guid tenantId, QueryOrder query)
        {
            if (query.TenantId.HasValue && query.TenantId.Value != tenantId)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, "tenantId trong query không khớp route");

            if (query.TotalFrom.HasValue && query.TotalTo.HasValue && query.TotalFrom.Value > query.TotalTo.Value)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, "totalFrom không được lớn hơn totalTo");

            if (query.CreatedFrom.HasValue && query.CreatedTo.HasValue && query.CreatedFrom.Value > query.CreatedTo.Value)
                return ServiceResult<IEnumerable<OrderDto>>.Fail(400, "createdFrom không được lớn hơn createdTo");

            var orderQuery = _orderRepository.GetOrdersByTenantQuery(tenantId);

            var searchTerm = query.NormalizedSearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var orderOrCustomerId))
                {
                    orderQuery = orderQuery.Where(o =>
                        o.Id == orderOrCustomerId ||
                        o.CustomerId == orderOrCustomerId ||
                        (o.Address != null && o.Address.Contains(searchTerm)) ||
                        (o.Status != null && o.Status.Contains(searchTerm)));
                }
                else
                {
                    orderQuery = orderQuery.Where(o =>
                        (o.Address != null && o.Address.Contains(searchTerm)) ||
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

            var sortBy = query.SortBy?.Trim();
            var isDescending = query.NormalizedIsDescending;
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

            var pageNumber = query.NormalizedPageNumber;
            var pageSize = query.NormalizedPageSize;
            var skipNumber = (pageNumber - 1) * pageSize;

            var orders = await orderQuery.Skip(skipNumber).Take(pageSize).ToListAsync();
            return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(o => o.ToOrderDto()));
        }

        public async Task<ServiceResult<OrderDto>> GetOrderAsync(Guid tenantId, Guid id)
        {
            var order = await _orderRepository.GetOrderAsync(tenantId, id);
            if (order == null)
                return ServiceResult<OrderDto>.Fail(404, "Không tìm thấy đơn hàng");

            return ServiceResult<OrderDto>.Ok(order.ToOrderDto());
        }

        public async Task<ServiceResult<OrderDto>> CreateOrderAsync(Guid tenantId, CreateOrderRequestDto createDto)
        {
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

        public async Task<ServiceResult<object>> UpdateOrderStatusAsync(Guid tenantId, Guid id, UpdateOrderStatusRequestDto updateDto)
        {
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

        public async Task<ServiceResult<object>> DeleteOrderAsync(Guid tenantId, Guid id)
        {
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
    }
}
