using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface IOrderRepository
    {
        public Task<Order?> GetOrderAsync(Guid tenantId, Guid orderId);
        public Task<List<Order>?> GetOrdersByCustomerAsync(Guid tenantId, Guid customerId);
        public IQueryable<Order> GetOrdersByTenantQuery(Guid tenantId);
        public Task<List<Order>?> GetOrdersByTenantAsync(Guid tenantId);
        public Task<Order> CreateOrderAsync(Order order);
        public Task<Order> UpdateOrderAsync(Order order);
        public Task<Order?> DeleteOrderAsync(Guid tenantId, Guid orderId);
    }
}