
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.Helpers
{
    public class QueryTenant
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }

        public bool isDescending { get; set; } = false;

        public string? SearchTerm { get; set; }

        [FromQuery(Name = "page")]
        public int? Page { get; set; }

        [FromQuery(Name = "search")]
        public string? Search { get; set; }

        [FromQuery(Name = "sortDir")]
        public string? SortDirection { get; set; }

        [FromQuery(Name = "isActive")]
        public bool? IsActive { get; set; }

        [FromQuery(Name = "storeName")]
        public string? StoreName { get; set; }

        [FromQuery(Name = "subdomain")]
        public string? Subdomain { get; set; }

        // Normalized values to avoid scattering query parsing logic in repository.
        public int NormalizedPageNumber => Page.HasValue && Page.Value > 0 ? Page.Value : PageNumber;

        public int NormalizedPageSize => PageSize <= 0 ? 10 : (PageSize > 100 ? 100 : PageSize);

        public string? NormalizedSearchTerm => !string.IsNullOrWhiteSpace(Search)
            ? Search.Trim()
            : (string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim());

        public bool NormalizedIsDescending =>
            !string.IsNullOrWhiteSpace(SortDirection)
                ? SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                : isDescending;
    }
}