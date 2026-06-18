using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToRequestTourGuide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestTourGuidePulicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrips");

            migrationBuilder.DropIndex(
                name: "IX_RequestTourGuidePrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePulicTrips_PublicTripId_TourGuideId",
                table: "RequestTourGuidePulicTrips",
                columns: new[] { "PublicTripId", "TourGuideId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePrivateTrips_PrivateTripId_TourGuideId",
                table: "RequestTourGuidePrivateTrips",
                columns: new[] { "PrivateTripId", "TourGuideId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestTourGuidePulicTrips_PublicTripId_TourGuideId",
                table: "RequestTourGuidePulicTrips");

            migrationBuilder.DropIndex(
                name: "IX_RequestTourGuidePrivateTrips_PrivateTripId_TourGuideId",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePulicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrips",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrips",
                column: "PrivateTripId");
        }
    }
}
