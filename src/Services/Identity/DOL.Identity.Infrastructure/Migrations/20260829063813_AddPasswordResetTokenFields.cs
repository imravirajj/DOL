using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DOL.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokenFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "password_reset_token",
                table: "application_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "password_reset_token_expires_at",
                table: "application_users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_reset_token",
                table: "application_users");

            migrationBuilder.DropColumn(
                name: "password_reset_token_expires_at",
                table: "application_users");
        }
    }
}
