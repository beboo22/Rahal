using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TotalEarnings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalEarnings",
                table: "TourGuides",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalEarnings",
                table: "TourGuides");
        }
    }
}
