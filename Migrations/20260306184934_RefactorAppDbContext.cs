using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAppDbContext : Migration
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
                name: "FK__carts__customer_id",
                table: "carts");

            migrationBuilder.DropForeignKey(
                name: "FK__categorie__tenan__5EBF139D",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "FK__customers__tenan__59FA5E80",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK__order_ite__order__75A278F5",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK__order_ite__produ__76969D2E",
                table: "order_items");

            migrationBuilder.DropForeignKey(
                name: "FK__orders__customer__71D1E811",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK__orders__tenant_i__70DDC3D8",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK__product_s__produ__product",
                table: "product_sku");

            migrationBuilder.DropForeignKey(
                name: "FK__products__catego__656C112C",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK__products__tenant__6477ECF3",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK__tenants__owner_i__534D60F1",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK__tenants__3213E83FD604089B",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK__products__3213E83FF5931BB4",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK__platform__3213E83FEE65E6A7",
                table: "platform_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK__orders__3213E83F77C92058",
                table: "orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK__order_it__3213E83F4ED53BDE",
                table: "order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK__customer__3213E83F09C610A2",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK__categori__3213E83F78E46108",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK__carts__3213E83F",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK__cart_ite__3213E83FF5F280A7",
                table: "cart_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK__product_sku__id",
                table: "product_sku");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "products");

            migrationBuilder.RenameTable(
                name: "product_sku",
                newName: "product_skus");

            migrationBuilder.RenameIndex(
                name: "UQ__tenants__E956860B0D810F7A",
                table: "tenants",
                newName: "IX_tenants_subdomain");

            migrationBuilder.RenameIndex(
                name: "UQ__platform__AB6E6164334FD397",
                table: "platform_users",
                newName: "IX_platform_users_email");

            migrationBuilder.RenameIndex(
                name: "UQ__customer__9C4479298F86976D",
                table: "customers",
                newName: "IX_customers_tenant_id_email");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "categories",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "stock",
                table: "product_skus",
                newName: "Stock");

            migrationBuilder.RenameIndex(
                name: "IX_product_sku_product_id",
                table: "product_skus",
                newName: "IX_product_skus_product_id");

            migrationBuilder.AlterColumn<string>(
                name: "subdomain",
                table: "tenants",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "store_name",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "tenants",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "tenants",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id",
                table: "products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "products",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "platform_users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "merchant");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "platform_users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "platform_users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "platform_users",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "fullname",
                table: "platform_users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "platform_users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "platform_users",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "platform_users",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "payment_method",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldDefaultValue: "COD");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "orders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "order_items",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "customers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "customers",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "customers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "customers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "categories",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "categories",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "carts",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AddColumn<Guid>(
                name: "tenant_id",
                table: "carts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "cart_items",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "product_skus",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tenants",
                table: "tenants",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_platform_users",
                table: "platform_users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orders",
                table: "orders",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_items",
                table: "order_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_customers",
                table: "customers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_categories",
                table: "categories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_carts",
                table: "carts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cart_items",
                table: "cart_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_skus",
                table: "product_skus",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_customers_tenant_id",
                table: "customers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_carts_tenant_id",
                table: "carts",
                column: "tenant_id");

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
                name: "FK_product_skus_products_product_id",
                table: "product_skus",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_categories_category_id",
                table: "products",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_product_skus_products_product_id",
                table: "product_skus");

            migrationBuilder.DropForeignKey(
                name: "FK_products_categories_category_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_products_tenants_tenant_id",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_tenants_platform_users_owner_id",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tenants",
                table: "tenants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_platform_users",
                table: "platform_users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orders",
                table: "orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_items",
                table: "order_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_customers",
                table: "customers");

            migrationBuilder.DropIndex(
                name: "IX_customers_tenant_id",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_categories",
                table: "categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_carts",
                table: "carts");

            migrationBuilder.DropIndex(
                name: "IX_carts_tenant_id",
                table: "carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cart_items",
                table: "cart_items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_skus",
                table: "product_skus");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "carts");

            migrationBuilder.RenameTable(
                name: "product_skus",
                newName: "product_sku");

            migrationBuilder.RenameIndex(
                name: "IX_tenants_subdomain",
                table: "tenants",
                newName: "UQ__tenants__E956860B0D810F7A");

            migrationBuilder.RenameIndex(
                name: "IX_platform_users_email",
                table: "platform_users",
                newName: "UQ__platform__AB6E6164334FD397");

            migrationBuilder.RenameIndex(
                name: "IX_customers_tenant_id_email",
                table: "customers",
                newName: "UQ__customer__9C4479298F86976D");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "categories",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "product_sku",
                newName: "stock");

            migrationBuilder.RenameIndex(
                name: "IX_product_skus_product_id",
                table: "product_sku",
                newName: "IX_product_sku_product_id");

            migrationBuilder.AlterColumn<string>(
                name: "subdomain",
                table: "tenants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "store_name",
                table: "tenants",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "tenants",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "tenants",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id",
                table: "products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "products",
                type: "bit",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "role",
                table: "platform_users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "merchant",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "platform_users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "platform_users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "platform_users",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "fullname",
                table: "platform_users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "platform_users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "platform_users",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "platform_users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_status",
                table: "orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "payment_method",
                table: "orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                defaultValue: "COD",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "orders",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "order_items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "customers",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "customers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "categories",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "carts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "cart_items",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "product_sku",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK__tenants__3213E83FD604089B",
                table: "tenants",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__products__3213E83FF5931BB4",
                table: "products",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__platform__3213E83FEE65E6A7",
                table: "platform_users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__orders__3213E83F77C92058",
                table: "orders",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__order_it__3213E83F4ED53BDE",
                table: "order_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__customer__3213E83F09C610A2",
                table: "customers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__categori__3213E83F78E46108",
                table: "categories",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__carts__3213E83F",
                table: "carts",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__cart_ite__3213E83FF5F280A7",
                table: "cart_items",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK__product_sku__id",
                table: "product_sku",
                column: "id");

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
                name: "FK__carts__customer_id",
                table: "carts",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__categorie__tenan__5EBF139D",
                table: "categories",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__customers__tenan__59FA5E80",
                table: "customers",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__order_ite__order__75A278F5",
                table: "order_items",
                column: "order_id",
                principalTable: "orders",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__order_ite__produ__76969D2E",
                table: "order_items",
                column: "product_sku_id",
                principalTable: "product_sku",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__orders__customer__71D1E811",
                table: "orders",
                column: "customer_id",
                principalTable: "customers",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__orders__tenant_i__70DDC3D8",
                table: "orders",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__product_s__produ__product",
                table: "product_sku",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__products__catego__656C112C",
                table: "products",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__products__tenant__6477ECF3",
                table: "products",
                column: "tenant_id",
                principalTable: "tenants",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK__tenants__owner_i__534D60F1",
                table: "tenants",
                column: "owner_id",
                principalTable: "platform_users",
                principalColumn: "id");
        }
    }
}


