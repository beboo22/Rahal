using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SpecificDiscounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "TripId",
                table: "SpecificDiscounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "GenericDiscounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificDiscounts_Code",
                table: "SpecificDiscounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecificDiscounts_TripId",
                table: "SpecificDiscounts",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericDiscounts_Code",
                table: "GenericDiscounts",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificDiscounts_PublicTrips_TripId",
                table: "SpecificDiscounts",
                column: "TripId",
                principalTable: "PublicTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificDiscounts_PublicTrips_TripId",
                table: "SpecificDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_SpecificDiscounts_Code",
                table: "SpecificDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_SpecificDiscounts_TripId",
                table: "SpecificDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_GenericDiscounts_Code",
                table: "GenericDiscounts");

            migrationBuilder.DropColumn(
                name: "TripId",
                table: "SpecificDiscounts");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SpecificDiscounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "GenericDiscounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
