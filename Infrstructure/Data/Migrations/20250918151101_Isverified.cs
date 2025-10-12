using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Isverified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isverified",
                table: "Travelers");

            migrationBuilder.DropColumn(
                name: "Isverified",
                table: "TravelCompanies");

            migrationBuilder.DropColumn(
                name: "Isverified",
                table: "TourGuides");

            migrationBuilder.AddColumn<bool>(
                name: "Isverified",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Isverified",
                table: "Users");

            migrationBuilder.AddColumn<bool>(
                name: "Isverified",
                table: "Travelers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Isverified",
                table: "TravelCompanies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Isverified",
                table: "TourGuides",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
