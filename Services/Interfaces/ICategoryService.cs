using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.Helpers;
using FluxifyAPI.Services.Common;

namespace FluxifyAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<ServiceResult<IEnumerable<CategoryDto>>> GetCategoriesAsync(Guid tenantId, QueryCategory query);
        Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid tenantId, Guid categoryId);
        Task<ServiceResult<CategoryDto>> CreateCategoryAsync(Guid tenantId, Guid platformUserId, CreateCategoryRequestDto createDto);
        Task<ServiceResult<CategoryDto>> UpdateCategoryAsync(Guid tenantId, Guid platformUserId, Guid categoryId, UpdateCategoryRequestDto updateDto);
        Task<ServiceResult<object>> DeleteCategoryAsync(Guid tenantId, Guid platformUserId, Guid categoryId);
    }
}


