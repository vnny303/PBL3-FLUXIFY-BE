using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.ProductSku
{
    public class UpdateProductSkuRequestDto
    {
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Giá không được âm")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không được âm")]
        public int? Stock { get; set; }

        public Dictionary<string, string>? Attributes { get; set; }

        public string? ImgUrl { get; set; }
    }
}


