using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHotel_flightToBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlightOffersId",
                table: "BookingPublicTrips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelsId",
                table: "BookingPublicTrips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FlightOffersId",
                table: "BookingPrivateTrips",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotelsId",
                table: "BookingPrivateTrips",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingPublicTrips_FlightOffersId",
                table: "BookingPublicTrips",
                column: "FlightOffersId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPublicTrips_HotelsId",
                table: "BookingPublicTrips",
                column: "HotelsId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPrivateTrips_FlightOffersId",
                table: "BookingPrivateTrips",
                column: "FlightOffersId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPrivateTrips_HotelsId",
                table: "BookingPrivateTrips",
                column: "HotelsId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPrivateTrips_FlightOffers_FlightOffersId",
                table: "BookingPrivateTrips",
                column: "FlightOffersId",
                principalTable: "FlightOffers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPrivateTrips_Hotels_HotelsId",
                table: "BookingPrivateTrips",
                column: "HotelsId",
                principalTable: "Hotels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPublicTrips_FlightOffers_FlightOffersId",
                table: "BookingPublicTrips",
                column: "FlightOffersId",
                principalTable: "FlightOffers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingPublicTrips_Hotels_HotelsId",
                table: "BookingPublicTrips",
                column: "HotelsId",
                principalTable: "Hotels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingPrivateTrips_FlightOffers_FlightOffersId",
                table: "BookingPrivateTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingPrivateTrips_Hotels_HotelsId",
                table: "BookingPrivateTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingPublicTrips_FlightOffers_FlightOffersId",
                table: "BookingPublicTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_BookingPublicTrips_Hotels_HotelsId",
                table: "BookingPublicTrips");

            migrationBuilder.DropIndex(
                name: "IX_BookingPublicTrips_FlightOffersId",
                table: "BookingPublicTrips");

            migrationBuilder.DropIndex(
                name: "IX_BookingPublicTrips_HotelsId",
                table: "BookingPublicTrips");

            migrationBuilder.DropIndex(
                name: "IX_BookingPrivateTrips_FlightOffersId",
                table: "BookingPrivateTrips");

            migrationBuilder.DropIndex(
                name: "IX_BookingPrivateTrips_HotelsId",
                table: "BookingPrivateTrips");

            migrationBuilder.DropColumn(
                name: "FlightOffersId",
                table: "BookingPublicTrips");

            migrationBuilder.DropColumn(
                name: "HotelsId",
                table: "BookingPublicTrips");

            migrationBuilder.DropColumn(
                name: "FlightOffersId",
                table: "BookingPrivateTrips");

            migrationBuilder.DropColumn(
                name: "HotelsId",
                table: "BookingPrivateTrips");
        }
    }
}
