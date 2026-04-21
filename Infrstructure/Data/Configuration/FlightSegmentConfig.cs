using Domain.Entity.Hotel_flights;
using Domain.Entity.TripEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Data.Configuration
{

    internal class FlightSegmentConfig : IEntityTypeConfiguration<FlightSegment>
    {
        public void Configure(EntityTypeBuilder<FlightSegment> builder)
        {
            builder.ToTable("FlightSegments");
            builder.HasKey(fs => fs.Id);

            builder.Property(fs => fs.DepartureTime).IsRequired();
            builder.Property(fs => fs.ArrivalTime).IsRequired();

            builder.Property(fs => fs.Airline)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(fs => fs.FlightNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.OwnsOne(x => x.DepartureAirport, airport =>
            {
                airport.Property(a => a.Name).HasColumnName("DepartureAirportName");
                airport.Property(a => a.Id).HasColumnName("DepartureAirportCode");
                airport.Property(a => a.Time).HasColumnName("DepartureAirportTime");
            });

            builder.OwnsOne(x => x.ArrivalAirport, airport =>
            {
                airport.Property(a => a.Name).HasColumnName("ArrivalAirportName");
                airport.Property(a => a.Id).HasColumnName("ArrivalAirportCode");
                airport.Property(a => a.Time).HasColumnName("ArrivalAirportTime");
            });
        }
    }

    internal class HotelConfig : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasIndex(x => x.Name);
        }
    }

}
