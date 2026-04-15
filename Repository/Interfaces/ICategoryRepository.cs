using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.Models;

namespace FluxifyAPI.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetCategoryAsync(Guid tenantId, Guid categoryId);
        IQueryable<Category> GetCategoriesByTenant(Guid tenantId);
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<Category?> DeleteCategoryAsync(Guid tenantId, Guid categoryId);
        Task<bool> IsCategoryNameExists(Guid tenantId, string name);
        Task<bool> IsCategoryExists(Guid tenantId, Guid categoryId);
    }
}