using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class SwitchReviewToProductSku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    product_sku_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviews_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_product_skus_product_sku_id",
                        column: x => x.product_sku_id,
                        principalTable: "product_skus",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_customer_id",
                table: "reviews",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_product_sku_id",
                table: "reviews",
                column: "product_sku_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_tenant_id",
                table: "reviews",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_tenant_id_customer_id",
                table: "reviews",
                columns: new[] { "tenant_id", "customer_id" });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_tenant_id_product_sku_id",
                table: "reviews",
                columns: new[] { "tenant_id", "product_sku_id" });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_tenant_id_product_sku_id_customer_id",
                table: "reviews",
                columns: new[] { "tenant_id", "product_sku_id", "customer_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reviews");
        }
    }
}
