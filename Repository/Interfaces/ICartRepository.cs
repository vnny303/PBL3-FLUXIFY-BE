using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid tenantId, Guid customerId);
        Task<Cart> CreateCartAsync(CreateCartRequestDto createDto);
        Task<bool> CartExists(Guid tenantId, Guid customerId);
        Task<bool> CartContainsProductSku(Guid tenantId, Guid userId, Guid productSkuId);
    }
}