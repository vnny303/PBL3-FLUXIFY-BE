using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Tenant
{
    public class StorefrontContentConfigDto
    {
        public HomeContentConfigDto Home { get; set; } = new();

        public AboutContentConfigDto About { get; set; } = new();
    }

    public class HomeContentConfigDto
    {
        [Url(ErrorMessage = "HeroImageUrl phải là URL hợp lệ")]
        public string? HeroImageUrl { get; set; }

        [Range(0, 1, ErrorMessage = "HeroOverlayOpacity phải trong khoảng 0 đến 1")]
        public double? HeroOverlayOpacity { get; set; }

        [StringLength(200, ErrorMessage = "Title tối đa 200 ký tự")]
        public string? Title { get; set; }

        [StringLength(300, ErrorMessage = "Subtitle tối đa 300 ký tự")]
        public string? Subtitle { get; set; }

        [StringLength(200, ErrorMessage = "FeaturedTitle tối đa 200 ký tự")]
        public string? FeaturedTitle { get; set; }

        [StringLength(300, ErrorMessage = "FeaturedSubtitle tối đa 300 ký tự")]
        public string? FeaturedSubtitle { get; set; }
    }

    public class AboutContentConfigDto
    {
        [StringLength(4000, ErrorMessage = "Story tối đa 4000 ký tự")]
        public string? Story { get; set; }
    }
}