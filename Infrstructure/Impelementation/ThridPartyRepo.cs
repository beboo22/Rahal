using Application.Abstraction.Specification;
using Domain.Abstraction;
using Domain.Entity.Hotel_flights;
using Infrastructure.Data;
using InfraStructure.Impelementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Impelementation
{
    internal class ReadHotelSearchHistoryRepository
    : ReadGenericRepo<HotelSearchHistory>, IReadHotelSearchHistoryRepository
    {
        private readonly ReadSysDbContext _context;

        public ReadHotelSearchHistoryRepository(ReadSysDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<HotelSearchHistory>> ListAsync(
            ISpecification<HotelSearchHistory> spec,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpec(spec)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(
            ISpecification<HotelSearchHistory> spec,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpec(spec)
                .CountAsync(cancellationToken);
        }
    }
    internal class ReadFlightSearchHistoryRepository
    : ReadGenericRepo<FlightSearchHistory>, IReadFlightSearchHistoryRepository
    {
        private readonly ReadSysDbContext _context;

        public ReadFlightSearchHistoryRepository(ReadSysDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<FlightSearchHistory>> ListAsync(
            ISpecification<FlightSearchHistory> spec,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpec(spec)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(
            ISpecification<FlightSearchHistory> spec,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpec(spec)
                .CountAsync(cancellationToken);
        }
    }

    internal class WriteHotelSearchHistoryRepository
    : WriteGenericRepo<HotelSearchHistory>, IWriteHotelSearchHistoryRepository
    {
        private readonly WriteSysDbContext _context;

        public WriteHotelSearchHistoryRepository(WriteSysDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(
            HotelSearchHistory entity,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<HotelSearchHistory>().AddAsync(entity, cancellationToken);
        }
    }
    internal class WriteFlightSearchHistoryRepository
    : WriteGenericRepo<FlightSearchHistory>, IWriteFlightSearchHistoryRepository
    {
        private readonly WriteSysDbContext _context;

        public WriteFlightSearchHistoryRepository(WriteSysDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task AddAsync(
            FlightSearchHistory entity,
            CancellationToken cancellationToken = default)
        {
            await _context.Set<FlightSearchHistory>().AddAsync(entity, cancellationToken);
        }
    }
}
