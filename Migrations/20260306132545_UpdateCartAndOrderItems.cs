using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartAndOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__cart_item__custo__7A672E12",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK__cart_item__produ__7B5B524B",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK__order_ite__produ__76969D2E",
                table: "order_items");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "order_items",
                newName: "product_sku_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                newName: "IX_order_items_product_sku_id");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "cart_items",
                newName: "product_sku_id");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "cart_items",
                newName: "cart_id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_product_id",
                table: "cart_items",
                newName: "IX_cart_items_product_sku_id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_customer_id",
                table: "cart_items",
                newName: "IX_cart_items_cart_id");

            migrationBuilder.CreateTable(
                name: "carts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__carts__3213E83F", x => x.id);
                    table.ForeignKey(
                        name: "FK__carts__customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_carts_customer_id",
                table: "carts",
                column: "customer_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK__cart_item__custo__7A672E12",
                table: "cart_items",
                column: "cart_id",
                principalTable: "carts",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__cart_item__produ__7B5B524B",
                table: "cart_items",
                column: "product_sku_id",
                principalTable: "product_sku",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__order_ite__produ__76969D2E",
                table: "order_items",
                column: "product_sku_id",
                principalTable: "product_sku",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__cart_item__custo__7A672E12",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK__cart_item__produ__7B5B524B",
                table: "cart_items");

            migrationBuilder.DropForeignKey(
                name: "FK__order_ite__produ__76969D2E",
                table: "order_items");

            migrationBuilder.DropTable(
                name: "carts");

            migrationBuilder.RenameColumn(
                name: "product_sku_id",
                table: "order_items",
                newName: "product_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_items_product_sku_id",
                table: "order_items",
                newName: "IX_order_items_product_id");

            migrationBuilder.RenameColumn(
                name: "product_sku_id",
                table: "cart_items",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "cart_id",
                table: "cart_items",
                newName: "customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_product_sku_id",
                table: "cart_items",
                newName: "IX_cart_items_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_items_cart_id",
                table: "cart_items",
                newName: "IX_cart_items_customer_id");

            migrationBuilder.AddForeignKey(
                name: "FK__cart_item__custo__7A672E12",
                table: "cart_items",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__cart_item__produ__7B5B524B",
                table: "cart_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__order_ite__produ__76969D2E",
                table: "order_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id");
        }
    }
}
