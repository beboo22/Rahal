using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommentAndPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperiencePost_Users_CreatedById",
                table: "ExperiencePost");

            migrationBuilder.DropForeignKey(
                name: "FK_HiringPost_TravelCompanies_CreatedById",
                table: "HiringPost");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HiringPost",
                table: "HiringPost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperiencePost",
                table: "ExperiencePost");

            migrationBuilder.RenameTable(
                name: "HiringPost",
                newName: "HiringPosts");

            migrationBuilder.RenameTable(
                name: "ExperiencePost",
                newName: "ExperiencePosts");

            migrationBuilder.RenameIndex(
                name: "IX_HiringPost_CreatedById",
                table: "HiringPosts",
                newName: "IX_HiringPosts_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ExperiencePost_CreatedById",
                table: "ExperiencePosts",
                newName: "IX_ExperiencePosts_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HiringPosts",
                table: "HiringPosts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperiencePosts",
                table: "ExperiencePosts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ExperiencePostComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperiencePostId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Msg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperiencePostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExperiencePostComments_ExperiencePosts_ExperiencePostId",
                        column: x => x.ExperiencePostId,
                        principalTable: "ExperiencePosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExperiencePostComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HiringPostComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HiringPostId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Msg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiringPostComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiringPostComments_HiringPosts_HiringPostId",
                        column: x => x.HiringPostId,
                        principalTable: "HiringPosts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HiringPostComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExperiencePostComments_ExperiencePostId",
                table: "ExperiencePostComments",
                column: "ExperiencePostId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperiencePostComments_UserId",
                table: "ExperiencePostComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringPostComments_HiringPostId",
                table: "HiringPostComments",
                column: "HiringPostId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringPostComments_UserId",
                table: "HiringPostComments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperiencePosts_Users_CreatedById",
                table: "ExperiencePosts",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HiringPosts_TravelCompanies_CreatedById",
                table: "HiringPosts",
                column: "CreatedById",
                principalTable: "TravelCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperiencePosts_Users_CreatedById",
                table: "ExperiencePosts");

            migrationBuilder.DropForeignKey(
                name: "FK_HiringPosts_TravelCompanies_CreatedById",
                table: "HiringPosts");

            migrationBuilder.DropTable(
                name: "ExperiencePostComments");

            migrationBuilder.DropTable(
                name: "HiringPostComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HiringPosts",
                table: "HiringPosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExperiencePosts",
                table: "ExperiencePosts");

            migrationBuilder.RenameTable(
                name: "HiringPosts",
                newName: "HiringPost");

            migrationBuilder.RenameTable(
                name: "ExperiencePosts",
                newName: "ExperiencePost");

            migrationBuilder.RenameIndex(
                name: "IX_HiringPosts_CreatedById",
                table: "HiringPost",
                newName: "IX_HiringPost_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ExperiencePosts_CreatedById",
                table: "ExperiencePost",
                newName: "IX_ExperiencePost_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HiringPost",
                table: "HiringPost",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExperiencePost",
                table: "ExperiencePost",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExperiencePostId = table.Column<int>(type: "int", nullable: true),
                    HiringPostId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEdited = table.Column<bool>(type: "bit", nullable: false),
                    Msg = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.AddForeignKey(
                name: "FK_ExperiencePost_Users_CreatedById",
                table: "ExperiencePost",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HiringPost_TravelCompanies_CreatedById",
                table: "HiringPost",
                column: "CreatedById",
                principalTable: "TravelCompanies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
