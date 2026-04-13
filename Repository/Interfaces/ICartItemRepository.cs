
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>?> GetCartItemsAsync(Guid tenantId, Guid customerId);
        Task<CartItem> AddToCartAsync(Guid tenantId, Guid customerId, Guid productSkuId, int quantity);
        Task<CartItem?> UpdateCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId, int quantity);
        Task<CartItem?> DeleteCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId);
    }
}

