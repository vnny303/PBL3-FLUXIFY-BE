using System.ComponentModel.DataAnnotations.Schema;

namespace FluxifyAPI.Models
{
    [Table("carts")]
    public class Cart
    {
        public Guid id { get; set; }
        public Guid customerId { get; set; }
        public Customer customer { get; set; } = null!;
        public List<CartItem>? cartItems { get; set; } = null!;
    }
}