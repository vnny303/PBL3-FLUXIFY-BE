using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryTenant : QueryBase
    {
        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "storeName")]
        public string? StoreName { get; set; }

        [FromQuery(Name = "subdomain")]
        public string? Subdomain { get; set; }
    }
}
