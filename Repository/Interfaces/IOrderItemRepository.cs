using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IOrderItemRepository
    {
        Task<OrderItem> GetOrderItemAsync(Guid tenantId, Guid orderItemId);
        Task<List<OrderItem>?> GetOrderItemsByOrderAsync(Guid tenantId, Guid orderId);
        Task<OrderItem> CreateOrderItemAsync(Guid tenantId, Guid orderId, Guid productSkuId, int quantity, decimal price);
        Task<OrderItem?> UpdateOrderItemAsync(Guid tenantId, Guid orderItemId, int quantity, decimal price);
        Task<OrderItem?> DeleteOrderItemAsync(Guid tenantId, Guid orderItemId);
    }
}

