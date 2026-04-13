using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
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
                .Where(ci => ci.Cart.CustomerId == customerId && ci.Cart.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<CartItem> AddToCartAsync(Guid tenantId, Guid customerId, Guid productSkuId, int quantity)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && c.TenantId == tenantId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    TenantId = tenantId
                };

                await _context.Carts.AddAsync(cart);
            }

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductSkuId == productSkuId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                await _context.SaveChangesAsync();
                return existingItem;
            }

            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductSkuId = productSkuId,
                Quantity = quantity
            };

            await _context.CartItems.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            return cartItem;
        }

        public async Task<CartItem?> UpdateCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId, int quantity)
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

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return cartItem;
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