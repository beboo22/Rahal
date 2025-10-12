using Domain.Entity;
using Domain.Abstraction;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Impelementation
{
    public class ReadGenericRepo<T> : IReadGenericRepo<T> where T : BaseEntity
    {
        ReadSysDbContext _context;

        public ReadGenericRepo(ReadSysDbContext context)
        {
            _context = context;
        }

        public virtual IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            var entity = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(t=>t.Id == id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with Id {id} not found.");
            }
            return entity;
        }

    }
}
