namespace FluxifyAPI.DTOs.Product
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public bool IsPrimary { get; set; }

        public int SortOrder { get; set; }
    }
}