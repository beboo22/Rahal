using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TourGuidEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrstructure.Data.Configuration
{

    public class TourGuideConfiguration : IEntityTypeConfiguration<TourGuide>
    {
        public void Configure(EntityTypeBuilder<TourGuide> builder)
        {
            builder.ToTable("TourGuides");

            builder.HasKey(tg => tg.Id);
            builder.Property(tg => tg.Id)
                   .ValueGeneratedNever();

            // Properties
            builder.Property(u => u.Ssn)
                   .IsRequired(false);

            builder.Property(u => u.Bio)
                   .IsRequired(false)
                   .HasMaxLength(1000);
            builder.Property(u => u.SalaryPerDay)
                     .HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(tg => tg.User)
                   .WithOne(u => u.TourGuideProfile)
                   .HasForeignKey<TourGuide>(tg => tg.UserId).OnDelete(DeleteBehavior.Cascade);// to ensure that when a User is deleted, the associated TourGuide is also deleted.

            builder.HasMany(tg => tg.tourGuidAddresses)
                   .WithOne(a => a.TourGuide)
                   .HasForeignKey(a => a.TourGuideId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tg => tg.tourGuidBusinessGalaries)
                   .WithOne(bg => bg.TourGuid)
                   .HasForeignKey(bg => bg.TourGuidId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
