using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Product
{
        public class UpdateProductRequestDto
        {
                [StringLength(255, MinimumLength = 2, ErrorMessage = "Tên sản phẩm phải từ 2 đến 255 ký tự")]
                public string? Name { get; set; }

                public Guid? CategoryId { get; set; }

                public string? Description { get; set; }

                public Dictionary<string, List<string>>? Attributes { get; set; }

                public List<string>? imgUrls { get; set; }

                public List<DetailSectionDto>? DetailSections { get; set; }

                public List<SpecificationDto>? Specifications { get; set; }

        }
}


