using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Customer
{
    public class UpdateCustomerRequestDto
    {
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        public string? OldPass { get; set; }

        public string? Password { get; set; }

        public bool? IsActive { get; set; }
    }
}

