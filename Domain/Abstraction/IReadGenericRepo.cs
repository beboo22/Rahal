using Application.Abstraction.Specification;
using Domain.Entity;

namespace Domain.Abstraction
{
    public interface IReadGenericRepo<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();

        public Task<T> GetByIDSpec(ISpecification<T> spec);
        public IQueryable<T> GetAllSpec(ISpecification<T> spec);


    }
}
