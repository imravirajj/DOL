using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalQuotationAndRtoTaxSlabs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    buyer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quotation_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    customer_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    customer_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    customer_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    selected_color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ex_showroom_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    rto_tax_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    insurance_base_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    insurance_addons_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    fastag_charges = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tcs_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    accessories_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    extended_warranty_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total_on_road_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    include_zero_dep = table.Column<bool>(type: "boolean", nullable: false),
                    include_engine_protect = table.Column<bool>(type: "boolean", nullable: false),
                    include_return_to_invoice = table.Column<bool>(type: "boolean", nullable: false),
                    include_extended_warranty = table.Column<bool>(type: "boolean", nullable: false),
                    selected_accessories_json = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    valid_until = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quotations_application_users_buyer_id",
                        column: x => x.buyer_id,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_vehicle_variants_vehicle_variant_id",
                        column: x => x.vehicle_variant_id,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rto_tax_slabs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    fuel_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tax_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    cess_percentage = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rto_tax_slabs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quotations_branch_id_status",
                table: "quotations",
                columns: new[] { "branch_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_quotations_buyer_id",
                table: "quotations",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_company_id_quotation_number",
                table: "quotations",
                columns: new[] { "company_id", "quotation_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_quotations_customer_email",
                table: "quotations",
                column: "customer_email");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_vehicle_variant_id",
                table: "quotations",
                column: "vehicle_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_rto_tax_slabs_company_id_state_name_fuel_type",
                table: "rto_tax_slabs",
                columns: new[] { "company_id", "state_name", "fuel_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotations");

            migrationBuilder.DropTable(
                name: "rto_tax_slabs");
        }
    }
}
