using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<ServiceResult<IEnumerable<CustomerDto>>> GetCustomersAsync(Guid tenantId, Guid ownerId, QueryCustomer query);
        Task<ServiceResult<CustomerDto>> GetCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId);
        Task<ServiceResult<object>> DeleteCustomerAsync(Guid tenantId, Guid customerId, Guid ownerId);
    }
}


