using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Data.Configuration
{
    internal class DiscountConfig : IEntityTypeConfiguration<GenericDiscount>
    {
        public void Configure(EntityTypeBuilder<GenericDiscount> builder)
        {
            builder.HasIndex(x=>x.Code).IsUnique();

            builder.Property(b => b.DiscountValue)
                   .HasColumnType("decimal(18,2)");
        }
    }
    internal class SpecDiscountConfig : IEntityTypeConfiguration<SpecificDiscount>
    {
        public void Configure(EntityTypeBuilder<SpecificDiscount> builder)
        {
            //builder.Property(x=>x.Code).i;
            builder.HasIndex(x=>x.Code).IsUnique();

            builder.Property(b => b.DiscountValue)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
