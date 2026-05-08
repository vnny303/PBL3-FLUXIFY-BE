using Microsoft.AspNetCore.Mvc;

namespace FluxifyAPI.Helpers
{
    public class QueryCategory : QueryBase
    {
        private string? _name;
        [FromQuery(Name = "name")]
        public string? Name
        {
            get => _name;
            set => _name = value?.Trim().ToLowerInvariant();
        }
        private string? _description;
        [FromQuery(Name = "description")]
        public string? Description
        {
            get => _description;
            set => _description = value?.Trim().ToLowerInvariant();
        }
        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }
    }
}


