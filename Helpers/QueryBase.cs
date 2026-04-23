using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.Helpers
{
    public class QueryBase
    {
        private int _page = 1;
        private int _pageSize = 10;
        private string? _sortBy;
        private string? _sortDirection;
        private string? _searchTerm;

        [FromQuery(Name = "page")]
        [Range(1, int.MaxValue)]
        public int Page
        {
            get => _page;
            set => _page = value > 0 ? value : 1;
        }

        [FromQuery(Name = "pageSize")]
        [Range(1, 100)]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value <= 0 ? 10 : (value > 100 ? 100 : value);
        }

        [FromQuery(Name = "sortBy")]
        public string? SortBy
        {
            get => _sortBy;
            set => _sortBy = string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
        }

        [FromQuery(Name = "sortDir")]
        public string? SortDirection
        {
            get => _sortDirection;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _sortDirection = null;
                    return;
                }

                var normalized = value.Trim().ToLowerInvariant();
                _sortDirection = string.Equals(normalized, "desc", StringComparison.OrdinalIgnoreCase)
                    ? "desc"
                    : "asc";
            }
        }

        [FromQuery(Name = "search")]
        public string? SearchTerm
        {
            get => _searchTerm;
            set => _searchTerm = string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();
        }
    }
}