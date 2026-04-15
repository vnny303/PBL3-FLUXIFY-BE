using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderAsync(Guid tenantId, Guid orderId);
        Task<IEnumerable<Order>?> GetOrdersByCustomerAsync(Guid tenantId, Guid customerId);
        public IQueryable<Order> GetOrdersByTenantQuery(Guid tenantId);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        Task<Order?> DeleteOrderAsync(Guid tenantId, Guid orderId);
    }
}

