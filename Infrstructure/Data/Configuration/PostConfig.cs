using Domain.Entity.PostEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Data.Configuration
{
    internal class PostConfig : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.UseTpcMappingStrategy();

        }
    }internal class PostExConfig : IEntityTypeConfiguration<ExperiencePost>
    {
        public void Configure(EntityTypeBuilder<ExperiencePost> builder)
        {
            builder.Property(ex => ex.Budget).HasColumnType("decimal(18,2)");
        }
    }
}
