using Domain.Entity;
using Domain.Abstraction;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Abstraction.Specification;

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


        public IQueryable<T> GetAllSpec(ISpecification<T> spec)
        {
            var items =  ApplySpec(spec).AsNoTracking();
            return items;
        }
        public async Task<T> GetByIDSpec(ISpecification<T> spec)
        {
            var items = await ApplySpec(spec).FirstOrDefaultAsync();
            if (items == null)
            {
                throw new KeyNotFoundException($"Entity not found.");
            }
            return items;
        }
        protected IQueryable<T> ApplySpec(ISpecification<T> spec)
        => SpecificationEvaluation<T>.GetQuery(_context.Set<T>(), spec);







    }
}
