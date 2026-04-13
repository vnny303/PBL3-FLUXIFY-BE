using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category?> GetCategoryAsync(Guid tenantId, Guid categoryId);
        public IQueryable<Category> GetCategoriesByTenant(Guid tenantId);
        public Task<IEnumerable<Category>?> GetCategoriesByTenantAsync(Guid tenantId);
        public Task<Category> CreateCategoryAsync(Category category);
        public Task<Category> UpdateCategoryAsync(Category category);
        public Task<Category?> DeleteCategoryAsync(Guid tenantId, Guid categoryId);
    }
}