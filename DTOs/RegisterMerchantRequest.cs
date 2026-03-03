namespace ShopifyAPI.DTOs
{
    public class RegisterMerchantRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
    }
}