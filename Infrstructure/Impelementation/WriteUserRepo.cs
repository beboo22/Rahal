using Domain.Abstraction;
using Domain.Entity;
using Domain.Entity.Identity;
using Infrastructure.Data;
using InfraStructure.Impelementation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrstructure.Impelementation
{
    internal class WriteUserRepo: WriteGenericRepo<User>,IWriteUserRepo 
    {
        WriteSysDbContext _context;

        public WriteUserRepo(WriteSysDbContext context):base(context) 
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string Email)
        {
            var exists = await _context.Set<User>().AsNoTracking().AnyAsync(e => e.Email == Email);
            return exists;
        }


    }
}
