using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Order
{
    public class CheckoutOrderRequestDto
    {
        [Required(ErrorMessage = "Địa chỉ giao hàng không được để trống")]
        public string Address { get; set; } = string.Empty;

        public string? PaymentMethod { get; set; }
    }
}
