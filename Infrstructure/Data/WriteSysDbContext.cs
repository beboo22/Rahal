using Domain.Entity.Identity;
using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TripEntity;
using Domain.Entity.PostEntity;

namespace Infrastructure.Data
{

    public class WriteSysDbContext : DbContext
    {
        public WriteSysDbContext(DbContextOptions<WriteSysDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadSysDbContext).Assembly);

            // Configure base type with TPC
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

        // DbSets
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
        public DbSet<BookingTrip> BookingTrips { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

    }


}
