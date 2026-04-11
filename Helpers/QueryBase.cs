using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.Helpers
{
    public class QueryBase
    {
        [FromQuery(Name = "page")]
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [FromQuery(Name = "pageSize")]
        [Range(1, 100)]
        public int PageSize { get; set; } = 10;

        [FromQuery(Name = "sortBy")]
        public string? SortBy { get; set; }

        [FromQuery(Name = "sortDir")]
        public string? SortDirection { get; set; }

        [FromQuery(Name = "search")]
        public string? SearchTerm { get; set; }

        public int NormalizedPageNumber => Page > 0 ? Page : 1;

        public int NormalizedPageSize => PageSize <= 0 ? 10 : (PageSize > 100 ? 100 : PageSize);

        public string? NormalizedSearchTerm =>
            string.IsNullOrWhiteSpace(SearchTerm) ? null : SearchTerm.Trim();

        public bool NormalizedIsDescending =>
            string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    }
}
