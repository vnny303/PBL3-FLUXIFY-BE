using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class ManyThings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_cart_items_cart_id",
                table: "cart_items");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "product_skus",
                newName: "stock");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_reviews_rating_range",
                table: "reviews",
                sql: "[rating] BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_products_tenant_id_category_id",
                table: "products",
                columns: new[] { "tenant_id", "category_id" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_product_skus_price_non_negative",
                table: "product_skus",
                sql: "[price] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_product_skus_stock_non_negative",
                table: "product_skus",
                sql: "[stock] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_id_created_at",
                table: "orders",
                columns: new[] { "tenant_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_id_customer_id_created_at",
                table: "orders",
                columns: new[] { "tenant_id", "customer_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_id_payment_status",
                table: "orders",
                columns: new[] { "tenant_id", "payment_status" });

            migrationBuilder.CreateIndex(
                name: "IX_orders_tenant_id_status",
                table: "orders",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_orders_amount_non_negative",
                table: "orders",
                sql: "[subtotal] >= 0 AND [shipping_fee] >= 0 AND [tax_amount] >= 0 AND [total_amount] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_order_items_quantity_positive",
                table: "order_items",
                sql: "[quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_order_items_unit_price_non_negative",
                table: "order_items",
                sql: "[unit_price] >= 0");

            migrationBuilder.CreateIndex(
                name: "IX_carts_tenant_id_customer_id",
                table: "carts",
                columns: new[] { "tenant_id", "customer_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_cart_id_product_sku_id",
                table: "cart_items",
                columns: new[] { "cart_id", "product_sku_id" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_cart_items_quantity_positive",
                table: "cart_items",
                sql: "[quantity] > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_reviews_rating_range",
                table: "reviews");

            migrationBuilder.DropIndex(
                name: "IX_products_tenant_id_category_id",
                table: "products");

            migrationBuilder.DropCheckConstraint(
                name: "CK_product_skus_price_non_negative",
                table: "product_skus");

            migrationBuilder.DropCheckConstraint(
                name: "CK_product_skus_stock_non_negative",
                table: "product_skus");

            migrationBuilder.DropIndex(
                name: "IX_orders_tenant_id_created_at",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_tenant_id_customer_id_created_at",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_tenant_id_payment_status",
                table: "orders");

            migrationBuilder.DropIndex(
                name: "IX_orders_tenant_id_status",
                table: "orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_orders_amount_non_negative",
                table: "orders");

            migrationBuilder.DropCheckConstraint(
                name: "CK_order_items_quantity_positive",
                table: "order_items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_order_items_unit_price_non_negative",
                table: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_carts_tenant_id_customer_id",
                table: "carts");

            migrationBuilder.DropIndex(
                name: "IX_cart_items_cart_id_product_sku_id",
                table: "cart_items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_cart_items_quantity_positive",
                table: "cart_items");

            migrationBuilder.RenameColumn(
                name: "stock",
                table: "product_skus",
                newName: "Stock");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_cart_items_cart_id",
                table: "cart_items",
                column: "cart_id");
        }
    }
}
