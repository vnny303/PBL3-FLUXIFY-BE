using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChangesAfterPull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_carts_cart_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_product_skus_product_sku_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_carts_customers_customer_id",
                table: "carts");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_tenants_tenant_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_tenants_tenant_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_product_skus_product_sku_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_customers_customer_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_tenants_tenant_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_platform_users_owner_id",
                table: "tenants");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_carts_cart_id",
                table: "cart_items",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_product_skus_product_sku_id",
                table: "cart_items",
                column: "product_sku_id",
                principalTable: "product_skus",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_carts_customers_customer_id",
                table: "carts",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_carts_tenants_tenant_id",
                table: "carts",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_tenants_tenant_id",
                table: "categories",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_tenants_tenant_id",
                table: "customers",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_product_skus_product_sku_id",
                table: "order_items",
                column: "product_sku_id",
                principalTable: "product_skus",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_tenants_tenant_id",
                table: "orders",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_platform_users_owner_id",
                table: "tenants",
                column: "owner_id",
                principalTable: "platform_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_carts_cart_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_cart_items_product_skus_product_sku_id",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK_carts_customers_customer_id",
                table: "carts");

            migrationBuilder.DropForeignKey(
                name: "FK_carts_tenants_tenant_id",
                table: "carts");

            migrationBuilder.DropForeignKey(
                name: "FK_categories_tenants_tenant_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_tenants_tenant_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_order_items_product_skus_product_sku_id",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_customers_customer_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_tenants_tenant_id",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_platform_users_owner_id",
                table: "tenants");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_carts_cart_id",
                table: "cart_items",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cart_items_product_skus_product_sku_id",
                table: "cart_items",
                column: "product_sku_id",
                principalTable: "product_skus",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_carts_customers_customer_id",
                table: "carts",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_categories_tenants_tenant_id",
                table: "categories",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_tenants_tenant_id",
                table: "customers",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_orders_order_id",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_order_items_product_skus_product_sku_id",
                table: "order_items",
                column: "product_sku_id",
                principalTable: "product_skus",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customers_customer_id",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_tenants_tenant_id",
                table: "orders",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tenants_platform_users_owner_id",
                table: "tenants",
                column: "owner_id",
                principalTable: "platform_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
