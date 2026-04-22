using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.ProductSku
{
    public class UpdateProductSkuRequestDto
    {
        [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
        public int? Stock { get; set; }

        public Dictionary<string, string>? Attributes { get; set; }

        public string? imgUrl { get; set; }
    }
}


