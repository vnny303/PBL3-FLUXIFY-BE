using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly AppDbContext _context;

        public OrderItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderItem> GetOrderItemAsync(Guid tenantId, Guid orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.Order.TenantId == tenantId);

            if (orderItem == null)
                throw new KeyNotFoundException("Order item not found.");

            return orderItem;
        }

        public async Task<List<OrderItem>?> GetOrderItemsByOrderAsync(Guid tenantId, Guid orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Order)
                .Where(oi => oi.OrderId == orderId && oi.Order.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<OrderItem> CreateOrderItemAsync(Guid tenantId, Guid orderId, Guid productSkuId, int quantity, decimal price)
        {
            var orderBelongsToTenant = await _context.Orders
                .AnyAsync(o => o.Id == orderId && o.TenantId == tenantId);

            if (!orderBelongsToTenant)
            {
                throw new KeyNotFoundException("Order not found for tenant.");
            }

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductSkuId = productSkuId,
                Quantity = quantity,
                UnitPrice = price
            };

            await _context.OrderItems.AddAsync(orderItem);
            await _context.SaveChangesAsync();

            return orderItem;
        }

        public async Task<OrderItem?> UpdateOrderItemAsync(Guid tenantId, Guid orderItemId, int quantity, decimal price)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.Order.TenantId == tenantId);

            if (orderItem == null)
            {
                return null;
            }

            orderItem.Quantity = quantity;
            orderItem.UnitPrice = price;
            await _context.SaveChangesAsync();

            return orderItem;
        }

        public async Task<OrderItem?> DeleteOrderItemAsync(Guid tenantId, Guid orderItemId)
        {
            var orderItem = await _context.OrderItems
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.Order.TenantId == tenantId);

            if (orderItem == null)
            {
                return null;
            }

            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return orderItem;
        }
    }
}


