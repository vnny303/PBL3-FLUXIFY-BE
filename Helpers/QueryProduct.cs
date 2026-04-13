using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryProduct : QueryBase
    {
        [FromQuery(Name = "tenantId")]
        public Guid? TenantId { get; set; }

        [FromQuery(Name = "categoryId")]
        public Guid? CategoryId { get; set; }

        [FromQuery(Name = "name")]
        public string? Name { get; set; }

        [FromQuery(Name = "hasAttributes")]
        public bool? HasAttributes { get; set; }
    }
}


