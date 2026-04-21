using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaytoHotel_Flight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "ExperiencePosts");

            migrationBuilder.DropColumn(
                name: "TipsAndRecommendations",
                table: "ExperiencePosts");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "HiringPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "HiringPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "HiringPosts");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "HiringPosts");

            migrationBuilder.AddColumn<decimal>(
                name: "Budget",
                table: "ExperiencePosts",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipsAndRecommendations",
                table: "ExperiencePosts",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
