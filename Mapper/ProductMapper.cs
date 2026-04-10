using FluxifyAPI.DTOs.Product;
using FluxifyAPI.Models;

namespace FluxifyAPI.Mapper
{
    public static class ProductMapper
    {
        private static List<ProductImage> BuildProductImages(IEnumerable<ProductImageInputDto> images, Guid productId)
        {
            var mappedImages = images
                .Select(i => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    ProductId = productId,
                    Url = i.Url.Trim(),
                    IsPrimary = i.IsPrimary,
                    SortOrder = i.SortOrder,
                    CreatedAt = DateTime.UtcNow
                })
                .OrderBy(i => i.SortOrder)
                .ThenBy(i => i.CreatedAt)
                .ToList();

            if (mappedImages.Count > 0 && !mappedImages.Any(i => i.IsPrimary))
            {
                mappedImages[0].IsPrimary = true;
            }

            if (mappedImages.Count(i => i.IsPrimary) > 1)
            {
                var firstPrimary = mappedImages.First(i => i.IsPrimary);
                foreach (var image in mappedImages)
                {
                    image.IsPrimary = image.Id == firstPrimary.Id;
                }
            }

            return mappedImages;
        }

        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                TenantId = product.TenantId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Attributes = product.Attributes,
                imgUrls = product.imgUrls,
                ProductSkus = product.ProductSkus.Select(ps => ps.ToProductSkuDto()).ToList()
            };
        }

        public static Product ToProductFromCreateDto(this CreateProductRequestDto createDto, Guid tenantId)
        {
            var productId = Guid.NewGuid();

            var product = new Product
            {
                Id = productId,
                TenantId = tenantId,
                CategoryId = createDto.CategoryId,
                Name = createDto.Name.Trim(),
                Description = createDto.Description?.Trim(),
                Attributes = createDto.Attributes,
                imgUrls = createDto.imgUrls ?? new List<string>(),
                ProductSkus = createDto.Skus
                    .Select(s => s.ToProductSkuFromCreateDto(productId))
                    .ToList()
            };

            return product;
        }

        public static Product ToProductFromUpdateDto(this UpdateProductRequestDto updateDto, Product existingProduct)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                existingProduct.Name = updateDto.Name.Trim();
            }

            if (updateDto.CategoryId.HasValue)
            {
                existingProduct.CategoryId = updateDto.CategoryId.Value;
            }

            if (updateDto.Description != null)
            {
                existingProduct.Description = updateDto.Description.Trim();
            }

            if (updateDto.Attributes != null)
            {
                existingProduct.Attributes = updateDto.Attributes;
            }

            if (updateDto.imgUrls != null)
            {
                existingProduct.imgUrls = updateDto.imgUrls;
            }

            return existingProduct;
        }
    }
}