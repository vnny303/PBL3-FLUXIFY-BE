using FluxifyAPI.DTOs.CustomerAddress;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ICustomerAddressService
    {
        Task<List<CustomerAddressDto>> GetAddressesByCustomerIdAsync(Guid tenantId, Guid customerId);
        Task<CustomerAddressDto?> GetAddressByIdAsync(Guid tenantId, Guid addressId);
        Task<CustomerAddressDto> CreateAddressAsync(Guid tenantId, CreateCustomerAddressDto createDto);
        Task<CustomerAddressDto?> UpdateAddressAsync(Guid tenantId, Guid addressId, UpdateCustomerAddressDto updateDto);
        Task<bool> DeleteAddressAsync(Guid tenantId, Guid addressId);
        Task<bool> SetDefaultAddressAsync(Guid tenantId, Guid customerId, Guid addressId);
    }
}
