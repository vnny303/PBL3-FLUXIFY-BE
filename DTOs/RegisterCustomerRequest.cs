namespace ShopifyAPI.DTOs
{
    public class RegisterCustomerRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty; // Store nào
    }
}