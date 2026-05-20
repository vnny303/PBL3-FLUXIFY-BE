using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    public partial class ProductionHardening : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_created_at",
                table: "orders",
                columns: new[] { "tenant_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_customer_created_at",
                table: "orders",
                columns: new[] { "tenant_id", "customer_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_status",
                table: "orders",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_payment_status",
                table: "orders",
                columns: new[] { "tenant_id", "payment_status" });

            migrationBuilder.CreateIndex(
                name: "IX_products_tenant_category",
                table: "products",
                columns: new[] { "tenant_id", "category_id" });

            migrationBuilder.CreateIndex(
                name: "IX_carts_tenant_customer_unique",
                table: "carts",
                columns: new[] { "tenant_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_cart_product_sku_unique",
                table: "cart_items",
                columns: new[] { "cart_id", "product_sku_id" },
                unique: true);

            migrationBuilder.Sql("ALTER TABLE [orders] ADD CONSTRAINT [CK_orders_amount_non_negative] CHECK ([subtotal] >= 0 AND [shipping_fee] >= 0 AND [tax_amount] >= 0 AND [total_amount] >= 0)");
            migrationBuilder.Sql("ALTER TABLE [order_items] ADD CONSTRAINT [CK_order_items_quantity_positive] CHECK ([quantity] > 0)");
            migrationBuilder.Sql("ALTER TABLE [order_items] ADD CONSTRAINT [CK_order_items_unit_price_non_negative] CHECK ([unit_price] >= 0)");
            migrationBuilder.Sql("ALTER TABLE [cart_items] ADD CONSTRAINT [CK_cart_items_quantity_positive] CHECK ([quantity] > 0)");
            migrationBuilder.Sql("ALTER TABLE [product_skus] ADD CONSTRAINT [CK_product_skus_stock_non_negative] CHECK ([stock] >= 0)");
            migrationBuilder.Sql("ALTER TABLE [product_skus] ADD CONSTRAINT [CK_product_skus_price_non_negative] CHECK ([price] >= 0)");
            migrationBuilder.Sql("ALTER TABLE [reviews] ADD CONSTRAINT [CK_reviews_rating_range] CHECK ([rating] BETWEEN 1 AND 5)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE [reviews] DROP CONSTRAINT [CK_reviews_rating_range]");
            migrationBuilder.Sql("ALTER TABLE [product_skus] DROP CONSTRAINT [CK_product_skus_price_non_negative]");
            migrationBuilder.Sql("ALTER TABLE [product_skus] DROP CONSTRAINT [CK_product_skus_stock_non_negative]");
            migrationBuilder.Sql("ALTER TABLE [cart_items] DROP CONSTRAINT [CK_cart_items_quantity_positive]");
            migrationBuilder.Sql("ALTER TABLE [order_items] DROP CONSTRAINT [CK_order_items_unit_price_non_negative]");
            migrationBuilder.Sql("ALTER TABLE [order_items] DROP CONSTRAINT [CK_order_items_quantity_positive]");
            migrationBuilder.Sql("ALTER TABLE [orders] DROP CONSTRAINT [CK_orders_amount_non_negative]");

            migrationBuilder.DropIndex(name: "IX_cart_items_cart_product_sku_unique", table: "cart_items");
            migrationBuilder.DropIndex(name: "IX_carts_tenant_customer_unique", table: "carts");
            migrationBuilder.DropIndex(name: "IX_products_tenant_category", table: "products");
            migrationBuilder.DropIndex(name: "IX_orders_tenant_payment_status", table: "orders");
            migrationBuilder.DropIndex(name: "IX_orders_tenant_status", table: "orders");
            migrationBuilder.DropIndex(name: "IX_orders_tenant_customer_created_at", table: "orders");
            migrationBuilder.DropIndex(name: "IX_orders_tenant_created_at", table: "orders");
        }
    }
}
