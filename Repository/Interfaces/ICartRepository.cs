using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid tenantId, Guid customerId);
        Task<Cart> CreateCartAsync(Guid tenantId, Guid customerId);
        Task<Cart?> DeleteCartAsync(Guid tenantId, Guid customerId);
    }
}