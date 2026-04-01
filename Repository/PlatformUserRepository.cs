using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
{
    public class PlatformUserRepository : IPlatformUserRepository
    {
        private readonly AppDbContext _context;

        public PlatformUserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlatformUser?> GetPlatformUserAsync(Guid tenantId, Guid platformUserId)
        {
            return await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Id == platformUserId && p.Tenants.Any(t => t.Id == tenantId));
        }

        public async Task<PlatformUser?> GetPlatformUserByEmailAsync(Guid tenantId, string email)
        {
            return await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Email == email && p.Tenants.Any(t => t.Id == tenantId));
        }

        public async Task<PlatformUser> CreatePlatformUserAsync(Guid tenantId, string name, string email, string password)
        {
            _ = tenantId;

            var user = new PlatformUser
            {
                Id = Guid.NewGuid(),
                Fullname = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = "merchant",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.PlatformUsers.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<PlatformUser?> UpdatePlatformUserAsync(Guid tenantId, Guid platformUserId, string name, string email, string password)
        {
            var user = await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Id == platformUserId && p.Tenants.Any(t => t.Id == tenantId));

            if (user == null)
            {
                return null;
            }

            user.Fullname = name;
            user.Email = email;

            if (!string.IsNullOrWhiteSpace(password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<PlatformUser?> DeletePlatformUserAsync(Guid tenantId, Guid platformUserId)
        {
            var user = await _context.PlatformUsers
                .Include(p => p.Tenants)
                .FirstOrDefaultAsync(p => p.Id == platformUserId && p.Tenants.Any(t => t.Id == tenantId));

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
