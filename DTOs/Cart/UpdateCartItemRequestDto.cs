using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Cart
{
    public class UpdateCartItemRequestDto
    {
        [Required(ErrorMessage = "Quantity là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
