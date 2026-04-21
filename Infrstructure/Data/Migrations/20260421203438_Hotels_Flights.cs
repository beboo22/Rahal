using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Hotels_Flights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlockedCounter",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HotelBrandss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelBrandss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelLocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceInsightss",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LowestPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceInsightss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HotelSearchHistorys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SearchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandsId = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelSearchHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HotelSearchHistorys_HotelBrandss_BrandsId",
                        column: x => x.BrandsId,
                        principalTable: "HotelBrandss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlightSearchHistorys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceInsightsId = table.Column<int>(type: "int", nullable: false),
                    SearchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSearchHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightSearchHistorys_PriceInsightss_PriceInsightsId",
                        column: x => x.PriceInsightsId,
                        principalTable: "PriceInsightss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Reviews = table.Column<int>(type: "int", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LowestPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PriceLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Amenities = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NearbyPlaces = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SponsoredHotel = table.Column<bool>(type: "bit", nullable: false),
                    EcoLabel = table.Column<int>(type: "int", nullable: false),
                    HotelSearchHistoryId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_HotelLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "HotelLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hotels_HotelSearchHistorys_HotelSearchHistoryId",
                        column: x => x.HotelSearchHistoryId,
                        principalTable: "HotelSearchHistorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FlightOffers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalDuration = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Layovers = table.Column<int>(type: "int", nullable: false),
                    CarbonEmissions = table.Column<bool>(type: "bit", nullable: false),
                    BookingToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightSearchHistoryId = table.Column<int>(type: "int", nullable: true),
                    FlightSearchHistoryId1 = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightOffers_FlightSearchHistorys_FlightSearchHistoryId",
                        column: x => x.FlightSearchHistoryId,
                        principalTable: "FlightSearchHistorys",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FlightOffers_FlightSearchHistorys_FlightSearchHistoryId1",
                        column: x => x.FlightSearchHistoryId1,
                        principalTable: "FlightSearchHistorys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RatePerNights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lowest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BeforeTaxesFees = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatePerNights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatePerNights_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FlightSegments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureAirportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureAirportCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartureAirportTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartureAirport_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepartureAirport_UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArrivalAirportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalAirportCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ArrivalAirportTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalAirport_CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalAirport_UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Airplane = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Airline = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AirlineLogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravelClass = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlightNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Overnight = table.Column<bool>(type: "bit", nullable: false),
                    LegRoom = table.Column<int>(type: "int", nullable: false),
                    FlightOfferId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlightSegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlightSegments_FlightOffers_FlightOfferId",
                        column: x => x.FlightOfferId,
                        principalTable: "FlightOffers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlightOffers_FlightSearchHistoryId",
                table: "FlightOffers",
                column: "FlightSearchHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightOffers_FlightSearchHistoryId1",
                table: "FlightOffers",
                column: "FlightSearchHistoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSearchHistorys_PriceInsightsId",
                table: "FlightSearchHistorys",
                column: "PriceInsightsId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightSegments_FlightOfferId",
                table: "FlightSegments",
                column: "FlightOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_HotelSearchHistoryId",
                table: "Hotels",
                column: "HotelSearchHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_LocationId",
                table: "Hotels",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_HotelSearchHistorys_BrandsId",
                table: "HotelSearchHistorys",
                column: "BrandsId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePerNights_HotelId",
                table: "RatePerNights",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlightSegments");

            migrationBuilder.DropTable(
                name: "RatePerNights");

            migrationBuilder.DropTable(
                name: "FlightOffers");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "FlightSearchHistorys");

            migrationBuilder.DropTable(
                name: "HotelLocations");

            migrationBuilder.DropTable(
                name: "HotelSearchHistorys");

            migrationBuilder.DropTable(
                name: "PriceInsightss");

            migrationBuilder.DropTable(
                name: "HotelBrandss");

            migrationBuilder.DropColumn(
                name: "BlockedCounter",
                table: "Users");
        }
    }
}
