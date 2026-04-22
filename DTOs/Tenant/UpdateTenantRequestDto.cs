using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FluxifyAPI.DTOs.Tenant
{
    public class UpdateTenantRequestDto
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Subdomain phải từ 3 đến 50 ký tự")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Subdomain chỉ được chứa chữ cái thường, số và dấu gạch ngang")]
        public string? Subdomain { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "StoreName phải từ 3 đến 100 ký tự")]
        public string? StoreName { get; set; }

        public bool? IsActive { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtraFields { get; set; }

        public bool ContainsDeprecatedThemePayload()
        {
            if (ExtraFields == null || ExtraFields.Count == 0)
                return false;

            foreach (var key in ExtraFields.Keys)
            {
                if (string.Equals(key, "contentConfig", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(key, "themeConfig", StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}


