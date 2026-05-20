using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Order
{
    public class UpdateOrderStatusRequestDto
    {
        [Required(ErrorMessage = "Trạng thái đơn hàng không được để trống")]
        [RegularExpression("^(Pending|Confirmed|Processing|Shipping|Shipped|Delivered|Completed|Cancelled|Canceled)$", ErrorMessage = "Trạng thái đơn hàng không hợp lệ")]
        public string Status { get; set; } = string.Empty;
    }
}
