using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Services;

namespace FluxifyAPI.IServices
{
    public interface ICartItemService
    {
        Task<ServiceResult<IEnumerable<object>>> GetCartItemsAsync(Guid tenantId, Guid customerId);
        Task<ServiceResult<object>> AddToCartAsync(Guid tenantId, Guid customerId, CreateCartItemRequestDto createDto);
        Task<ServiceResult<object>> UpdateCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId, UpdateCartItemRequestDto updateDto);
        Task<ServiceResult<object>> RemoveCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId);
        Task<ServiceResult<object>> ClearCartAsync(Guid tenantId, Guid customerId);
    }
}
