using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Photos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhotoSearchResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoSearchResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhotoResultItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Position = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Original = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalWidth = table.Column<int>(type: "int", nullable: false),
                    OriginalHeight = table.Column<int>(type: "int", nullable: false),
                    PhotoSearchResponseId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoResultItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhotoResultItems_PhotoSearchResponses_PhotoSearchResponseId",
                        column: x => x.PhotoSearchResponseId,
                        principalTable: "PhotoSearchResponses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoResultItems_PhotoSearchResponseId",
                table: "PhotoResultItems",
                column: "PhotoSearchResponseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoResultItems");

            migrationBuilder.DropTable(
                name: "PhotoSearchResponses");
        }
    }
}
