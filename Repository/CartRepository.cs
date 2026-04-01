using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartAsync(Guid tenantId, Guid customerId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.ProductSku)
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CustomerId == customerId);
        }

        public async Task<Cart> CreateCartAsync(Guid tenantId, Guid customerId)
        {
            var cart = new Cart
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                CustomerId = customerId
            };

            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return cart;
        }

        public async Task<Cart?> DeleteCartAsync(Guid tenantId, Guid customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.CustomerId == customerId);

            if (cart == null)
            {
                return null;
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return cart;
        }
    }
}
