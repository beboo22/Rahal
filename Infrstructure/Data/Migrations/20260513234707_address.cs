using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class address : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourGuideAddress");

            migrationBuilder.DropTable(
                name: "TravelerAddresses");

            migrationBuilder.DropTable(
                name: "TravelerCompanyAddress");

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "Travelers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Travelers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Travelers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Travelers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "TravelCompanies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "TravelCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TravelCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "TravelCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "TravelCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "TourGuides",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "TourGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "TourGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "TourGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "TourGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "TravelCompanies");

            migrationBuilder.DropColumn(
                name: "City",
                table: "TravelCompanies");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "TravelCompanies");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "TravelCompanies");

            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "TourGuides");

            migrationBuilder.DropColumn(
                name: "City",
                table: "TourGuides");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "TourGuides");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "TourGuides");

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "TravelCompanies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhotoUrl",
                table: "TourGuides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TourGuideAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuideAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourGuideAddress_TourGuides_TourGuideId",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelerId = table.Column<int>(type: "int", nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerAddresses_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalTable: "Travelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerCompanyAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelCompanyId = table.Column<int>(type: "int", nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerCompanyAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerCompanyAddress_TravelCompanies_TravelCompanyId",
                        column: x => x.TravelCompanyId,
                        principalTable: "TravelCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideAddress_TourGuideId",
                table: "TourGuideAddress",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelerAddresses_TravelerId",
                table: "TravelerAddresses",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelerCompanyAddress_TravelCompanyId",
                table: "TravelerCompanyAddress",
                column: "TravelCompanyId");
        }
    }
}
