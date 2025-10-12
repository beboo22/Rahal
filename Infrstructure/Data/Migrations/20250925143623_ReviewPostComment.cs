using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ReviewPostComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "PostSequence");

            migrationBuilder.CreateTable(
                name: "ExperiencePost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [PostSequence]"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipsAndRecommendations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperiencePost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperiencePost_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HiringPost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [PostSequence]"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiringPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiringPost_TravelCompanies_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "TravelCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicTripId = table.Column<int>(type: "int", nullable: true),
                    PrivateTripId = table.Column<int>(type: "int", nullable: true),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    RevieweeId = table.Column<int>(type: "int", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_PrivateTrips_PrivateTripId",
                        column: x => x.PrivateTripId,
                        principalTable: "PrivateTrips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_PublicTrips_PublicTripId",
                        column: x => x.PublicTripId,
                        principalTable: "PublicTrips",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reviews_Users_RevieweeId",
                        column: x => x.RevieweeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperiencePostId = table.Column<int>(type: "int", nullable: true),
                    HiringPostId = table.Column<int>(type: "int", nullable: true),
                    Msg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_ExperiencePost_ExperiencePostId",
                        column: x => x.ExperiencePostId,
                        principalTable: "ExperiencePost",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_HiringPost_HiringPostId",
                        column: x => x.HiringPostId,
                        principalTable: "HiringPost",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ExperiencePostId",
                table: "Comments",
                column: "ExperiencePostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_HiringPostId",
                table: "Comments",
                column: "HiringPostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperiencePost_CreatedById",
                table: "ExperiencePost",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_HiringPost_CreatedById",
                table: "HiringPost",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PrivateTripId",
                table: "Reviews",
                column: "PrivateTripId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PublicTripId",
                table: "Reviews",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_RevieweeId",
                table: "Reviews",
                column: "RevieweeId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewerId",
                table: "Reviews",
                column: "ReviewerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "ExperiencePost");

            migrationBuilder.DropTable(
                name: "HiringPost");

            migrationBuilder.DropSequence(
                name: "PostSequence");
        }
    }
}
