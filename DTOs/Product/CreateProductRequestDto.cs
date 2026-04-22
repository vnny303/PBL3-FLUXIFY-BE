using System.ComponentModel.DataAnnotations;
using FluxifyAPI.DTOs.ProductSku;

namespace FluxifyAPI.DTOs.Product
{
        public class CreateProductRequestDto
        {
                [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
                [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải từ 2 đến 255 ký tự")]
                public string Name { get; set; } = string.Empty;

                [Required(ErrorMessage = "CategoryId không được để trống")]
                public Guid CategoryId { get; set; }

                public string? Description { get; set; }

                public Dictionary<string, List<string>>? Attributes { get; set; }

                public List<string>? imgUrls { get; set; }

                public List<CreateProductSkuRequestDto> Skus { get; set; } = new List<CreateProductSkuRequestDto>();
        }
}


