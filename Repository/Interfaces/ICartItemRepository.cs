
using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>?> GetCartItemsAsync(Guid tenantId, Guid customerId);
        Task<CartItem?> GetCartItemByIdAsync(Guid tenantId, Guid customerId, Guid cartItemId);
        Task<CartItem?> GetCartItemAsync(Guid tenantId, Guid customerId, Guid productSkuId);
        Task<CartItem> AddToCartAsync(CartItem cartItemModel);
        Task<CartItem?> UpdateCartItemAsync(CartItem cartItemModel);
        Task<CartItem?> DeleteCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId);
    }
}