using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Cart
{
    public class CreateCartItemRequestDto
    {
        [Required(ErrorMessage = "ProductSkuId không được để trống")]
        public Guid ProductSkuId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;
    }
}


