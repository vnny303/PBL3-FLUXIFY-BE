using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryTenant : QueryBase
    {
        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }
        private string? _storeName;

        [FromQuery(Name = "storeName")]
        public string? StoreName
        {
            get => _storeName;
            set => _storeName = value?.Trim().ToLowerInvariant();
        }
        private string? _subdomain;
        [FromQuery(Name = "subdomain")]
        public string? Subdomain
        {
            get => _subdomain;
            set => _subdomain = value?.Trim().ToLowerInvariant();
        }
    }
}


