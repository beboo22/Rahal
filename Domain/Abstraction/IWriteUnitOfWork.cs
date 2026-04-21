using Domain.Entity;

namespace Domain.Abstraction
{
    public interface IWriteUnitOfWork:IDisposable
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
    }
}
