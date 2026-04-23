using Domain.Entity;
using Domain.Entity.Amadeus;
using Domain.Entity.Hotel_flights;
using Domain.Entity.Identity;
using Domain.Entity.photo;
using Domain.Entity.PostEntity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TripEntity;
using Infrstructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class ReadSysDbContext : DbContext
    {
        public ReadSysDbContext(DbContextOptions<ReadSysDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //confige all assembly entities
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadSysDbContext).Assembly);
            //modelBuilder.Entity<User>().UseTpcMappingStrategy();

            //// Configure tables
            //modelBuilder.Entity<Traveler>().ToTable("Travelers");
            //modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");
            //modelBuilder.Entity<PasswordResetToken>().ToTable("PasswordResetTokens");
            //modelBuilder.Entity<TourGuide>().ToTable("TourGuides");
            //modelBuilder.Entity<TravelCompany>().ToTable("TravelCompanies");
            //modelBuilder.Entity<Admin>().ToTable("Admins");
            //modelBuilder.Entity<User>().Ignore(u => u.RefreshTokens);
        }
        //Dbsets


        // DbSets
        public DbSet<HotelSearchCache> HotelSearchCaches { get; set; }
        public DbSet<GenericDiscount> GenericDiscounts { get; set; } // ✅ helps querying base type
        public DbSet<SpecificDiscount> SpecificDiscounts { get; set; } // ✅ helps querying base type
        public DbSet<User> Users { get; set; } // ✅ helps querying base type
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<TourGuide> TourGuides { get; set; }
        public DbSet<TravelCompany> TravelCompanies { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PublicTrip> PublicTrips { get; set; }
        public DbSet<PrivateTrip> PrivateTrips { get; set; }
        public DbSet<ActivityPublicTrip> ActivityPublicTrips { get; set; }
        public DbSet<ActivityPrivateTrip> ActivityPrivateTrips { get; set; }
        public DbSet<ReviewPrivateTrip> ReviewPrivateTrips { get; set; }
        public DbSet<ReviewPublicTrip> ReviewPublicTrips { get; set; }

        public DbSet<HiringPost> HiringPosts { get; set; }
        public DbSet<ExperiencePost> ExperiencePosts { get; set; }
        public DbSet<ExperiencePostComment> ExperiencePostComments { get; set; }
        public DbSet<HiringPostComment> HiringPostComments { get; set; }

        public DbSet<BookingPrivateTrip> BookingPrivateTrips { get; set; }
        public DbSet<BookingPublicTrip> BookingPublicTrips { get; set; }


        public DbSet<RequestTourGuidePrivateTrip> RequestTourGuidePrivateTrips { get; set; }
        public DbSet<RequestTourGuidePulicTrip> RequestTourGuidePulicTrips { get; set; }

        public DbSet<PaymentRequest> PaymentRequests { get; set; }



        public DbSet<HotelBrands> HotelBrandss { get; set; }
        public DbSet<RatePerNight> RatePerNights { get; set; }
        public DbSet<HotelLocation> HotelLocations { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelSearchHistory> HotelSearchHistorys { get; set; }

        //public DbSet<PriceInsights> PriceInsightss { get; set; }
        //public DbSet<AirportInfo> AirportInfos { get; set; }
        public DbSet<FlightSegment> FlightSegments { get; set; }
        public DbSet<FlightOffer> FlightOffers { get; set; }
        public DbSet<FlightSearchHistory> FlightSearchHistorys { get; set; }



        public DbSet<PayFlight> PayFlights { get; set; }
        public DbSet<PayHotel> payHotels { get; set; }

        public DbSet<PhotoResultItem> PhotoResultItems { get; set; }
        public DbSet<PhotoSearchResponse> PhotoSearchResponses { get; set; }


    }
}

