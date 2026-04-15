using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<ServiceResult<IEnumerable<CartDto>>> GetCartsAsync(Guid tenantId, Guid customerId);
        Task<ServiceResult<CartDto>> CreateCartAsync(Guid tenantId, Guid customerId, CreateCartRequestDto createDto);
    }
}


