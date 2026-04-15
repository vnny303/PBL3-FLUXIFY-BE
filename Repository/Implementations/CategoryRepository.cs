using FluxifyAPI.Data;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetCategoryAsync(Guid tenantId, Guid categoryId)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == categoryId);
        }

        public IQueryable<Category> GetCategoriesByTenant(Guid tenantId)
        {
            return _context.Categories
                .Where(c => c.TenantId == tenantId)
                .Include(c => c.Products)
                .AsNoTracking();
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            if (_context.Entry(category).State == EntityState.Detached)
                _context.Categories.Attach(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> DeleteCategoryAsync(Guid tenantId, Guid categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == categoryId);
            if (category == null)
                return null;
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> IsCategoryNameExists(Guid tenantId, string name)
        {
            return await _context.Categories.AnyAsync(c => c.TenantId == tenantId && c.Name == name);
        }

        public async Task<bool> IsCategoryExists(Guid tenantId, Guid categoryId)
        {
            return await _context.Categories.AnyAsync(c => c.TenantId == tenantId && c.Id == categoryId);
        }
    }
}


