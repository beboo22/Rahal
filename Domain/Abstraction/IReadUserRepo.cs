using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstraction
{
    public interface IReadUserRepo<T>where T : User
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();

    }
}
