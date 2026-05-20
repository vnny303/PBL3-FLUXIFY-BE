using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluxifyAPI.Migrations
{
    /// <summary>
    /// Removes bank transfer / QR payment configuration because Fluxify now supports COD-only checkout.
    /// Existing orders keep payment_method and payment_status, but QR-specific reference/content columns are removed.
    /// </summary>
    public partial class RemoveQrPaymentAndBankTransfer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenant_payment_settings");

            migrationBuilder.DropColumn(
                name: "payment_reference",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "transfer_content",
                table: "orders");

            migrationBuilder.AddCheckConstraint(
                name: "CK_orders_payment_method_cod_only",
                table: "orders",
                sql: "[payment_method] IS NULL OR [payment_method] = 'COD'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_orders_payment_method_cod_only",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "payment_reference",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "transfer_content",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tenant_payment_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    bank_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bank_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bank_account_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bank_account_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_payment_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_tenant_payment_settings_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_settings_tenant_id_is_active",
                table: "tenant_payment_settings",
                columns: new[] { "tenant_id", "is_active" });
        }
    }
}
