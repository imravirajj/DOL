using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleInventoryAndConcurrencyWaitlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "vehicle_models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    make = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_models", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_variants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    variant_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    fuel_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transmission = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ex_showroom_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    colors_available = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_variants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicle_variants_vehicle_models_vehicle_model_id",
                        column: x => x.vehicle_model_id,
                        principalTable: "vehicle_models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vin_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    engine_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    color = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    reserved_by_buyer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reservation_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    confirmed_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicle_stocks_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_stocks_vehicle_variants_vehicle_variant_id",
                        column: x => x.vehicle_variant_id,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "waitlist_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    vehicle_variant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    buyer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    queue_position = table.Column<int>(type: "integer", nullable: false),
                    token_amount_paid = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    allocated_stock_id = table.Column<Guid>(type: "uuid", nullable: true),
                    allocated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_waitlist_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_waitlist_entries_application_users_buyer_id",
                        column: x => x.buyer_id,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_waitlist_entries_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_waitlist_entries_vehicle_variants_vehicle_variant_id",
                        column: x => x.vehicle_variant_id,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_stocks_branch_id_vehicle_variant_id_status",
                table: "vehicle_stocks",
                columns: new[] { "branch_id", "vehicle_variant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_stocks_company_id_vin_number",
                table: "vehicle_stocks",
                columns: new[] { "company_id", "vin_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_stocks_reservation_expires_at",
                table: "vehicle_stocks",
                column: "reservation_expires_at");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_stocks_vehicle_variant_id",
                table: "vehicle_stocks",
                column: "vehicle_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_variants_vehicle_model_id",
                table: "vehicle_variants",
                column: "vehicle_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_branch_id_vehicle_variant_id_queue_position",
                table: "waitlist_entries",
                columns: new[] { "branch_id", "vehicle_variant_id", "queue_position" });

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_buyer_id",
                table: "waitlist_entries",
                column: "buyer_id");

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_company_id_idempotency_key",
                table: "waitlist_entries",
                columns: new[] { "company_id", "idempotency_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_waitlist_entries_vehicle_variant_id",
                table: "waitlist_entries",
                column: "vehicle_variant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicle_stocks");

            migrationBuilder.DropTable(
                name: "waitlist_entries");

            migrationBuilder.DropTable(
                name: "vehicle_variants");

            migrationBuilder.DropTable(
                name: "vehicle_models");
        }
    }
}
