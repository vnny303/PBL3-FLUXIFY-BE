namespace FluxifyAPI.DTOs
{
    public class RegisterCustomerRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}