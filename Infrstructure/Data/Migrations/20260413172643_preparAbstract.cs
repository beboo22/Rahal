using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class preparAbstract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePrivateTrip_PrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePrivateTrip_TourGuides_TourGuideId",
                table: "RequestTourGuidePrivateTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePulicTrip_PublicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePulicTrip_TourGuides_TourGuideId",
                table: "RequestTourGuidePulicTrip");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_PrivateTrips_PrivateTripId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_PublicTrips_PublicTripId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_RevieweeId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_ReviewerId",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "BookingTrips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTourGuidePulicTrip",
                table: "RequestTourGuidePulicTrip");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTourGuidePrivateTrip",
                table: "RequestTourGuidePrivateTrip");

            migrationBuilder.RenameTable(
                name: "Reviews",
                newName: "Review");

            migrationBuilder.RenameTable(
                name: "RequestTourGuidePulicTrip",
                newName: "RequestTourGuidePulicTrips");

            migrationBuilder.RenameTable(
                name: "RequestTourGuidePrivateTrip",
                newName: "RequestTourGuidePrivateTrips");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Review",
                newName: "IX_Review_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_RevieweeId",
                table: "Review",
                newName: "IX_Review_RevieweeId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_PublicTripId",
                table: "Review",
                newName: "IX_Review_PublicTripId");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_PrivateTripId",
                table: "Review",
                newName: "IX_Review_PrivateTripId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePulicTrip_TourGuideId",
                table: "RequestTourGuidePulicTrips",
                newName: "IX_RequestTourGuidePulicTrips_TourGuideId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePulicTrip_PublicTripId",
                table: "RequestTourGuidePulicTrips",
                newName: "IX_RequestTourGuidePulicTrips_PublicTripId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePrivateTrip_TourGuideId",
                table: "RequestTourGuidePrivateTrips",
                newName: "IX_RequestTourGuidePrivateTrips_TourGuideId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePrivateTrip_PrivateTripId",
                table: "RequestTourGuidePrivateTrips",
                newName: "IX_RequestTourGuidePrivateTrips_PrivateTripId");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Review",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PrivateTripId1",
                table: "Review",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicTripId1",
                table: "Review",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Accept",
                table: "RequestTourGuidePulicTrips",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "RequestTourGuidePulicTrips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Accept",
                table: "RequestTourGuidePrivateTrips",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcceptedAt",
                table: "RequestTourGuidePrivateTrips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Review",
                table: "Review",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTourGuidePulicTrips",
                table: "RequestTourGuidePulicTrips",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTourGuidePrivateTrips",
                table: "RequestTourGuidePrivateTrips",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BookingPrivateTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrivateTripId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalBookingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOwnerProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Canceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPrivateTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPrivateTrips_PrivateTrips_PrivateTripId",
                        column: x => x.PrivateTripId,
                        principalTable: "PrivateTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingPrivateTrips_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingPublicTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicTripId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalBookingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOwnerProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    Canceled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPublicTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingPublicTrips_PublicTrips_PublicTripId",
                        column: x => x.PublicTripId,
                        principalTable: "PublicTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingPublicTrips_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_PrivateTripId1",
                table: "Review",
                column: "PrivateTripId1");

            migrationBuilder.CreateIndex(
                name: "IX_Review_PublicTripId1",
                table: "Review",
                column: "PublicTripId1");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPrivateTrips_PrivateTripId",
                table: "BookingPrivateTrips",
                column: "PrivateTripId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPrivateTrips_UserId",
                table: "BookingPrivateTrips",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPublicTrips_PublicTripId",
                table: "BookingPublicTrips",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPublicTrips_UserId",
                table: "BookingPublicTrips",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePrivateTrips_PrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrips",
                column: "PrivateTripId",
                principalTable: "PrivateTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePrivateTrips_TourGuides_TourGuideId",
                table: "RequestTourGuidePrivateTrips",
                column: "TourGuideId",
                principalTable: "TourGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePulicTrips_PublicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrips",
                column: "PublicTripId",
                principalTable: "PublicTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePulicTrips_TourGuides_TourGuideId",
                table: "RequestTourGuidePulicTrips",
                column: "TourGuideId",
                principalTable: "TourGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Users_ReviewerId",
                table: "Review",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePrivateTrips_PrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePrivateTrips_TourGuides_TourGuideId",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePulicTrips_PublicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrips");

            migrationBuilder.DropForeignKey(
                name: "FK_RequestTourGuidePulicTrips_TourGuides_TourGuideId",
                table: "RequestTourGuidePulicTrips");

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

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Users_ReviewerId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "BookingPrivateTrips");

            migrationBuilder.DropTable(
                name: "BookingPublicTrips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Review",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_PrivateTripId1",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_PublicTripId1",
                table: "Review");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTourGuidePulicTrips",
                table: "RequestTourGuidePulicTrips");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestTourGuidePrivateTrips",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "PrivateTripId1",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "PublicTripId1",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Accept",
                table: "RequestTourGuidePulicTrips");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "RequestTourGuidePulicTrips");

            migrationBuilder.DropColumn(
                name: "Accept",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.DropColumn(
                name: "AcceptedAt",
                table: "RequestTourGuidePrivateTrips");

            migrationBuilder.RenameTable(
                name: "Review",
                newName: "Reviews");

            migrationBuilder.RenameTable(
                name: "RequestTourGuidePulicTrips",
                newName: "RequestTourGuidePulicTrip");

            migrationBuilder.RenameTable(
                name: "RequestTourGuidePrivateTrips",
                newName: "RequestTourGuidePrivateTrip");

            migrationBuilder.RenameIndex(
                name: "IX_Review_ReviewerId",
                table: "Reviews",
                newName: "IX_Reviews_ReviewerId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_RevieweeId",
                table: "Reviews",
                newName: "IX_Reviews_RevieweeId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_PublicTripId",
                table: "Reviews",
                newName: "IX_Reviews_PublicTripId");

            migrationBuilder.RenameIndex(
                name: "IX_Review_PrivateTripId",
                table: "Reviews",
                newName: "IX_Reviews_PrivateTripId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePulicTrips_TourGuideId",
                table: "RequestTourGuidePulicTrip",
                newName: "IX_RequestTourGuidePulicTrip_TourGuideId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePulicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrip",
                newName: "IX_RequestTourGuidePulicTrip_PublicTripId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePrivateTrips_TourGuideId",
                table: "RequestTourGuidePrivateTrip",
                newName: "IX_RequestTourGuidePrivateTrip_TourGuideId");

            migrationBuilder.RenameIndex(
                name: "IX_RequestTourGuidePrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrip",
                newName: "IX_RequestTourGuidePrivateTrip_PrivateTripId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                table: "Reviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTourGuidePulicTrip",
                table: "RequestTourGuidePulicTrip",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestTourGuidePrivateTrip",
                table: "RequestTourGuidePrivateTrip",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "BookingTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicTripId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    AppProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    TotalBookingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOwnerProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingTrips_PublicTrips_PublicTripId",
                        column: x => x.PublicTripId,
                        principalTable: "PublicTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTrips_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_PublicTripId",
                table: "BookingTrips",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_UserId",
                table: "BookingTrips",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePrivateTrip_PrivateTrips_PrivateTripId",
                table: "RequestTourGuidePrivateTrip",
                column: "PrivateTripId",
                principalTable: "PrivateTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePrivateTrip_TourGuides_TourGuideId",
                table: "RequestTourGuidePrivateTrip",
                column: "TourGuideId",
                principalTable: "TourGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePulicTrip_PublicTrips_PublicTripId",
                table: "RequestTourGuidePulicTrip",
                column: "PublicTripId",
                principalTable: "PublicTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RequestTourGuidePulicTrip_TourGuides_TourGuideId",
                table: "RequestTourGuidePulicTrip",
                column: "TourGuideId",
                principalTable: "TourGuides",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_PrivateTrips_PrivateTripId",
                table: "Reviews",
                column: "PrivateTripId",
                principalTable: "PrivateTrips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_PublicTrips_PublicTripId",
                table: "Reviews",
                column: "PublicTripId",
                principalTable: "PublicTrips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_RevieweeId",
                table: "Reviews",
                column: "RevieweeId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_ReviewerId",
                table: "Reviews",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
