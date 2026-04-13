using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Order
{
    public class CreateOrderItemRequestDto
    {
        [Required(ErrorMessage = "ProductSkuId không được để trống")]
        public Guid ProductSkuId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá không được âm")]
        public decimal UnitPrice { get; set; }
    }
}


