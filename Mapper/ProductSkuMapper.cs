using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluxifyAPI.DTOs.ProductSku;
using FluxifyAPI.Models;
using System.Text.Json;

namespace FluxifyAPI.Mapper
{
    public static class ProductSkuMapper
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private static string? SerializeAttributes(Dictionary<string, string>? attributes)
        {
            return attributes == null || attributes.Count == 0
                ? null
                : JsonSerializer.Serialize(attributes, JsonOptions);
        }

        private static Dictionary<string, string>? DeserializeAttributes(string? attributesJson)
        {
            if (string.IsNullOrWhiteSpace(attributesJson))
                return null;

            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(attributesJson, JsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        public static ProductSkuDto ToProductSkuDto(this ProductSku productSku)
        {
            return new ProductSkuDto
            {
                Id = productSku.Id,
                ProductId = productSku.ProductId,
                Price = productSku.Price,
                Stock = productSku.Stock,
                Attributes = productSku.Attributes,
                imgUrl = productSku.imgUrl
            };
        }

        public static ProductSku ToProductSkuFromCreateDto(this CreateProductSkuRequestDto createDto, Guid productId)
        {
            return new ProductSku
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Price = createDto.Price,
                Stock = createDto.Stock,
                Attributes = createDto.Attributes,
                imgUrl = createDto.imgUrl ?? ""
            };
        }

        public static ProductSku ToProductSkuFromUpdateDto(this UpdateProductSkuRequestDto updateDto, ProductSku existingProductSku)
        {
            if (updateDto.Price.HasValue)
                existingProductSku.Price = updateDto.Price.Value;
            if (updateDto.Stock.HasValue)
                existingProductSku.Stock = updateDto.Stock.Value;

            if (updateDto.Attributes != null)
                existingProductSku.Attributes = updateDto.Attributes;
            if (updateDto.imgUrl != null)
                existingProductSku.imgUrl = updateDto.imgUrl;
            return existingProductSku;
        }
    }
}

