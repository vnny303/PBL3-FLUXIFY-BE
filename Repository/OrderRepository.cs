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

        public async Task<Order> GetOrderAsync(Guid tenantId, Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException("Order not found.");
            }

            return order;
        }

        public async Task<List<Order>?> GetOrdersByCustomerAsync(Guid tenantId, Guid customerId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.TenantId == tenantId && o.CustomerId == customerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Order>?> GetOrdersByTenantAsync(Guid tenantId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.TenantId == tenantId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Guid tenantId, Guid customerId, List<OrderItem> orderItems, decimal totalAmount)
        {
            var order = new Order
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId,
                Status = "Pending",
                PaymentStatus = "Pending",
                TotalAmount = totalAmount,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in orderItems)
            {
                item.Id = Guid.NewGuid();
                item.OrderId = order.Id;
            }

            order.OrderItems = orderItems;

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<Order?> UpdateOrderStatusAsync(Guid tenantId, Guid orderId, string newStatus)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.TenantId == tenantId && o.Id == orderId);

            if (order == null)
            {
                return null;
            }

            order.Status = newStatus;
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
