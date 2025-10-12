using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TravelerCompanyEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Infrstructure.Data.Configuration
{
    public class TravelCompanyConfiguration : IEntityTypeConfiguration<TravelCompany>
    {
        public void Configure(EntityTypeBuilder<TravelCompany> builder)
        {
            builder.ToTable("TravelCompanies");

            builder.HasKey(tc => tc.Id);
            builder.Property(tg => tg.Id)
                   .ValueGeneratedNever();

            // Properties
            builder.Property(u => u.Ssn)
                   .IsRequired(false);




            builder.HasOne(tc => tc.User)
                   .WithOne(u => u.TravelerCompanyProfile)
                   .HasForeignKey<TravelCompany>(tc => tc.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.travelCompanyBusinessGalaries)
                   .WithOne(a => a.TravelCompany)
                   .HasForeignKey(a => a.TravelCompanyId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.travelCompanyBusinessGalaries)
                   .WithOne(bg => bg.TravelCompany)
                   .HasForeignKey(bg => bg.TravelCompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(tc => tc.HiringPosts)
                   .WithOne(bg => bg.CreatedBy)
                   .HasForeignKey(bg => bg.CreatedById)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
