using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvertBrandsToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HotelSearchHistorys_HotelBrandss_BrandsId",
                table: "HotelSearchHistorys");

            migrationBuilder.DropIndex(
                name: "IX_HotelSearchHistorys_BrandsId",
                table: "HotelSearchHistorys");

            migrationBuilder.DropColumn(
                name: "BrandsId",
                table: "HotelSearchHistorys");

            migrationBuilder.AddColumn<string>(
                name: "Brands",
                table: "HotelSearchHistorys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brands",
                table: "HotelSearchHistorys");

            migrationBuilder.AddColumn<int>(
                name: "BrandsId",
                table: "HotelSearchHistorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HotelSearchHistorys_BrandsId",
                table: "HotelSearchHistorys",
                column: "BrandsId");

            migrationBuilder.AddForeignKey(
                name: "FK_HotelSearchHistorys_HotelBrandss_BrandsId",
                table: "HotelSearchHistorys",
                column: "BrandsId",
                principalTable: "HotelBrandss",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
