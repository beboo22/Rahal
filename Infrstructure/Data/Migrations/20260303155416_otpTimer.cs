using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class otpTimer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastPasswordResetTime",
                table: "Users",
                newName: "NextOtpAllowedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOtpRequestTime",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OtpRequestCount",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastOtpRequestTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "OtpRequestCount",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "NextOtpAllowedAt",
                table: "Users",
                newName: "LastPasswordResetTime");
        }
    }
}
