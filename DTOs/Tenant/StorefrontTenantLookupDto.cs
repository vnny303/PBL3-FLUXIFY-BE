namespace FluxifyAPI.DTOs.Tenant
{
    public class StorefrontTenantLookupDto
    {
        public Guid Id { get; set; }

        public string Subdomain { get; set; } = string.Empty;

        public string StoreName { get; set; } = string.Empty;

        public bool? IsActive { get; set; }

        public StorefrontContentConfigDto ContentConfig { get; set; } = new();

        public StorefrontThemeConfigDto ThemeConfig { get; set; } = new();
    }
}