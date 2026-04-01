using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface IOrderRepository
    {
        public Task<Order> GetOrderAsync(Guid tenantId, Guid orderId);
        public Task<List<Order>?> GetOrdersByCustomerAsync(Guid tenantId, Guid customerId);
        public Task<List<Order>?> GetOrdersByTenantAsync(Guid tenantId);
        public Task<Order> CreateOrderAsync(Guid tenantId, Guid customerId, List<OrderItem> orderItems, decimal totalAmount);
        public Task<Order?> UpdateOrderStatusAsync(Guid tenantId, Guid orderId, string newStatus);
        public Task<Order?> DeleteOrderAsync(Guid tenantId, Guid orderId);
    }
}