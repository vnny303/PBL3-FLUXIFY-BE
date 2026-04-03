using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Order
{
    public class UpdateOrderStatusRequestDto
    {
        [Required(ErrorMessage = "Trạng thái đơn hàng không được để trống")]
        public string Status { get; set; } = string.Empty;
    }
}
