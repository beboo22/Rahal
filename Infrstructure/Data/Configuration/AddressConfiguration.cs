using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrstructure.Data.Configuration
{

    public class TrvelerAddressConfiguration : IEntityTypeConfiguration<TrvelerAddress>
    {
        public void Configure(EntityTypeBuilder<TrvelerAddress> builder)
        {
            builder.ToTable("TravelerAddresses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Street).HasMaxLength(200);
            builder.Property(a => a.BuildingNumber).HasMaxLength(50);

            builder.HasOne(a => a.Traveler)
                   .WithMany(t => t.trvelerAddresses)
                   .HasForeignKey(a => a.TravelerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
    public class TourGuideAddressConfiguration : IEntityTypeConfiguration<TourGuideAddress>
    {
        public void Configure(EntityTypeBuilder<TourGuideAddress> builder)
        {
            builder.ToTable("TourGuideAddress");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Street).HasMaxLength(200);
            builder.Property(a => a.BuildingNumber).HasMaxLength(50);

            builder.HasOne(a => a.TourGuide)
                   .WithMany(t => t.tourGuidAddresses)
                   .HasForeignKey(a => a.TourGuideId).OnDelete(DeleteBehavior.Cascade);
        }
    }
    public class TravelerCompanyAddressConfiguration : IEntityTypeConfiguration<TravelerCompanyAddress>
    {
        public void Configure(EntityTypeBuilder<TravelerCompanyAddress> builder)
        {
            builder.ToTable("TravelerCompanyAddress");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
            builder.Property(a => a.City).IsRequired().HasMaxLength(100);
            builder.Property(a => a.Street).HasMaxLength(200);
            builder.Property(a => a.BuildingNumber).HasMaxLength(50);

            builder.HasOne(a => a.TravelCompany)
                   .WithMany(t => t.traveleCompanyAddresses)
                   .HasForeignKey(a => a.TravelCompanyId).OnDelete(DeleteBehavior.Cascade);// to delete address if the company is deleted
        }
    }

}
