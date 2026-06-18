using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrstructure.Data.Configuration
{

    public class ReqConf : IEntityTypeConfiguration<RequestTourGuidePrivateTrip>
    {
        public void Configure(EntityTypeBuilder<RequestTourGuidePrivateTrip> builder)
        {
            builder
        .HasIndex(r => new { r.PrivateTripId, r.TourGuideId })
        .IsUnique();
        }
    }
    public class ReqPubConf : IEntityTypeConfiguration<RequestTourGuidePulicTrip>
    {
        public void Configure(EntityTypeBuilder<RequestTourGuidePulicTrip> builder)
        {
            builder
        .HasIndex(r => new { r.PublicTripId, r.TourGuideId })
        .IsUnique();
        }
    }


}
