using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            if (customer.Id == Guid.Empty)
                customer.Id = Guid.NewGuid();

            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> DeleteCustomerAsync(Guid tenantId, Guid customerId)
        {
            var customerModel = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);
            if (customerModel == null)
                return null;

            _context.Customers.Remove(customerModel);
            await _context.SaveChangesAsync();
            return customerModel;
        }

        public async Task<Customer?> GetCustomerAsync(Guid tenantId, Guid customerId)
        {
            var customer = await _context.Customers
                            .Include(c => c.Cart)
                            .Include(c => c.Orders)
                            .FirstOrDefaultAsync(c => c.Id == customerId && c.TenantId == tenantId);
            return customer;
        }


        // public async Task<Customer?> GetCustomerByCartAsync(Guid tenantId, Guid cartId)
        // {
        //     var customer = await _context.Customers
        //                         .Include(c => c.Cart)
        //                         .Include(c => c.Orders)
        //                         .FirstOrDefaultAsync(c => c.Cart.Id == cartId && c.TenantId == tenantId);
        //     return customer;
        // }

        public async Task<Customer?> GetCustomerByEmailAsync(Guid tenantId, string email)
        {
            var customer = await _context.Customers
                                .Include(c => c.Cart)
                                .Include(c => c.Orders)
                                .FirstOrDefaultAsync(c => c.Email == email && c.TenantId == tenantId);
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (_context.Entry(customer).State == EntityState.Detached)
                _context.Customers.Attach(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<IEnumerable<Customer>> GetCustomersBySubdomainAsync(string subdomain)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Subdomain == subdomain);
            if (tenant == null)
                return new List<Customer>();
            var customers = await _context.Customers
                                .Include(c => c.Cart)
                                .Include(c => c.Orders)
                                .Include(c => c.Tenant)
                                .Where(c => c.TenantId == tenant.Id)
                                .ToListAsync();
            return customers;
        }
        public IQueryable<Customer> GetCustomer(Guid tenantId)
        {
            return _context.Customers
                .Where(c => c.TenantId == tenantId)
                .AsNoTracking();
        }

        public async Task<bool> CustomerExists(Guid tenantId, Guid customerId)
        {
            return await _context.Customers.AnyAsync(c => c.Id == customerId && c.TenantId == tenantId);
        }
    }
}

