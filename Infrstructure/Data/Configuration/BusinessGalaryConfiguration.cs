using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrstructure.Data.Configuration
{

    public class TourGuideBusinessGalaryConfiguration : IEntityTypeConfiguration<TourGuideBusinessGalary>
    {
        public void Configure(EntityTypeBuilder<TourGuideBusinessGalary> builder)
        {
            builder.ToTable("TourGuideBusinessGalleries");

            builder.HasKey(bg => bg.Id);

            builder.Property(bg => bg.PhotoUrl).IsRequired();
            builder.Property(bg => bg.Date).IsRequired();
            builder.Property(bg => bg.Location).HasMaxLength(200);
            builder.Property(bg => bg.Description).HasMaxLength(500);

            builder.HasOne(bg => bg.TourGuid)
                   .WithMany(tg => tg.tourGuidBusinessGalaries)
                   .HasForeignKey(bg => bg.TourGuidId).OnDelete(DeleteBehavior.Cascade);
        }
    }
    public class TravelCompanyBusinessGalaryConfiguration : IEntityTypeConfiguration<TravelCompanyBusinessGalary>
    {
        public void Configure(EntityTypeBuilder<TravelCompanyBusinessGalary> builder)
        {
            builder.ToTable("TravelCompanyBusinessGalary");

            builder.HasKey(bg => bg.Id);

            builder.Property(bg => bg.PhotoUrl).IsRequired();
            builder.Property(bg => bg.Date).IsRequired();
            builder.Property(bg => bg.Location).HasMaxLength(200);
            builder.Property(bg => bg.Description).HasMaxLength(500);

            builder.HasOne(bg => bg.TravelCompany)
                   .WithMany(tg => tg.travelCompanyBusinessGalaries)
                   .HasForeignKey(bg => bg.TravelCompanyId).OnDelete(DeleteBehavior.Cascade);
        }
    }

}
