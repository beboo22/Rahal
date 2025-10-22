using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Specification;

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
