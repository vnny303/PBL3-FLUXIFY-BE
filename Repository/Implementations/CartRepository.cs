using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Mapper;

namespace FluxifyAPI.Repository.Implementations
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

        public async Task<Cart> CreateCartAsync(CreateCartRequestDto createDto)
        {
            var cart = createDto.ToCartFromCreateDto();
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            return cart;
        }
        public async Task<bool> CartExists(Guid tenantId, Guid customerId)
        {
            return await _context.Carts.AnyAsync(c => c.TenantId == tenantId && c.CustomerId == customerId);
        }

        public async Task<bool> CartContainsProductSku(Guid tenantId, Guid userId, Guid productSkuId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .AnyAsync(c => c.TenantId == tenantId && c.CustomerId == userId && c.CartItems.Any(ci => ci.ProductSkuId == productSkuId));
        }
    }
}