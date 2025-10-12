using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TripEntity;

namespace Infrstructure.Data.Configuration
{
    public class PrivateActivityConfiguration : IEntityTypeConfiguration<ActivityPrivateTrip>
    {
        public void Configure(EntityTypeBuilder<ActivityPrivateTrip> builder)
        {
            builder.ToTable("ActivityPrivateTrip");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Destination)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.FullPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.SelectedDay)
                   .IsRequired();

            builder.Property(a => a.Image)
                   .HasMaxLength(500);

            builder.Property(a => a.TripCategory)
                   .HasConversion<int>();

            builder.Property(a => a.StartAt)
                   .IsRequired();

            builder.Property(a => a.EndAt)
                   .IsRequired();

            builder.HasOne(a => a.PrivateTrip)
                   .WithMany(t => t.PrivateActivities)
                   .HasForeignKey(a => a.PrivateTripId)
                   .OnDelete(DeleteBehavior.Cascade); // When a Trip is deleted, its associated Activities are also deleted.
        }
    }
    
    public class PublicActivityConfiguration : IEntityTypeConfiguration<ActivityPublicTrip>
    {
        public void Configure(EntityTypeBuilder<ActivityPublicTrip> builder)
        {
            builder.ToTable("ActivityPublicTrip");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Destination)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.FullPrice)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.SelectedDay)
                   .IsRequired();

            builder.Property(a => a.Image)
                   .HasMaxLength(500);

            builder.Property(a => a.TripCategory)
                   .HasConversion<int>();

            builder.Property(a => a.StartAt)
                   .IsRequired();

            builder.Property(a => a.EndAt)
                   .IsRequired();

            builder.HasOne(a => a.PublicTrip)
                   .WithMany(t => t.PublicActivities)
                   .HasForeignKey(a => a.PublicTripId)
                   .OnDelete(DeleteBehavior.Cascade); // When a Trip is deleted, its associated Activities are also deleted.
        }
    }




}
