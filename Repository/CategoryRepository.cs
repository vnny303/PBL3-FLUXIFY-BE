using FluxifyAPI.Data;
using FluxifyAPI.Interfaces;
using FluxifyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Repository
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
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == categoryId);
        }

        public async Task<List<Category>?> GetCategoriesByTenantAsync(Guid tenantId)
        {
            return await _context.Categories
                .Where(c => c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<Category> CreateCategoryAsync(Guid tenantId, string name, string description)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = name,
                Description = description,
                IsActive = true
            };

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> UpdateCategoryAsync(Guid tenantId, Guid categoryId, string name, string description)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == categoryId);

            if (category == null)
            {
                return null;
            }

            category.Name = name;
            category.Description = description;

            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category?> DeleteCategoryAsync(Guid tenantId, Guid categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && c.Id == categoryId);

            if (category == null)
            {
                return null;
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return category;
        }
    }
}
