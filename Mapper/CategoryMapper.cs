using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.Cartegory;
using FluxifyAPI.DTOs.Product;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class CategoryMapper
    {
        public static CategoryDto ToCategoryDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                TenantId = category.TenantId,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                Products = category.Products?.Select(p => p.ToProductDto()).ToList() ?? new List<ProductDto>()
            };
        }

        public static Category ToCategoryFromCreateDto(this CreateCategoryRequestDto createDto, Guid tenantId)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = createDto.Name.Trim(),
                Description = createDto.Description?.Trim(),
                IsActive = createDto.IsActive ?? true
            };
        }

        public static Category ToCategoryFromUpdateDto(this UpdateCategoryRequestDto updateDto, Category existingCategory)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                existingCategory.Name = updateDto.Name.Trim();
            }

            if (updateDto.Description != null)
            {
                existingCategory.Description = updateDto.Description.Trim();
            }

            if (updateDto.IsActive.HasValue)
            {
                existingCategory.IsActive = updateDto.IsActive.Value;
            }

            return existingCategory;
        }
    }
}

