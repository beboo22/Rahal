using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Activity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityType",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataId",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "ActivityPublicTrip",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "ActivityPublicTrip",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceRange",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "ActivityPublicTrip",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reviews",
                table: "ActivityPublicTrip",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "serviceOption",
                table: "ActivityPublicTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActivityType",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DataId",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "ActivityPrivateTrip",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "ActivityPrivateTrip",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlaceId",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceRange",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "ActivityPrivateTrip",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Reviews",
                table: "ActivityPrivateTrip",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "serviceOption",
                table: "ActivityPrivateTrip",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "DataId",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "PriceRange",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Reviews",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "serviceOption",
                table: "ActivityPublicTrip");

            migrationBuilder.DropColumn(
                name: "ActivityType",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "DataId",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "PriceRange",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Reviews",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "ActivityPrivateTrip");

            migrationBuilder.DropColumn(
                name: "serviceOption",
                table: "ActivityPrivateTrip");
        }
    }
}
