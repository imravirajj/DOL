using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinalEnterpriseModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    DocumentNumber = table.Column<string>(type: "text", nullable: false),
                    FileUrl = table.Column<string>(type: "text", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    VerificationStatus = table.Column<int>(type: "integer", nullable: false),
                    VerifiedByStaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RejectionReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerDocuments_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomerDocuments_application_users_UserId",
                        column: x => x.UserId,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EvChargingStations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    StationName = table.Column<string>(type: "text", nullable: false),
                    LocationAddress = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    ConnectorType = table.Column<string>(type: "text", nullable: false),
                    PowerKw = table.Column<int>(type: "integer", nullable: false),
                    TariffPerKwh = table.Column<decimal>(type: "numeric", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvChargingStations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EvChargingStations_branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "branches",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HomeChargerInstallations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    InstallationAddress = table.Column<string>(type: "text", nullable: false),
                    PreferredSurveyDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChargerModel = table.Column<string>(type: "text", nullable: false),
                    SurveyStatus = table.Column<int>(type: "integer", nullable: false),
                    TechnicianNotes = table.Column<string>(type: "text", nullable: true),
                    InstalledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeChargerInstallations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HomeChargerInstallations_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HomeChargerInstallations_application_users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuotationId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionReference = table.Column<string>(type: "text", nullable: false),
                    GatewayProvider = table.Column<string>(type: "text", nullable: false),
                    GatewayPaymentId = table.Column<string>(type: "text", nullable: true),
                    GatewayOrderId = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Purpose = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentMode = table.Column<string>(type: "text", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    ReceiptUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_application_users_BuyerId",
                        column: x => x.BuyerId,
                        principalTable: "application_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesLeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedStaffId = table.Column<Guid>(type: "uuid", nullable: true),
                    InterestedModelId = table.Column<Guid>(type: "uuid", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: false),
                    CustomerPhone = table.Column<string>(type: "text", nullable: false),
                    CustomerEmail = table.Column<string>(type: "text", nullable: true),
                    LeadSource = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    NextFollowUpDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LostReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesLeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesLeads_application_users_AssignedStaffId",
                        column: x => x.AssignedStaffId,
                        principalTable: "application_users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SalesLeads_vehicle_models_InterestedModelId",
                        column: x => x.InterestedModelId,
                        principalTable: "vehicle_models",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WarrantyPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PackageType = table.Column<int>(type: "integer", nullable: false),
                    DurationMonths = table.Column<int>(type: "integer", nullable: false),
                    KilometerLimit = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleWarrantySubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    BuyerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    WarrantyPackageId = table.Column<Guid>(type: "uuid", nullable: false),
                    VinNumber = table.Column<string>(type: "text", nullable: false),
                    SubscriptionNumber = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PricePaid = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleWarrantySubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleWarrantySubscriptions_VehicleOrders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "VehicleOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleWarrantySubscriptions_WarrantyPackages_WarrantyPacka~",
                        column: x => x.WarrantyPackageId,
                        principalTable: "WarrantyPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDocuments_OrderId",
                table: "CustomerDocuments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerDocuments_UserId",
                table: "CustomerDocuments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EvChargingStations_BranchId",
                table: "EvChargingStations",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeChargerInstallations_BuyerId",
                table: "HomeChargerInstallations",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_HomeChargerInstallations_OrderId",
                table: "HomeChargerInstallations",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_BuyerId",
                table: "PaymentTransactions",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesLeads_AssignedStaffId",
                table: "SalesLeads",
                column: "AssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesLeads_InterestedModelId",
                table: "SalesLeads",
                column: "InterestedModelId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleWarrantySubscriptions_OrderId",
                table: "VehicleWarrantySubscriptions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleWarrantySubscriptions_WarrantyPackageId",
                table: "VehicleWarrantySubscriptions",
                column: "WarrantyPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerDocuments");

            migrationBuilder.DropTable(
                name: "EvChargingStations");

            migrationBuilder.DropTable(
                name: "HomeChargerInstallations");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "SalesLeads");

            migrationBuilder.DropTable(
                name: "VehicleWarrantySubscriptions");

            migrationBuilder.DropTable(
                name: "WarrantyPackages");
        }
    }
}
