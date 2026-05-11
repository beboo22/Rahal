using Domain.Abstraction;
using Domain.Entity.Identity;
using Domain.Entity.Status;
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
    internal class WriteStatusUserRepo : WriteGenericRepo<StatusUser>, IWriteStatusUserRepo
    {
        private WriteSysDbContext _context;

        public WriteStatusUserRepo(WriteSysDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> ExistsAsync(int UserId, int StatusId)
        {
            return await _context.Set<StatusUser>().AnyAsync(x => x.viewById == UserId && x.StatusId == StatusId);
        }

        public override async Task UpdateAsync(StatusUser entity, int Id)
        {
            var item = await _context.Set<StatusUser>().FirstOrDefaultAsync(x=>x.StatusId == entity.StatusId && x.viewById == entity.viewById);

            entity.Id = item.Id;


            try
            {
                // Copy values from incoming entity to tracked entity
                _context.Entry(item).CurrentValues.SetValues(entity);

            }
            catch (DbUpdateException ex)
            {
                // Log exception or rethrow with more context
                throw new Exception("An error occurred while updating the entity.", ex);
            }


        }


    }
}
