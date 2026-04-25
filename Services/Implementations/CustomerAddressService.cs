using FluxifyAPI.DTOs.CustomerAddress;
using FluxifyAPI.Mapper;
using FluxifyAPI.Models;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Services.Interfaces;

namespace FluxifyAPI.Services.Implementations
{
    public class CustomerAddressService : ICustomerAddressService
    {
        private readonly ICustomerAddressRepository _repository;

        public CustomerAddressService(ICustomerAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CustomerAddressDto>> GetAddressesByCustomerIdAsync(Guid tenantId, Guid customerId)
        {
            var addresses = await _repository.GetAddressesByCustomerIdAsync(tenantId, customerId);
            return addresses.Select(x => x.ToDto()).ToList();
        }

        public async Task<CustomerAddressDto?> GetAddressByIdAsync(Guid tenantId, Guid addressId)
        {
            var address = await _repository.GetAddressByIdAsync(tenantId, addressId);
            return address?.ToDto();
        }

        public async Task<CustomerAddressDto> CreateAddressAsync(Guid tenantId, CreateCustomerAddressDto createDto)
        {
            var entity = createDto.ToEntity();
            entity.TenantId = tenantId;
            var created = await _repository.CreateAddressAsync(entity);
            return created.ToDto();
        }

        public async Task<CustomerAddressDto?> UpdateAddressAsync(Guid tenantId, Guid addressId, UpdateCustomerAddressDto updateDto)
        {
            var address = await _repository.GetAddressByIdAsync(tenantId, addressId);
            if (address == null) return null;

            address.UpdateEntity(updateDto);
            await _repository.UpdateAddressAsync(address);
            
            return address.ToDto();
        }

        public async Task<bool> DeleteAddressAsync(Guid tenantId, Guid addressId)
        {
            var address = await _repository.GetAddressByIdAsync(tenantId, addressId);
            if (address == null) return false;

            await _repository.DeleteAddressAsync(address);
            return true;
        }

        public async Task<bool> SetDefaultAddressAsync(Guid tenantId, Guid customerId, Guid addressId)
        {
            var address = await _repository.GetAddressByIdAsync(tenantId, addressId);
            if (address == null || address.CustomerId != customerId) return false;

            await _repository.SetDefaultAddressAsync(tenantId, customerId, addressId);
            return true;
        }
    }
}
