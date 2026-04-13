using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICartRepository
    {
        public Task<Cart?> GetCartAsync(Guid tenantId, Guid customerId);
        public Task<Cart> CreateCartAsync(Guid tenantId, Guid customerId);
        public Task<Cart?> DeleteCartAsync(Guid tenantId, Guid customerId);
    }
}

