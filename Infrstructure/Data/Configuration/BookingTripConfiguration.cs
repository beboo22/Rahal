using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Data.Configuration
{
    public class BookingPublicTripConfiguration : IEntityTypeConfiguration<BookingPublicTrip>
    {
        public void Configure(EntityTypeBuilder<BookingPublicTrip> builder)
        {
            //builder.ToTable("BookingTrips");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookingDate)
                   .IsRequired();

            

            builder.Property(b => b.TotalBookingPrice)
                   .HasColumnType("decimal(18,2)");
            builder.Property(b => b.TotalOwnerProfit)
                   .HasColumnType("decimal(18,2)");

            builder.Property(b => b.AppProfit)
                   .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(b => b.PublicTrip)
                   .WithMany(t => t.BookingPublicTrips)
                   .HasForeignKey(b => b.PublicTripId)
                   .OnDelete(DeleteBehavior.Cascade); // When a Trip is deleted, its associated BookingTrips are also deleted.

            builder.HasOne(b => b.User)
                   .WithMany(u => u.BookingPublicTrips)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Cascade);// When a User is deleted, their associated BookingTrips are also deleted.
        }
    }
    public class BookingPrivateTripConfiguration : IEntityTypeConfiguration<BookingPrivateTrip>
    {
        public void Configure(EntityTypeBuilder<BookingPrivateTrip> builder)
        {
            //builder.ToTable("BookingTrips");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookingDate)
                   .IsRequired();

            

            builder.Property(b => b.TotalBookingPrice)
                   .HasColumnType("decimal(18,2)");
            builder.Property(b => b.TotalOwnerProfit)
                   .HasColumnType("decimal(18,2)");

            builder.Property(b => b.AppProfit)
                   .HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(b => b.PrivateTrip)
                   .WithMany(t => t.BookingPrivateTrips)
                   .HasForeignKey(b => b.PrivateTripId)
                   .OnDelete(DeleteBehavior.Cascade); // When a Trip is deleted, its associated BookingTrips are also deleted.

            builder.HasOne(b => b.User)
                   .WithMany(u => u.BookingPrivateTrips)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Cascade);// When a User is deleted, their associated BookingTrips are also deleted.
        }
    }

}
