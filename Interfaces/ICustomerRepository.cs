using FluxifyAPI.DTOs.Customer;
using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerAsync(Guid tenantId, Guid customerId);
        Task<List<Customer>> GetCustomersByTenantAsync(Guid tenantId);
        Task<List<Customer>> GetCustomersBySubdomainAsync(string subdomain);
        Task<Customer?> GetCustomerByEmailAsync(Guid tenantId, string email);
        Task<Customer?> GetCustomerByCartAsync(Guid tenantId, Guid cartId);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer?> UpdateCustomerAsync(Guid tenantId, Guid customerId, UpdateCustomerRequestDto customer);
        Task<Customer?> DeleteCustomerAsync(Guid tenantId, Guid customerId);
    }
}