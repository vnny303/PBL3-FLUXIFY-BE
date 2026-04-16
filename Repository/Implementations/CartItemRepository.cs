using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;
using FluxifyAPI.DTOs.Cart;

namespace FluxifyAPI.Repository.Implementations
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>?> GetCartItemsAsync(Guid tenantId, Guid customerId)
        {
            return await _context.CartItems
                .Include(ci => ci.ProductSku)
                    .ThenInclude(ps => ps.Product)
                .Where(ci => ci.Cart.CustomerId == customerId && ci.Cart.TenantId == tenantId)
                .ToListAsync();
        }
        public async Task<CartItem?> GetCartItemAsync(Guid tenantId, Guid customerId, Guid productSkuId)
        {
            return await _context.CartItems
                .Include(ci => ci.ProductSku)
                    .ThenInclude(ps => ps.Product)
                .FirstOrDefaultAsync(ci => ci.ProductSkuId == productSkuId
                    && ci.Cart.CustomerId == customerId
                    && ci.Cart.TenantId == tenantId);
        }

        public async Task<CartItem> AddToCartAsync(CartItem cartItemModel)
        {
            await _context.CartItems.AddAsync(cartItemModel);
            await _context.SaveChangesAsync();

            return cartItemModel;
        }
        public async Task<CartItem?> UpdateCartItemAsync(CartItem cartItemModel)
        {
            if (_context.Entry(cartItemModel).State == EntityState.Detached)
                _context.CartItems.Attach(cartItemModel);
            await _context.SaveChangesAsync();
            return cartItemModel;
        }

        public async Task<CartItem?> DeleteCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId)
        {
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId
                    && ci.Cart.CustomerId == customerId
                    && ci.Cart.TenantId == tenantId);

            if (cartItem == null)
            {
                return null;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }
    }
}

