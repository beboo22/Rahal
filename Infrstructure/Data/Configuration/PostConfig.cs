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
            builder.Property(x=>x.Title).IsRequired(false);
            builder.Property(x=>x.Description).IsRequired(false);
            builder.Property(x=>x.PhotoUrl).IsRequired(false);
            builder.Property(x=>x.Description).IsRequired(false);
            builder.Property(x=>x.PhotoUrl).IsRequired(false);
            builder.Property(x=>x.City).IsRequired(false);
            builder.Property(x=>x.Country).IsRequired(false);



        }
    }
    internal class PostExConfig : IEntityTypeConfiguration<ExperiencePost>
    {
        public void Configure(EntityTypeBuilder<ExperiencePost> builder)
        {
            //builder.Property(x=>x.Budget).IsRequired(false);
            //builder.Property(x=>x.TipsAndRecommendations).IsRequired(false);
            //builder.Property(ex => ex.Budget).HasColumnType("decimal(18,2)");

            builder.HasMany(x=>x.Comments)
                .WithOne(x=>x.ExperiencePost)
                .HasForeignKey(x=>x.ExperiencePostId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CreatedBy)
                .WithMany(x => x.Posts)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(deleteBehavior: DeleteBehavior.Cascade);

            builder.HasMany(x => x.Likes)
                .WithOne(x => x.post)
                .HasForeignKey(x => x.postId)
                .OnDelete(DeleteBehavior.NoAction);

        }
    }
    internal class PostHirConfig : IEntityTypeConfiguration<HiringPost>
    {
        public void Configure(EntityTypeBuilder<HiringPost> builder)
        {
            //builder.Property(x=>x.Budget).IsRequired(false);
            //builder.Property(x=>x.TipsAndRecommendations).IsRequired(false);
            //builder.Property(ex => ex.Budget).HasColumnType("decimal(18,2)");

            builder.HasMany(x=>x.Comments)
                .WithOne(x=>x.HiringPost)
                .HasForeignKey(x=>x.HiringPostId)
                .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
