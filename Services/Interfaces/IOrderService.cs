using FluxifyAPI.DTOs.Order;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<IEnumerable<OrderDto>>> GetOrdersAsync(Guid tenantId, QueryOrder query);
        Task<ServiceResult<OrderDto>> GetOrderAsync(Guid tenantId, Guid id);
        Task<ServiceResult<OrderDto>> CreateOrderAsync(Guid tenantId, CreateOrderRequestDto createDto);
        Task<ServiceResult<OrderDto>> UpdateOrderStatusAsync(Guid tenantId, Guid orderId, UpdateOrderStatusRequestDto updateDto);
        Task<ServiceResult<object>> DeleteOrderAsync(Guid tenantId, Guid id);
    }
}


