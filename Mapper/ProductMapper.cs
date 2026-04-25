using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Models;
using System.Text.Json;

namespace FluxifyAPI.Mapper
{
    public static class ProductMapper
    {
        private static (double averageRating, int reviewCount) GetRatingSummary(Product product)
        {
            var reviews = product.ProductSkus
                .SelectMany(ps => ps.Reviews ?? new List<Review>())
                .ToList();

            var reviewCount = reviews.Count;
            var averageRating = reviewCount == 0 ? 0 : Math.Round(reviews.Average(r => r.Rating), 2);
            return (averageRating, reviewCount);
        }

        // Parse sang typed DTO — dùng cho ProductDetailDto
        private static List<DetailSectionDto> ParseDetailSections(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<DetailSectionDto>();
            try { return JsonSerializer.Deserialize<List<DetailSectionDto>>(json) ?? new List<DetailSectionDto>(); }
            catch (JsonException) { return new List<DetailSectionDto>(); }
        }

        private static List<SpecificationDto> ParseSpecifications(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<SpecificationDto>();
            try { return JsonSerializer.Deserialize<List<SpecificationDto>>(json) ?? new List<SpecificationDto>(); }
            catch (JsonException) { return new List<SpecificationDto>(); }
        }

        // Parse sang Dictionary — dùng cho ProductDto (list)
        private static List<Dictionary<string, string>> ParseSectionsRaw(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new List<Dictionary<string, string>>();
            try { return JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json) ?? new List<Dictionary<string, string>>(); }
            catch (JsonException) { return new List<Dictionary<string, string>>(); }
        }

        // ── ToProductDto — dùng cho danh sách sản phẩm ────────────────────

        public static ProductDto ToProductDto(this Product product)
        {
            var (averageRating, reviewCount) = GetRatingSummary(product);

            return new ProductDto
            {
                Id = product.Id,
                TenantId = product.TenantId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Attributes = product.Attributes,
                imgUrls = product.imgUrls,
                AverageRating = averageRating,
                ReviewCount = reviewCount,
                DetailSections = ParseSectionsRaw(product.DetailSections),
                Specifications = ParseSectionsRaw(product.Specifications),
                ProductSkus = product.ProductSkus.Select(ps => ps.ToProductSkuDto()).ToList()
            };
        }

        // ── ToProductDetailDto — dùng cho GET /products/{id} ───────────────

        public static ProductDetailDto ToProductDetailDto(this Product product)
        {
            var (averageRating, reviewCount) = GetRatingSummary(product);

            return new ProductDetailDto
            {
                Id = product.Id,
                TenantId = product.TenantId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                Attributes = product.Attributes,
                imgUrls = product.imgUrls,
                AverageRating = averageRating,
                ReviewCount = reviewCount,
                DetailSections = ParseDetailSections(product.DetailSections),
                Specifications = ParseSpecifications(product.Specifications),
                ProductSkus = product.ProductSkus.Select(ps => ps.ToProductSkuDto()).ToList()
            };
        }

        public static Product ToProductFromCreateDto(
            this CreateProductRequestDto createDto,
            Guid tenantId,
            Dictionary<string, List<string>>? resolvedAttributes = null,
            IEnumerable<CreateProductSkuRequestDto>? resolvedSkus = null)
        {
            var productId = Guid.NewGuid();
            var skuSource = resolvedSkus ?? createDto.Skus ?? new List<CreateProductSkuRequestDto>();

            return new Product
            {
                Id = productId,
                TenantId = tenantId,
                CategoryId = createDto.CategoryId,
                Name = createDto.Name.Trim(),
                Description = createDto.Description?.Trim(),
                Attributes = resolvedAttributes ?? createDto.Attributes,
                imgUrls = createDto.imgUrls ?? new List<string>(),
                DetailSections = createDto.DetailSections == null ? null : JsonSerializer.Serialize(createDto.DetailSections),
                Specifications = createDto.Specifications == null ? null : JsonSerializer.Serialize(createDto.Specifications),
                ProductSkus = skuSource.Select(s => s.ToProductSkuFromCreateDto(productId)).ToList()
            };
        }

        public static Product ToProductFromUpdateDto(this UpdateProductRequestDto updateDto, Product existingProduct)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                existingProduct.Name = updateDto.Name.Trim();

            if (updateDto.CategoryId.HasValue)
                existingProduct.CategoryId = updateDto.CategoryId.Value;

            if (updateDto.Description != null)
                existingProduct.Description = updateDto.Description.Trim();

            if (updateDto.Attributes != null)
                existingProduct.Attributes = updateDto.Attributes;

            if (updateDto.imgUrls != null)
                existingProduct.imgUrls = updateDto.imgUrls;

            if (updateDto.DetailSections != null)
                existingProduct.DetailSections = JsonSerializer.Serialize(updateDto.DetailSections);

            if (updateDto.Specifications != null)
                existingProduct.Specifications = JsonSerializer.Serialize(updateDto.Specifications);

            return existingProduct;
        }
    }
}