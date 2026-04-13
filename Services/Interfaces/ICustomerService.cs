using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Services;

namespace FluxifyAPI.IServices
{
    public interface ICustomerService
    {
        Task<ServiceResult<IEnumerable<CustomerDto>>> GetCustomersAsync(Guid tenantId, Guid ownerId);
        Task<ServiceResult<CustomerDto>> GetCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId);
        Task<ServiceResult<CustomerDto>> GetCustomerByEmailAsync(Guid tenantId, string email, Guid ownerId);
        Task<ServiceResult<CustomerDto>> GetCustomerByCartAsync(Guid tenantId, Guid cartId, Guid ownerId);
        Task<ServiceResult<CustomerDto>> CreateCustomerAsync(Guid tenantId, CreateCustomerRequestDto customerDto, Guid ownerId);
        Task<ServiceResult<CustomerDto>> UpdateCustomerAsync(Guid tenantId, Guid customerId, UpdateCustomerRequestDto customerDto, Guid ownerId);
        Task<ServiceResult<object>> DeleteCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId);
    }
}
