using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<ServiceResult<CartDto>> GetCartsAsync(Guid tenantId, Guid userId);
        Task<ServiceResult<CartDto>> CreateCartAsync(CreateCartRequestDto createDto);
    }
}