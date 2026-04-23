using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixOwnedTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightSearchHistorys_PriceInsightss_PriceInsightsId",
                table: "FlightSearchHistorys");

            migrationBuilder.DropTable(
                name: "PriceInsightss");

            migrationBuilder.DropIndex(
                name: "IX_FlightSearchHistorys_PriceInsightsId",
                table: "FlightSearchHistorys");

            migrationBuilder.DropColumn(
                name: "Brands",
                table: "HotelSearchHistorys");

            migrationBuilder.DropColumn(
                name: "ArrivalAirport_CreatedAt",
                table: "FlightSegments");

            migrationBuilder.DropColumn(
                name: "ArrivalAirport_UpdatedAt",
                table: "FlightSegments");

            migrationBuilder.DropColumn(
                name: "DepartureAirport_CreatedAt",
                table: "FlightSegments");

            migrationBuilder.DropColumn(
                name: "DepartureAirport_UpdatedAt",
                table: "FlightSegments");

            migrationBuilder.DropColumn(
                name: "PriceInsightsId",
                table: "FlightSearchHistorys");

            migrationBuilder.AlterColumn<string>(
                name: "TravelClass",
                table: "FlightSegments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FlightNumber",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DepartureAirportName",
                table: "FlightSegments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DepartureAirportCode",
                table: "FlightSegments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ArrivalAirportName",
                table: "FlightSegments",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ArrivalAirportCode",
                table: "FlightSegments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Airline",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceInsights_LowestPrice",
                table: "FlightSearchHistorys",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PriceInsights_PriceLevel",
                table: "FlightSearchHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceInsights_LowestPrice",
                table: "FlightSearchHistorys");

            migrationBuilder.DropColumn(
                name: "PriceInsights_PriceLevel",
                table: "FlightSearchHistorys");

            migrationBuilder.AddColumn<string>(
                name: "Brands",
                table: "HotelSearchHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "TravelClass",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "FlightNumber",
                table: "FlightSegments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DepartureAirportName",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "DepartureAirportCode",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ArrivalAirportName",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "ArrivalAirportCode",
                table: "FlightSegments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Airline",
                table: "FlightSegments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalAirport_CreatedAt",
                table: "FlightSegments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalAirport_UpdatedAt",
                table: "FlightSegments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureAirport_CreatedAt",
                table: "FlightSegments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureAirport_UpdatedAt",
                table: "FlightSegments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PriceInsightsId",
                table: "FlightSearchHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PriceInsightss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LowestPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceInsightss", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightSearchHistorys_PriceInsightsId",
                table: "FlightSearchHistorys",
                column: "PriceInsightsId");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightSearchHistorys_PriceInsightss_PriceInsightsId",
                table: "FlightSearchHistorys",
                column: "PriceInsightsId",
                principalTable: "PriceInsightss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
