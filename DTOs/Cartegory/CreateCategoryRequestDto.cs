using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Cartegory
{
    public class CreateCategoryRequestDto
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2 đến 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}
