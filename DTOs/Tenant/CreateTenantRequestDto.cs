using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Tenant
{
    public class CreateTenantRequestDto
    {
        [Required(ErrorMessage = "Subdomain không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Subdomain phải từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Subdomain chỉ được chứa chữ cái thường, số và dấu gạch ngang")]
        public string Subdomain { get; set; } = string.Empty;

        [Required(ErrorMessage = "StoreName không được để trống")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "StoreName phải từ 3 đến 100 ký tự")]
        public string StoreName { get; set; } = string.Empty;

        public bool? IsActive { get; set; } = true;
    }
}
