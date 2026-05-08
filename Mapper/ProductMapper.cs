using FluxifyAPI.DTOs.Product;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Models;
using System.Text.Json;

namespace FluxifyAPI.Mapper
{
    public static class ProductMapper
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private static string? SerializeAttributes(Dictionary<string, List<string>>? attributes)
        {
            return attributes == null || attributes.Count == 0
                ? null
                : JsonSerializer.Serialize(attributes, JsonOptions);
        }

        private static Dictionary<string, List<string>>? DeserializeAttributes(string? attributesJson)
        {
            if (string.IsNullOrWhiteSpace(attributesJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(attributesJson, JsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
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
                Attributes = DeserializeAttributes(product.Attributes),
                imgUrls = product.imgUrls,
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

            var product = new Product
            {
                Id = productId,
                TenantId = tenantId,
                CategoryId = createDto.CategoryId,
                Name = createDto.Name.Trim(),
                Description = createDto.Description?.Trim(),
                Attributes = SerializeAttributes(resolvedAttributes ?? createDto.Attributes),
                imgUrls = createDto.imgUrls ?? new List<string>(),
                ProductSkus = skuSource
                    .Select(s => s.ToProductSkuFromCreateDto(productId))
                    .ToList()
            };

            return product;
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
                existingProduct.Attributes = SerializeAttributes(updateDto.Attributes);
            if (updateDto.imgUrls != null)
                existingProduct.imgUrls = updateDto.imgUrls;
            return existingProduct;
        }
    }
}

