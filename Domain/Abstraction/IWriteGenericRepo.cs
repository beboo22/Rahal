using Domain.Entity;

namespace Domain.Abstraction
{
    public interface IWriteGenericRepo<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        Task AddRangAsync(List<T> entity);
        Task UpdateAsync(T entity, int Id);
        Task UpdateRangeAsync(List<T> entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
