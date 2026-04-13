using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderAsync(Guid tenantId, Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == orderId);
        }

        public async Task<List<Order>?> GetOrdersByCustomerAsync(Guid tenantId, Guid customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.TenantId == tenantId && o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public IQueryable<Order> GetOrdersByTenantQuery(Guid tenantId)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.TenantId == tenantId)
                .AsNoTracking();
        }

        public async Task<List<Order>?> GetOrdersByTenantAsync(Guid tenantId)
        {
            return await GetOrdersByTenantQuery(tenantId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            if (_context.Entry(order).State == EntityState.Detached)
                _context.Orders.Attach(order);

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order?> DeleteOrderAsync(Guid tenantId, Guid orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }
    }
}
