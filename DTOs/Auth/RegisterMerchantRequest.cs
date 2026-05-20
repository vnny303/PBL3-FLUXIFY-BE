using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs
{
    public class RegisterMerchantRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên cửa hàng là bắt buộc")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên cửa hàng phải từ 2 đến 100 ký tự")]
        public string StoreName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subdomain là bắt buộc")]
        [RegularExpression("^[a-z0-9-]{3,63}$", ErrorMessage = "Subdomain chỉ gồm chữ thường, số, dấu gạch ngang và dài 3-63 ký tự")]
        public string Subdomain { get; set; } = string.Empty;
    }
}
