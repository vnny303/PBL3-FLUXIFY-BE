using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Order
{
    public class CreateOrderRequestDto
    {
        public Guid? CustomerId { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        public Guid AddressId { get; set; }

        public string? PaymentMethod { get; set; }

        public string? PaymentStatus { get; set; }

        [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
        public List<CreateOrderItemRequestDto> OrderItems { get; set; } = new List<CreateOrderItemRequestDto>();
    }
}


