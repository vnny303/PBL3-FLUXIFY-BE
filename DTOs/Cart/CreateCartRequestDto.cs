using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Cart
{
    public class CreateCartRequestDto
    {
        [Required(ErrorMessage = "TenantId không được để trống")]
        public Guid TenantId { get; set; }
        [Required(ErrorMessage = "CustomerId không được để trống")]
        public Guid CustomerId { get; set; }
    }
}

