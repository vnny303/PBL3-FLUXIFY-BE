using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "address",
                table: "orders");

            migrationBuilder.AddColumn<Guid>(
                name: "address_id",
                table: "orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "customer_addresses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    customer_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    receiver_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    province = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    district = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ward = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    street_address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_addresses", x => x.id);
                    table.ForeignKey(
                        name: "FK_customer_addresses_customers_customer_id",
                        column: x => x.customer_id,
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_customer_addresses_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_address_id",
                table: "orders",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_addresses_customer_id",
                table: "customer_addresses",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_customer_addresses_tenant_id",
                table: "customer_addresses",
                column: "tenant_id");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_customer_addresses_address_id",
                table: "orders",
                column: "address_id",
                principalTable: "customer_addresses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_customer_addresses_address_id",
                table: "orders");

            migrationBuilder.DropTable(
                name: "customer_addresses");

            migrationBuilder.DropIndex(
                name: "IX_orders_address_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "address_id",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
