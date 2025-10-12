using Domain.Entity;

namespace Domain.Abstraction
{
    public interface IWriteUnitOfWork:IDisposable
    {
        Task BeginTransiaction();
        //Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
    }
}
