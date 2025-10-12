using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TravelerEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrstructure.Data.Configuration
{

    public class TravelerConfiguration : IEntityTypeConfiguration<Traveler>
    {
        public void Configure(EntityTypeBuilder<Traveler> builder)
        {
            builder.ToTable("Travelers");

            builder.HasKey(t => t.Id);
            builder.Property(tg => tg.Id)
                   .ValueGeneratedNever();

            // Properties
            builder.Property(u => u.Ssn)
                   .IsRequired(false);



            builder.HasOne(t => t.User)
                   .WithOne(u => u.TravelerProfile)
                   .HasForeignKey<Traveler>(t => t.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.trvelerAddresses)
                   .WithOne(a => a.Traveler)
                   .HasForeignKey(a => a.TravelerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
