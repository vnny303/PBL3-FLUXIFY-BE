using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCategory : QueryBase
    {
        [FromQuery(Name = "tenantId")]
        public Guid? TenantId { get; set; }

        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }
    }
}
