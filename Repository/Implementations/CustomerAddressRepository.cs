using FluxifyAPI.Data;
using FluxifyAPI.Models;
using FluxifyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
{
    public class CustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly AppDbContext _context;

        public CustomerAddressRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CustomerAddress>> GetAddressesByCustomerIdAsync(Guid tenantId, Guid customerId)
        {
            return await _context.CustomerAddresses
                .Where(ca => ca.TenantId == tenantId && ca.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<CustomerAddress?> GetAddressByIdAsync(Guid tenantId, Guid addressId)
        {
            return await _context.CustomerAddresses
                .FirstOrDefaultAsync(ca => ca.TenantId == tenantId && ca.Id == addressId);
        }

        public async Task<CustomerAddress> CreateAddressAsync(CustomerAddress address)
        {
            if (address.IsDefault)
            {
                var existingDefault = await _context.CustomerAddresses
                    .FirstOrDefaultAsync(ca => ca.TenantId == address.TenantId && ca.CustomerId == address.CustomerId && ca.IsDefault);
                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                    _context.CustomerAddresses.Update(existingDefault);
                }
            }
            else
            {
                var hasAny = await _context.CustomerAddresses
                    .AnyAsync(ca => ca.TenantId == address.TenantId && ca.CustomerId == address.CustomerId);
                if (!hasAny)
                {
                    address.IsDefault = true;
                }
            }

            await _context.CustomerAddresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task UpdateAddressAsync(CustomerAddress address)
        {
            if (address.IsDefault)
            {
                var existingDefault = await _context.CustomerAddresses
                    .FirstOrDefaultAsync(ca => ca.TenantId == address.TenantId && ca.CustomerId == address.CustomerId && ca.IsDefault && ca.Id != address.Id);
                if (existingDefault != null)
                {
                    existingDefault.IsDefault = false;
                    _context.CustomerAddresses.Update(existingDefault);
                }
            }
            _context.CustomerAddresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAddressAsync(CustomerAddress address)
        {
            _context.CustomerAddresses.Remove(address);
            await _context.SaveChangesAsync();
        }

        public async Task SetDefaultAddressAsync(Guid tenantId, Guid customerId, Guid addressId)
        {
            var addresses = await _context.CustomerAddresses
                .Where(ca => ca.TenantId == tenantId && ca.CustomerId == customerId)
                .ToListAsync();

            foreach (var addr in addresses)
            {
                addr.IsDefault = (addr.Id == addressId);
            }

            _context.CustomerAddresses.UpdateRange(addresses);
            await _context.SaveChangesAsync();
        }
    }
}
