using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations {
    public class PlatformUserRepository : IPlatformUserRepository
    {
        private readonly AppDbContext _context;

        public PlatformUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlatformUser>> GetAllPlatformUsersAsync()
        {
            return await _context.PlatformUsers
                .AsSplitQuery()
                .Include(p => p.Tenants)
                    .ThenInclude(t => t.Categories)
                        .ThenInclude(c => c.Products)
                            .ThenInclude(p => p.ProductSkus)
                .Include(p => p.Tenants)
                    .ThenInclude(t => t.Customers)
                        .ThenInclude(c => c.Cart)
                            .ThenInclude(cart => cart.CartItems)
                .Include(p => p.Tenants)
                    .ThenInclude(t => t.Customers)
                        .ThenInclude(c => c.Orders)
                            .ThenInclude(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<PlatformUser?> GetPlatformUserAsync(Guid platformUserId)
        {
            return await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Id == platformUserId);
        }

        public async Task<PlatformUser?> GetPlatformUserByEmailAsync(string email)
        {
            return await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<PlatformUser?> GetMerchantByEmailAsync(string email)
        {
            return await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Email == email && p.Role == "merchant");
        }

        public Task<bool> PlatformUserEmailExistsAsync(string email)
        {
            return _context.PlatformUsers.AnyAsync(p => p.Email == email);
        }

        public async Task<PlatformUser> CreatePlatformUserAsync(PlatformUser platformUser)
        {
            await _context.PlatformUsers.AddAsync(platformUser);
            await _context.SaveChangesAsync();

            return platformUser;
        }

        public async Task<PlatformUser> UpdatePlatformUserAsync(PlatformUser platformUser)
        {
            _context.PlatformUsers.Update(platformUser);
            await _context.SaveChangesAsync();
            return platformUser;
        }

        public async Task<PlatformUser?> DeletePlatformUserAsync(Guid platformUserId)
        {
            var user = await _context.PlatformUsers
                .FirstOrDefaultAsync(p => p.Id == platformUserId);

            if (user == null)
            {
                return null;
            }

            _context.PlatformUsers.Remove(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}


