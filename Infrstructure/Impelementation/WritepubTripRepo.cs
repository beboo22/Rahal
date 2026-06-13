using Domain.Abstraction;
using Domain.Entity.TripEntity;
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
    internal class WritepubTripRepo : WriteGenericRepo<PublicTrip>, IWritepubTripRepo
    {
        WriteSysDbContext _context;

        public WritepubTripRepo(WriteSysDbContext context):base(context)
        {
            _context = context;
        }
        public override async Task DeleteAsync(int id)
        {
            var entity = await _context.PublicTrips
                .Include(x => x.Reviews) // Crucial: This pulls the reviews into EF tracking
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity is null)
                throw new KeyNotFoundException($"Entity with Id {id} not found.");

            try
            {
                // 1. Manually remove the tracked reviews first
                if (entity.Reviews != null && entity.Reviews.Any())
                {
                    _context.RemoveRange(entity.Reviews);
                }

                // 2. Now it is safe to remove the trip
                _context.PublicTrips.Remove(entity);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the entity.", ex);
            }
        }
    }
}
