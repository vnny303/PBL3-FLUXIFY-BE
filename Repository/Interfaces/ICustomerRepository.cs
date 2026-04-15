using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerAsync(Guid tenantId, Guid customerId);
        Task<IEnumerable<Customer>> GetCustomersBySubdomainAsync(string subdomain);
        IQueryable<Customer> GetCustomer(Guid tenantId);
        Task<Customer?> GetCustomerByEmailAsync(Guid tenantId, string email);
        // Task<Customer?> GetCustomerByCartAsync(Guid tenantId, Guid cartId);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(Customer customer);
        Task<Customer?> DeleteCustomerAsync(Guid tenantId, Guid customerId);
        Task<bool> CustomerExists(Guid tenantId, Guid customerId);
    }
}