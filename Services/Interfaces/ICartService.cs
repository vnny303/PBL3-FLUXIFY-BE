using FluxifyAPI.DTOs.Cart;
using FluxifyAPI.Services;

namespace FluxifyAPI.IServices
{
    public interface ICartService
    {
        Task<ServiceResult<IEnumerable<CartDto>>> GetCartsAsync(Guid tenantId, Guid customerId);
        Task<ServiceResult<CartDto>> CreateCartAsync(Guid tenantId, Guid customerId, CreateCartRequestDto createDto);
    }
}
