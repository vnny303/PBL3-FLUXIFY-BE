using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface IOrderItemRepository
    {
        public Task<OrderItem> GetOrderItemAsync(Guid tenantId, Guid orderItemId);
        public Task<List<OrderItem>?> GetOrderItemsByOrderAsync(Guid tenantId, Guid orderId);
        public Task<OrderItem> CreateOrderItemAsync(Guid tenantId, Guid orderId, Guid productSkuId, int quantity, decimal price);
        public Task<OrderItem?> UpdateOrderItemAsync(Guid tenantId, Guid orderItemId, int quantity, decimal price);
        public Task<OrderItem?> DeleteOrderItemAsync(Guid tenantId, Guid orderItemId);
    }
}