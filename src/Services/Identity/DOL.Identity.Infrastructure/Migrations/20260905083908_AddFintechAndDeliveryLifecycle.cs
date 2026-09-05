using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFintechAndDeliveryLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuotationId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequiredLoanAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    TenureInMonths = table.Column<int>(type: "integer", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "numeric", nullable: false),
                    PanNumber = table.Column<string>(type: "text", nullable: false),
                    EmploymentType = table.Column<string>(type: "text", nullable: false),
                    SelectedBankName = table.Column<string>(type: "text", nullable: true),
                    ApprovedLoanAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    ApprovedInterestRate = table.Column<decimal>(type: "numeric", nullable: true),
                    MonthlyEmi = table.Column<decimal>(type: "numeric", nullable: true),
                    SanctionLetterNumber = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanApplications_quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestDriveBookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerName = table.Column<string>(type: "text", nullable: false),
                    CustomerPhone = table.Column<string>(type: "text", nullable: false),
                    CustomerEmail = table.Column<string>(type: "text", nullable: false),
                    DrivingLicenseNumber = table.Column<string>(type: "text", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeSlot = table.Column<string>(type: "text", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: false),
                    HomeAddress = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: true),
                    FeedbackNotes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDriveBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestDriveBookings_vehicle_variants_VehicleVariantId",
                        column: x => x.VehicleVariantId,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuotationId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleVariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedStockId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    BookingAmountPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    DownPaymentPaid = table.Column<decimal>(type: "numeric", nullable: false),
                    LoanDisbursedAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DeliveryType = table.Column<int>(type: "integer", nullable: false),
                    DeliveryOtp = table.Column<string>(type: "text", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleOrders_quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleOrders_vehicle_stocks_AllocatedStockId",
                        column: x => x.AllocatedStockId,
                        principalTable: "vehicle_stocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleOrders_vehicle_variants_VehicleVariantId",
                        column: x => x.VehicleVariantId,
                        principalTable: "vehicle_variants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryInspections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleStockId = table.Column<Guid>(type: "uuid", nullable: false),
                    InspectorStaffId = table.Column<Guid>(type: "uuid", nullable: false),
                    OdometerReadingKm = table.Column<int>(type: "integer", nullable: false),
                    BatteryHealthPct = table.Column<int>(type: "integer", nullable: false),
                    ExteriorConditionOk = table.Column<bool>(type: "boolean", nullable: false),
                    InteriorCleanOk = table.Column<bool>(type: "boolean", nullable: false),
                    ToolKitAndSpareWheelOk = table.Column<bool>(type: "boolean", nullable: false),
                    DocumentationOk = table.Column<bool>(type: "boolean", nullable: false),
                    InspectionNotes = table.Column<string>(type: "text", nullable: true),
                    IsCustomerAccepted = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerSignatureUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryInspections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryInspections_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryInspections_vehicle_stocks_VehicleStockId",
                        column: x => x.VehicleStockId,
                        principalTable: "vehicle_stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryInspections_OrderId",
                table: "DeliveryInspections",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryInspections_VehicleStockId",
                table: "DeliveryInspections",
                column: "VehicleStockId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplications_QuotationId",
                table: "LoanApplications",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDriveBookings_VehicleVariantId",
                table: "TestDriveBookings",
                column: "VehicleVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOrders_AllocatedStockId",
                table: "VehicleOrders",
                column: "AllocatedStockId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOrders_QuotationId",
                table: "VehicleOrders",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOrders_VehicleVariantId",
                table: "VehicleOrders",
                column: "VehicleVariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryInspections");

            migrationBuilder.DropTable(
                name: "LoanApplications");

            migrationBuilder.DropTable(
                name: "TestDriveBookings");

            migrationBuilder.DropTable(
                name: "VehicleOrders");
        }
    }
}
