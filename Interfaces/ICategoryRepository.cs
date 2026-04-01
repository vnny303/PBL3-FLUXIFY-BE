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
        public Task<List<Category>?> GetCategoriesByTenantAsync(Guid tenantId);
        public Task<Category> CreateCategoryAsync(Guid tenantId, string name, string description);
        public Task<Category?> UpdateCategoryAsync(Guid tenantId, Guid categoryId, string name, string description);
        public Task<Category?> DeleteCategoryAsync(Guid tenantId, Guid categoryId);
    }
}