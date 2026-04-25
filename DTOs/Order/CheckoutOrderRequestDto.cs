using System;
using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Order
{
    public class CheckoutOrderRequestDto
    {
        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public Guid AddressId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? OrderNote { get; set; }

        [Required(ErrorMessage = "Phương thức vận chuyển không được để trống")]
        [RegularExpression("^(standard|express)$", ErrorMessage = "shippingMethod chỉ nhận standard hoặc express")]
        public string ShippingMethod { get; set; } = "standard";
    }
}
