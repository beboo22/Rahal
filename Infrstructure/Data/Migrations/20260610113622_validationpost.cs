using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class validationpost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_PrivateTrips_PrivateTripId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PrivateTrips_PrivateTripId1",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PublicTrips_PublicTripId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PublicTrips_PublicTripId1",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Users_RevieweeId",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "RevieweeId",
                table: "Review",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "PublicTripId1",
                table: "Review",
                newName: "TourGuideId");

            migrationBuilder.RenameColumn(
                name: "PrivateTripId1",
                table: "Review",
                newName: "HotelId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_RevieweeId",
                table: "Review",
                newName: "IX_Review_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_PublicTripId1",
                table: "Review",
                newName: "IX_Review_TourGuideId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_PrivateTripId1",
                table: "Review",
                newName: "IX_Review_HotelId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TourGuideBusinessGalleries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TourGuideBusinessGalleries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "HiringPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "ExperiencePosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_ExperiencePosts_postId",
                table: "Likes",
                column: "postId",
                principalTable: "ExperiencePosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Hotels_HotelId",
                table: "Review",
                column: "HotelId",
                principalTable: "Hotels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PrivateTrips_PrivateTripId",
                table: "Review",
                column: "PrivateTripId",
                principalTable: "PrivateTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PublicTrips_PublicTripId",
                table: "Review",
                column: "PublicTripId",
                principalTable: "PublicTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_TourGuides_TourGuideId",
                table: "Review",
                column: "TourGuideId",
                principalTable: "TourGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Users_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Likes_ExperiencePosts_postId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Hotels_HotelId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PrivateTrips_PrivateTripId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_PublicTrips_PublicTripId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_TourGuides_TourGuideId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Users_UserId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TourGuideBusinessGalleries");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TourGuideBusinessGalleries");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "HiringPosts");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "ExperiencePosts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Review",
                newName: "RevieweeId");

            migrationBuilder.RenameColumn(
                name: "TourGuideId",
                table: "Review",
                newName: "PublicTripId1");

            migrationBuilder.RenameColumn(
                name: "HotelId",
                table: "Review",
                newName: "PrivateTripId1");

            migrationBuilder.RenameIndex(
                name: "IX_Review_UserId",
                table: "Review",
                newName: "IX_Review_RevieweeId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_TourGuideId",
                table: "Review",
                newName: "IX_Review_PublicTripId1");

            migrationBuilder.RenameIndex(
                name: "IX_Review_HotelId",
                table: "Review",
                newName: "IX_Review_PrivateTripId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PrivateTrips_PrivateTripId",
                table: "Review",
                column: "PrivateTripId",
                principalTable: "PrivateTrips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PrivateTrips_PrivateTripId1",
                table: "Review",
                column: "PrivateTripId1",
                principalTable: "PrivateTrips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PublicTrips_PublicTripId",
                table: "Review",
                column: "PublicTripId",
                principalTable: "PublicTrips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_PublicTrips_PublicTripId1",
                table: "Review",
                column: "PublicTripId1",
                principalTable: "PublicTrips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Users_RevieweeId",
                table: "Review",
                column: "RevieweeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
