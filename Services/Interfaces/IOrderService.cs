using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersAsync(Guid tenantId, Guid platformUserId, QueryOrder query);
        Task<ServiceResult<OrderDto>> GetOrderAsync(Guid tenantId, Guid platformUserId, Guid id);
        Task<ServiceResult<OrderDto>> CreateOrderAsync(Guid tenantId, Guid platformUserId, CreateOrderRequestDto createDto);
        Task<ServiceResult<object>> UpdateOrderStatusAsync(Guid tenantId, Guid platformUserId, Guid orderId, UpdateOrderStatusRequestDto updateDto);
        Task<ServiceResult<object>> DeleteOrderAsync(Guid tenantId, Guid platformUserId, Guid id);

        Task<ServiceResult<IEnumerable<OrderDto>>> GetMyOrdersAsync(Guid tenantId, Guid customerId, QueryOrder query);
        Task<ServiceResult<OrderDto>> GetMyOrderAsync(Guid tenantId, Guid customerId, Guid orderId);
        Task<ServiceResult<OrderDto>> CheckoutAsync(Guid tenantId, Guid customerId, CheckoutOrderRequestDto checkoutDto);
        Task<ServiceResult<object>> CancelMyOrderAsync(Guid tenantId, Guid customerId, Guid orderId);
    }
}


