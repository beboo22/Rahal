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
    public class BookingTripConfiguration : IEntityTypeConfiguration<BookingTrip>
    {
        public void Configure(EntityTypeBuilder<BookingTrip> builder)
        {
            builder.ToTable("BookingTrips");

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
                   .WithMany(t => t.BookingTrips)
                   .HasForeignKey(b => b.PublicTripId)
                   .OnDelete(DeleteBehavior.Cascade); // When a Trip is deleted, its associated BookingTrips are also deleted.

            builder.HasOne(b => b.User)
                   .WithMany(u => u.BookingTrips)
                   .HasForeignKey(b => b.UserId)
                   .OnDelete(DeleteBehavior.Cascade);// When a User is deleted, their associated BookingTrips are also deleted.
        }
    }

}
