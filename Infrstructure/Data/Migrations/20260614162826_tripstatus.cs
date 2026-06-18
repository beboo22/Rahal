using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class tripstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TripStatus",
                table: "PublicTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TripStatus",
                table: "PrivateTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TripStatus",
                table: "PublicTrips");

            migrationBuilder.DropColumn(
                name: "TripStatus",
                table: "PrivateTrips");
        }
    }
}
