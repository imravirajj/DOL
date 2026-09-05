using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantAndBranchIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "access_scope",
                table: "application_users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "branch_id",
                table: "application_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "application_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "scope_entity_id",
                table: "application_users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    subscription_plan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    currency = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    iso_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "state_regions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    country_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    state_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_state_regions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_state_regions_countries_country_id",
                        column: x => x.country_id,
                        principalTable: "countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    state_region_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cities_state_regions_state_region_id",
                        column: x => x.state_region_id,
                        principalTable: "state_regions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    city_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    branch_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contact_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    is_main_branch = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_branches_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_branches_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "application_roles",
                columns: new[] { "Id", "created_at", "description", "name" },
                values: new object[,]
                {
                    { new Guid("c0a80101-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Company Super Administrator", "CompanyAdmin" },
                    { new Guid("c0a80101-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Branch Manager", "BranchManager" },
                    { new Guid("c0a80101-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Branch Staff Member", "BranchStaff" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_application_users_branch_id",
                table: "application_users",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_application_users_company_id",
                table: "application_users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_branches_city_id",
                table: "branches",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_branches_company_id_branch_code",
                table: "branches",
                columns: new[] { "company_id", "branch_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cities_state_region_id_name",
                table: "cities",
                columns: new[] { "state_region_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_code",
                table: "companies",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_countries_company_id_iso_code",
                table: "countries",
                columns: new[] { "company_id", "iso_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_state_regions_country_id_name",
                table: "state_regions",
                columns: new[] { "country_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_application_users_branches_branch_id",
                table: "application_users",
                column: "branch_id",
                principalTable: "branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_application_users_companies_company_id",
                table: "application_users",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_application_users_branches_branch_id",
                table: "application_users");

            migrationBuilder.DropForeignKey(
                name: "FK_application_users_companies_company_id",
                table: "application_users");

            migrationBuilder.DropTable(
                name: "branches");

            migrationBuilder.DropTable(
                name: "cities");

            migrationBuilder.DropTable(
                name: "companies");

            migrationBuilder.DropTable(
                name: "state_regions");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropIndex(
                name: "IX_application_users_branch_id",
                table: "application_users");

            migrationBuilder.DropIndex(
                name: "IX_application_users_company_id",
                table: "application_users");

            migrationBuilder.DeleteData(
                table: "application_roles",
                keyColumn: "Id",
                keyValue: new Guid("c0a80101-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "application_roles",
                keyColumn: "Id",
                keyValue: new Guid("c0a80101-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "application_roles",
                keyColumn: "Id",
                keyValue: new Guid("c0a80101-0000-0000-0000-000000000006"));

            migrationBuilder.DropColumn(
                name: "access_scope",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "branch_id",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "scope_entity_id",
                table: "application_users");
        }
    }
}
