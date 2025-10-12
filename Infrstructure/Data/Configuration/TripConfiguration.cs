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
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.UseTpcMappingStrategy();

            //builder.ToTable("Trips");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.From)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.Destination)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(t => t.Price)
                   .HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(t => t.TripCategory)
                   .HasConversion<int>();

            builder.Property(t => t.StartDate)
                   .IsRequired();
            builder.HasOne(t => t.CreatedBy)
                   .WithMany(u => u.CreatedTrips)
                   .HasForeignKey(t => t.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict); // When a User is deleted, do not delete their created Trips.
       
        }
    }
    
    public class PublicTripConfiguration : IEntityTypeConfiguration<PublicTrip>
    {
        public void Configure(EntityTypeBuilder<PublicTrip> builder)
        {
            builder.HasMany(t=>t.requestTourGuides)
                   .WithOne(r => r.PublicTrip)
                   .HasForeignKey(r => r.PublicTripId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.Property(b => b.TravelerFee)
                   .HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(b => b.OwnerTripFee)
                   .HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(t => t.IncludedPackages)
                   .HasConversion<int>();
            
            builder.HasOne(p=>p.TourGuide)
                .WithMany()
                .HasForeignKey(p => p.TourGuideId).OnDelete(DeleteBehavior.Restrict); // When a TourGuide is deleted, do not delete their Public Trips.

        }
    }

    public class PrivateTripConfiguration : IEntityTypeConfiguration<PrivateTrip>
    {
        public void Configure(EntityTypeBuilder<PrivateTrip> builder)
        {
            builder.Property(b => b.CustomizationFee)
                .HasColumnType("decimal(18,2)")
                   .IsRequired();
            builder.HasMany(t=>t.requestTourGuides)
                   .WithOne(r => r.PrivateTrip)
                   .HasForeignKey(r => r.PrivateTripId)
                   .OnDelete(DeleteBehavior.Cascade); // When a Trip is deleted, delete associated RequestTourGuides.
            builder.HasOne(p=>p.TourGuide)
                .WithMany()
                .HasForeignKey(p => p.TourGuideId).OnDelete(DeleteBehavior.Restrict); // When a TourGuide is deleted, do not delete their Public Trips.
            //builder.ha
        }
    }
}
