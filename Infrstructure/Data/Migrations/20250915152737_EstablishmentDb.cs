using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrstructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EstablishmentDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "TripSequence");

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    BlockedEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BlockedStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FinancialBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LastPasswordResetTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLogoutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Languages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HasWhatsApp = table.Column<bool>(type: "bit", nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhoneNumber_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrivateTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [TripSequence]"),
                    From = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TripCategory = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: true),
                    CustomizationFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateTrips_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateTrips_Users_TourGuideId",
                        column: x => x.TourGuideId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PublicTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [TripSequence]"),
                    From = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TripCategory = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    IncludedPackages = table.Column<int>(type: "int", nullable: false),
                    MaxNumberOfMember = table.Column<int>(type: "int", nullable: false),
                    CurrentNumberOfMember = table.Column<int>(type: "int", nullable: false),
                    TravelerFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OwnerTripFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicTrips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PublicTrips_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublicTrips_Users_TourGuideId",
                        column: x => x.TourGuideId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Revoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourGuides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SalaryPerDay = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Isverified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FrontIdentityPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackIdentityPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Ssn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourGuides_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelCompanies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Isverified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FrontIdentityPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackIdentityPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ssn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelCompanies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelCompanies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Travelers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Isverified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FrontIdentityPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackIdentityPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ssn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Travelers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Travelers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "userRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPrivateTrip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectedDay = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TripCategory = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    PrivateTripId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPrivateTrip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPrivateTrip_PrivateTrips_PrivateTripId",
                        column: x => x.PrivateTripId,
                        principalTable: "PrivateTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityPublicTrip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Destination = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SelectedDay = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TripCategory = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    PublicTripId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityPublicTrip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityPublicTrip_PublicTrips_PublicTripId",
                        column: x => x.PublicTripId,
                        principalTable: "PublicTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingTrips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicTripId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalBookingPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalOwnerProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AppProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "RequestTourGuidePrivateTrip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrivateTripId = table.Column<int>(type: "int", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTourGuidePrivateTrip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestTourGuidePrivateTrip_PrivateTrips_PrivateTripId",
                        column: x => x.PrivateTripId,
                        principalTable: "PrivateTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestTourGuidePrivateTrip_TourGuides_TourGuideId",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestTourGuidePulicTrip",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicTripId = table.Column<int>(type: "int", nullable: false),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTourGuidePulicTrip", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestTourGuidePulicTrip_PublicTrips_PublicTripId",
                        column: x => x.PublicTripId,
                        principalTable: "PublicTrips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestTourGuidePulicTrip_TourGuides_TourGuideId",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourGuideAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourGuideId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuideAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourGuideAddress_TourGuides_TourGuideId",
                        column: x => x.TourGuideId,
                        principalTable: "TourGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourGuideBusinessGalleries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TourGuidId = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourGuideBusinessGalleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourGuideBusinessGalleries_TourGuides_TourGuidId",
                        column: x => x.TourGuidId,
                        principalTable: "TourGuides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelCompanyBusinessGalary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelCompanyId = table.Column<int>(type: "int", nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelCompanyBusinessGalary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelCompanyBusinessGalary_TravelCompanies_TravelCompanyId",
                        column: x => x.TravelCompanyId,
                        principalTable: "TravelCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerCompanyAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelCompanyId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerCompanyAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerCompanyAddress_TravelCompanies_TravelCompanyId",
                        column: x => x.TravelCompanyId,
                        principalTable: "TravelCompanies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TravelerAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TravelerId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BuildingNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TravelerAddresses_Travelers_TravelerId",
                        column: x => x.TravelerId,
                        principalTable: "Travelers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPrivateTrip_PrivateTripId",
                table: "ActivityPrivateTrip",
                column: "PrivateTripId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityPublicTrip_PublicTripId",
                table: "ActivityPublicTrip",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_PublicTripId",
                table: "BookingTrips",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTrips_UserId",
                table: "BookingTrips",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UserId",
                table: "Languages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PhoneNumber_UserId",
                table: "PhoneNumber",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateTrips_CreatedById",
                table: "PrivateTrips",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateTrips_TourGuideId",
                table: "PrivateTrips",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicTrips_CreatedById",
                table: "PublicTrips",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_PublicTrips_TourGuideId",
                table: "PublicTrips",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePrivateTrip_PrivateTripId",
                table: "RequestTourGuidePrivateTrip",
                column: "PrivateTripId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePrivateTrip_TourGuideId",
                table: "RequestTourGuidePrivateTrip",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePulicTrip_PublicTripId",
                table: "RequestTourGuidePulicTrip",
                column: "PublicTripId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTourGuidePulicTrip_TourGuideId",
                table: "RequestTourGuidePulicTrip",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideAddress_TourGuideId",
                table: "TourGuideAddress",
                column: "TourGuideId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuideBusinessGalleries_TourGuidId",
                table: "TourGuideBusinessGalleries",
                column: "TourGuidId");

            migrationBuilder.CreateIndex(
                name: "IX_TourGuides_UserId",
                table: "TourGuides",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelCompanies_UserId",
                table: "TravelCompanies",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TravelCompanyBusinessGalary_TravelCompanyId",
                table: "TravelCompanyBusinessGalary",
                column: "TravelCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelerAddresses_TravelerId",
                table: "TravelerAddresses",
                column: "TravelerId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelerCompanyAddress_TravelCompanyId",
                table: "TravelerCompanyAddress",
                column: "TravelCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Travelers_UserId",
                table: "Travelers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_userRoles_RoleId",
                table: "userRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_userRoles_UserId",
                table: "userRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityPrivateTrip");

            migrationBuilder.DropTable(
                name: "ActivityPublicTrip");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "BookingTrips");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "PhoneNumber");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RequestTourGuidePrivateTrip");

            migrationBuilder.DropTable(
                name: "RequestTourGuidePulicTrip");

            migrationBuilder.DropTable(
                name: "TourGuideAddress");

            migrationBuilder.DropTable(
                name: "TourGuideBusinessGalleries");

            migrationBuilder.DropTable(
                name: "TravelCompanyBusinessGalary");

            migrationBuilder.DropTable(
                name: "TravelerAddresses");

            migrationBuilder.DropTable(
                name: "TravelerCompanyAddress");

            migrationBuilder.DropTable(
                name: "userRoles");

            migrationBuilder.DropTable(
                name: "PrivateTrips");

            migrationBuilder.DropTable(
                name: "PublicTrips");

            migrationBuilder.DropTable(
                name: "TourGuides");

            migrationBuilder.DropTable(
                name: "Travelers");

            migrationBuilder.DropTable(
                name: "TravelCompanies");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropSequence(
                name: "TripSequence");
        }
    }
}
