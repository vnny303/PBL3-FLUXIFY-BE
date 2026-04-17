using System.ComponentModel.DataAnnotations;

namespace FluxifyAPI.DTOs.Tenant
{
    public class StorefrontThemeConfigDto
    {
        public ThemeColorsConfigDto Colors { get; set; } = new();

        public ThemeTypographyConfigDto Typography { get; set; } = new();

        public ThemeLayoutConfigDto Layout { get; set; } = new();

        public ThemeComponentsConfigDto Components { get; set; } = new();
    }

    public class ThemeColorsConfigDto
    {
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Primary phải là mã màu hex hợp lệ")]
        public string? Primary { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Background phải là mã màu hex hợp lệ")]
        public string? Background { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Text phải là mã màu hex hợp lệ")]
        public string? Text { get; set; }
    }

    public class ThemeTypographyConfigDto
    {
        [StringLength(100, ErrorMessage = "FontFamily tối đa 100 ký tự")]
        public string? FontFamily { get; set; }
    }

    public class ThemeLayoutConfigDto
    {
        [Range(0, 128, ErrorMessage = "BorderRadius phải trong khoảng 0 đến 128")]
        public int? BorderRadius { get; set; }
    }

    public class ThemeComponentsConfigDto
    {
        public ThemeSurfaceConfigDto Header { get; set; } = new();

        public ThemeSurfaceConfigDto Footer { get; set; } = new();

        public ThemeProductCardConfigDto ProductCard { get; set; } = new();
    }

    public class ThemeSurfaceConfigDto
    {
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Background phải là mã màu hex hợp lệ")]
        public string? Background { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Text phải là mã màu hex hợp lệ")]
        public string? Text { get; set; }
    }

    public class ThemeProductCardConfigDto
    {
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Background phải là mã màu hex hợp lệ")]
        public string? Background { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Text phải là mã màu hex hợp lệ")]
        public string? Text { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Price phải là mã màu hex hợp lệ")]
        public string? Price { get; set; }

        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Badge phải là mã màu hex hợp lệ")]
        public string? Badge { get; set; }
    }
}