using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Helpers;
using FluxifyAPI.Repository.Interfaces;
using FluxifyAPI.Mapper;
using FluxifyAPI.Services.Interfaces;
using FluxifyAPI.Services.Common;
using Microsoft.EntityFrameworkCore;

namespace FluxifyAPI.Services.Implementations {
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(Guid tenantId, QueryCategory query)
        {
            if (query.TenantId.HasValue && query.TenantId.Value != tenantId)
                return ServiceResult<IEnumerable<CategoryDto>>.Fail(400, "tenantId trong query không khớp route");

            var categoryQuery = _categoryRepository.GetCategoriesByTenant(tenantId);

            var searchTerm = query.NormalizedSearchTerm;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (Guid.TryParse(searchTerm, out var categoryId))
                {
                    categoryQuery = categoryQuery.Where(c =>
                        c.Id == categoryId ||
                        c.Name.Contains(searchTerm) ||
                        (c.Description != null && c.Description.Contains(searchTerm)));
                }
                else
                {
                    categoryQuery = categoryQuery.Where(c =>
                        c.Name.Contains(searchTerm) ||
                        (c.Description != null && c.Description.Contains(searchTerm)));
                }
            }

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var name = query.Name.Trim();
                categoryQuery = categoryQuery.Where(c => c.Name.Contains(name));
            }

            if (query.IsActive.HasValue)
                categoryQuery = categoryQuery.Where(c => c.IsActive == query.IsActive.Value);

            var sortBy = query.SortBy?.Trim();
            var isDescending = query.NormalizedIsDescending;
            var normalizedSortBy = sortBy?.ToLowerInvariant();

            if (normalizedSortBy == "name")
                categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.Name) : categoryQuery.OrderBy(c => c.Name);
            else if (normalizedSortBy == "description")
                categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.Description) : categoryQuery.OrderBy(c => c.Description);
            else if (normalizedSortBy == "isactive" || normalizedSortBy == "is_active")
                categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.IsActive) : categoryQuery.OrderBy(c => c.IsActive);
            else if (normalizedSortBy == "id")
                categoryQuery = isDescending ? categoryQuery.OrderByDescending(c => c.Id) : categoryQuery.OrderBy(c => c.Id);
            else
                categoryQuery = categoryQuery.OrderBy(c => c.Id);

            var pageNumber = query.NormalizedPageNumber;
            var pageSize = query.NormalizedPageSize;
            var skipNumber = (pageNumber - 1) * pageSize;

            var categories = await categoryQuery.Skip(skipNumber).Take(pageSize).ToListAsync();

            return ServiceResult<IEnumerable<CategoryDto>>.Ok(categories.Select(c => c.ToCategoryDto()));
        }

        public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(Guid tenantId, CreateCategoryRequestDto createDto)
        {
            var category = createDto.ToCategoryFromCreateDto(tenantId);
            var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
            return ServiceResult<CategoryDto>.Ok(createdCategory.ToCategoryDto());
        }

        public async Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid tenantId, Guid categoryId, UpdateCategoryRequestDto updateDto)
        {
            var category = await _categoryRepository.GetCategoryAsync(tenantId, categoryId);
            if (category == null)
                return ServiceResult<CategoryDto>.Fail(404, "Không tìm thấy danh mục!");

            updateDto.ToCategoryFromUpdateDto(category);
            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(category);
            return ServiceResult<CategoryDto>.Ok(updatedCategory.ToCategoryDto());
        }

        public async Task<ServiceResult<object>> DeleteCategoryAsync(Guid tenantId, Guid categoryId)
        {
            var deletedCategory = await _categoryRepository.DeleteCategoryAsync(tenantId, categoryId);
            if (deletedCategory == null)
                return ServiceResult<object>.Fail(404, "Không tìm thấy danh mục!");

            return ServiceResult<object>.Ok(new { message = "Xóa thành công!" });
        }
    }
}




