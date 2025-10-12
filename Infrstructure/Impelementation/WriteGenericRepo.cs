using Domain.Abstraction;
using Domain.Entity;
using Domain.Entity.PostEntity;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InfraStructure.Impelementation
{
    internal class WriteGenericRepo<T> : IWriteGenericRepo<T> where T : BaseEntity
    {
        WriteSysDbContext _context;

        public WriteGenericRepo(WriteSysDbContext context)
        {
            _context = context;
        }

        public async virtual Task AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }
            try
            {

                await _context.Set<T>().AddAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while adding the entity.", ex);
            }
        }

        public async Task AddRangAsync(List<T> entity)
        {
            if (!entity.Any())
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }
            try
            {

                await _context.Set<T>().AddRangeAsync(entity);
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while adding the entity.", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            T entity = typeof(T) switch
            {
                var t when t == typeof(HiringPost) =>
                    (T)(object)await _context.Set<HiringPost>()
                        .Include(p => p.Comments)
                        .FirstOrDefaultAsync(p => p.Id == id),

                var t when t == typeof(ExperiencePost) =>
                    (T)(object)await _context.Set<ExperiencePost>()
                        .Include(p => p.Comments)
                        .FirstOrDefaultAsync(p => p.Id == id),

                _ => await _context.Set<T>().FindAsync(id)
            };

            if (entity == null)
                throw new KeyNotFoundException($"Entity with Id {id} not found.");

            try
            {
                _context.Set<T>().Remove(entity);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the entity.", ex);
            }
        }


        public virtual async Task<bool> ExistsAsync(int id)
        {
            var exists = await _context.Set<T>().AsNoTracking().AnyAsync(e => e.Id == id);
            return exists;
        }
        public virtual async Task UpdateAsync(T entity, int Id)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");

            var existingEntity = await _context.Set<T>().FindAsync(Id);
            if (existingEntity == null)
                throw new KeyNotFoundException($"Entity with Id {Id} not found.");
            //_context.UpdateRange(existingEntity);
            try
            {
                // Copy values from incoming entity to tracked entity
                _context.Entry(existingEntity).CurrentValues.SetValues(entity);

                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log exception or rethrow with more context
                throw new Exception("An error occurred while updating the entity.", ex);
            }
        }

        public async Task UpdateRangeAsync(List<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null or empty");

            try
            {
                _context.UpdateRange(entities);
                //await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"An error occurred while updating multiple entities of type {typeof(T).Name}.", ex);
            }
        }
    }
}
