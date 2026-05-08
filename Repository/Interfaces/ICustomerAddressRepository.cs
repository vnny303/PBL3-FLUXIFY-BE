using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICustomerAddressRepository
    {
        Task<List<CustomerAddress>> GetAddressesByCustomerIdAsync(Guid tenantId, Guid customerId);
        Task<CustomerAddress?> GetAddressByIdAsync(Guid tenantId, Guid addressId);
        Task<CustomerAddress> CreateAddressAsync(CustomerAddress address);
        Task UpdateAddressAsync(CustomerAddress address);
        Task DeleteAddressAsync(CustomerAddress address);
        Task SetDefaultAddressAsync(Guid tenantId, Guid customerId, Guid addressId);
    }
}
