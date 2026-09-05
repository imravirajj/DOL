using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEnterpriseDealershipModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerNotifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActionUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerNotifications_application_users_UserId",
                        column: x => x.UserId,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealershipReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ReviewText = table.Column<string>(type: "text", nullable: false),
                    IsVerifiedBuyer = table.Column<bool>(type: "boolean", nullable: false),
                    DealerResponse = table.Column<string>(type: "text", nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealershipReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealershipReviews_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DealershipReviews_application_users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    InsurerName = table.Column<string>(type: "text", nullable: false),
                    PolicyNumber = table.Column<string>(type: "text", nullable: false),
                    PlanType = table.Column<string>(type: "text", nullable: false),
                    PremiumAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IdvAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CoverageStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CoverageEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PolicyDocumentUrl = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePolicies_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAppointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VinNumber = table.Column<string>(type: "text", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: false),
                    VehicleVariantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ServiceType = table.Column<int>(type: "integer", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSlot = table.Column<string>(type: "text", nullable: false),
                    CustomerComments = table.Column<string>(type: "text", nullable: true),
                    EstimatedCost = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualCost = table.Column<decimal>(type: "numeric", nullable: true),
                    WorkshopNotes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceAppointments_vehicle_variants_VehicleVariantId",
                        column: x => x.VehicleVariantId,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleAccessories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PartNumber = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CompatibleVariantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    InstallationCost = table.Column<decimal>(type: "numeric", nullable: false),
                    WarrantyMonths = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleAccessories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleAccessories_vehicle_variants_CompatibleVariantId",
                        column: x => x.CompatibleVariantId,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleTradeIns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Make = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    KilometersDriven = table.Column<int>(type: "integer", nullable: false),
                    FuelType = table.Column<string>(type: "text", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    HasAccidentHistory = table.Column<bool>(type: "boolean", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "text", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "numeric", nullable: false),
                    OfferedValue = table.Column<decimal>(type: "numeric", nullable: true),
                    InspectionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    InspectorNotes = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTradeIns", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNotifications_UserId",
                table: "CustomerNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DealershipReviews_BuyerId",
                table: "DealershipReviews",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_DealershipReviews_OrderId",
                table: "DealershipReviews",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicies_OrderId",
                table: "InsurancePolicies",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAppointments_VehicleVariantId",
                table: "ServiceAppointments",
                column: "VehicleVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAccessories_CompatibleVariantId",
                table: "VehicleAccessories",
                column: "CompatibleVariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerNotifications");

            migrationBuilder.DropTable(
                name: "DealershipReviews");

            migrationBuilder.DropTable(
                name: "InsurancePolicies");

            migrationBuilder.DropTable(
                name: "ServiceAppointments");

            migrationBuilder.DropTable(
                name: "VehicleAccessories");

            migrationBuilder.DropTable(
                name: "VehicleTradeIns");
        }
    }
}
