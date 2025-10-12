using Domain.Entity.Identity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.Abstraction;
using Domain.Entity.TravelerEntity;
using Domain.Entity.TourGuidEntity;
using Domain.Entity.TravelerCompanyEntity;

namespace Infrstructure.Impelementation
{
    internal class ReadUserRepo<T>: IReadUserRepo<T> where T : User
    {
        ReadSysDbContext _context;

        public ReadUserRepo(ReadSysDbContext context)
        {
            _context = context;
        }

        public virtual IQueryable<T> GetAll()
        {
            if (typeof(T) == typeof(Traveler))
                return (IQueryable<T>)_context.Travelers.AsNoTracking();

            if (typeof(T) == typeof(TourGuide))
                return (IQueryable<T>)_context.TourGuides.AsNoTracking();

            if (typeof(T) == typeof(Admin))
                return (IQueryable<T>)_context.Admins.AsNoTracking();

            if (typeof(T) == typeof(TravelCompany))
                return (IQueryable<T>)_context.TravelCompanies.AsNoTracking();

            throw new NotSupportedException($"Type {typeof(T).Name} is not supported by this repository.");
        }


        public virtual async Task<T> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with Id {id} not found.");
            }
            return entity;
        }

    }
}
