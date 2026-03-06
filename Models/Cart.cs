using System.ComponentModel.DataAnnotations.Schema;

namespace FluxifyAPI.Models
{
    [Table("carts")]
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}