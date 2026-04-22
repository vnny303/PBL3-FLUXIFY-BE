using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ICartItemService
    {
        Task<ServiceResult<IEnumerable<CartItemDto>>> GetCartItemsAsync(Guid tenantId, Guid customerId);
        Task<ServiceResult<CartItemDto>> AddToCartAsync(Guid tenantId, Guid customerId, CreateCartItemRequestDto createDto);
        Task<ServiceResult<CartItemDto>> UpdateCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId, UpdateCartItemRequestDto updateDto);
        Task<ServiceResult<object>> RemoveCartItemAsync(Guid tenantId, Guid customerId, Guid cartItemId);
        Task<ServiceResult<object>> ClearCartAsync(Guid tenantId, Guid customerId);

    }
}