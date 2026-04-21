using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.Identity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrstructure.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Table name
            builder.ToTable("Users");

            builder.Property(u => u.Isverified)
                   .HasDefaultValue(false);

            // Primary Key
            builder.HasKey(u => u.Id);

            builder.Property(u => u.FinancialBalance)
                .HasColumnType(SQlSyntax.Decimal)
                   .IsRequired(false);

            builder.Property(u => u.FName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.LName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(u => u.Email)
                   .IsRequired();

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            //builder.Property(u => u.PasswordHash)
            //       .IsRequired();

            builder.Property(u => u.IsActive)
                   .HasDefaultValue(true);

            builder.Property(u => u.IsBlocked)
                   .HasDefaultValue(false);


            // Relationships
            builder.HasMany(u => u.Roles)
                   .WithOne(u => u.User)
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RefreshTokens)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(u => u.PasswordResetTokens)
            //       .WithOne(r => r.User)
            //       .HasForeignKey(r => r.UserId)
            //       .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Languages)
                   .WithOne(l => l.User)
                   .HasForeignKey(l => l.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.phoneNumbers)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-one profiles
            builder.HasOne(u => u.TravelerProfile)
                   .WithOne(t => t.User)
                   .HasForeignKey<Traveler>(t => t.UserId);

            builder.HasOne(u => u.TourGuideProfile)
                   .WithOne(tg => tg.User)
                   .HasForeignKey<TourGuide>(tg => tg.UserId);

            builder.HasOne(u => u.TravelerCompanyProfile)
                   .WithOne(tc => tc.User)
                   .HasForeignKey<TravelCompany>(tc => tc.UserId);

            builder.HasOne(u => u.AdminProfile)
                   .WithOne(a => a.User)
                   .HasForeignKey<Admin>(a => a.UserId);

            builder.HasMany(r => r.ReviewsReceived)
                   .WithOne(u => u.Reviewee)
                   .HasForeignKey(r => r.RevieweeId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(r => r.ReviewsWritten)
                   .WithOne(u => u.Reviewer)
                   .HasForeignKey(r => r.ReviewerId)
                   .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(r => r.Posts)
                   .WithOne(u => u.CreatedBy)
                   .HasForeignKey(r => r.CreatedById)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }


    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany(r => r.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
