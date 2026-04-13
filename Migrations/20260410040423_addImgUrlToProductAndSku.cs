using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class addImgUrlToProductAndSku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imgUrls",
                table: "products",
                newName: "img_urls");

            migrationBuilder.RenameColumn(
                name: "imgUrl",
                table: "product_skus",
                newName: "img_url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "img_urls",
                table: "products",
                newName: "imgUrls");

            migrationBuilder.RenameColumn(
                name: "img_url",
                table: "product_skus",
                newName: "imgUrl");
        }
    }
}


