using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstraction
{
    public interface IReadGenericRepo<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        
    }
}
