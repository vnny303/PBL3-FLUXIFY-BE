using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Product
{
    public class ProductImageInputDto
    {
        [Required(ErrorMessage = "Image URL không được để trống")]
        [StringLength(1000, ErrorMessage = "Image URL không được vượt quá 1000 ký tự")]
        [Url(ErrorMessage = "Image URL không đúng định dạng")]
        public string Url { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "SortOrder phải lớn hơn hoặc bằng 0")]
        public int SortOrder { get; set; }
    }
}