using FluxifyAPI.Data;
using FluxifyAPI.Models;
using FluxifyAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
{
    public class TenantPaymentSettingRepository : ITenantPaymentSettingRepository
    {
        private readonly AppDbContext _context;

        public TenantPaymentSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TenantPaymentSetting>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.TenantPaymentSettings
                .AsNoTracking()
                .Where(x => x.TenantId == tenantId)
                .OrderByDescending(x => x.IsActive)
                .ThenByDescending(x => x.UpdatedAt)
                .ToListAsync();
        }

        public async Task<TenantPaymentSetting?> GetByIdAsync(Guid id)
        {
            return await _context.TenantPaymentSettings
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TenantPaymentSetting?> GetActiveByTenantIdAsync(Guid tenantId)
        {
            return await _context.TenantPaymentSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.IsActive);
        }

        public async Task<TenantPaymentSetting> CreateAsync(TenantPaymentSetting setting)
        {
            await _context.TenantPaymentSettings.AddAsync(setting);
            await _context.SaveChangesAsync();
            return setting;
        }

        public async Task<TenantPaymentSetting> UpdateAsync(TenantPaymentSetting setting)
        {
            if (_context.Entry(setting).State == EntityState.Detached)
                _context.TenantPaymentSettings.Attach(setting);

            await _context.SaveChangesAsync();
            return setting;
        }

        public async Task<bool> DeleteAsync(TenantPaymentSetting setting)
        {
            _context.TenantPaymentSettings.Remove(setting);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SetOthersInactiveAsync(Guid tenantId, Guid exceptId)
        {
            var others = await _context.TenantPaymentSettings
                .Where(x => x.TenantId == tenantId && x.Id != exceptId && x.IsActive)
                .ToListAsync();

            foreach (var item in others)
            {
                item.IsActive = false;
                item.UpdatedAt = DateTime.UtcNow;
            }

            if (others.Count > 0)
                await _context.SaveChangesAsync();
        }
    }
}
