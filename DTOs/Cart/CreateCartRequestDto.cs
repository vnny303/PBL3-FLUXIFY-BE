namespace FluxifyAPI.DTOs.Cart
{
    public class CreateCartRequestDto
    {
        public Guid? TenantId { get; set; }

        public Guid? CustomerId { get; set; }
    }
}

