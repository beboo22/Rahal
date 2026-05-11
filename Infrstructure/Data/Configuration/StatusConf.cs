using Domain.Entity.Status;
using Domain.Entity.TourGuidEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Data.Configuration
{
    internal class StatusConf : IEntityTypeConfiguration<Status>
    {
        public void Configure(EntityTypeBuilder<Status> builder)
        {
            builder.HasMany(x=>x.StatusUsers)
                .WithOne(x=>x.Status)
                .HasForeignKey(x=>x.StatusId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
