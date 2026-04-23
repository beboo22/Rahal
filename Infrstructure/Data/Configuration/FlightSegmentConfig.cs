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
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Duration)
                   .IsRequired();

            builder.Property(x => x.TravelClass)
                   .HasMaxLength(100);

            builder.Property(x => x.LegRoom);

            builder.Property(x => x.Overnight);

            /*
             * Departure Airport
             */
            builder.OwnsOne(x => x.DepartureAirport, dep =>
            {
                dep.Property(x => x.Code)
                   .HasColumnName("DepartureAirportCode")
                   .HasMaxLength(20);

                dep.Property(x => x.Name)
                   .HasColumnName("DepartureAirportName")
                   .HasMaxLength(250);

                dep.Property(x => x.Time)
                   .HasColumnName("DepartureAirportTime");
            });

            /*
             * Arrival Airport
             */
            builder.OwnsOne(x => x.ArrivalAirport, arr =>
            {
                arr.Property(x => x.Code)
                   .HasColumnName("ArrivalAirportCode")
                   .HasMaxLength(20);

                arr.Property(x => x.Name)
                   .HasColumnName("ArrivalAirportName")
                   .HasMaxLength(250);

                arr.Property(x => x.Time)
                   .HasColumnName("ArrivalAirportTime");
            });

            /*
             * Airline
             */
            //builder.OwnsOne(x => x.Airline, airline =>
            //{
            //    airline.Property(x => x.)
            //           .HasColumnName("AirlineName")
            //           .HasMaxLength(200);

            //    airline.Property(x => x.Logo)
            //           .HasColumnName("AirlineLogo")
            //           .HasMaxLength(500);
            //});

            /*
             * Airplane
             */
            builder.Property(x => x.Airplane)
                   .HasMaxLength(int.MaxValue);

            /*
             * Ignore audit properties if inherited separately
             */
            builder.Navigation(x => x.DepartureAirport).IsRequired();
            builder.Navigation(x => x.ArrivalAirport).IsRequired();
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
