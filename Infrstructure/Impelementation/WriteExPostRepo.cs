using Domain.Abstraction;
using Domain.Entity.PostEntity;
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
    internal class WriteExPostRepo : WriteGenericRepo<ExperiencePost>,IWriteExPostRepo
    {
        private WriteSysDbContext _context;

        public WriteExPostRepo(WriteSysDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task DeleteAsync(int id)
        {
            try
            {
                // 1. جلب البوست مع الكومنتات (ضروري تعمل Include)
                var item = await _context.Set<ExperiencePost>()
                    .Include(x => x.Comments)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (item != null)
                {
                    // 2. مسح الكومنتات المرتبطة أولاً
                    if (item.Comments != null && item.Comments.Any())
                    {
                        _context.Set<ExperiencePostComment>().RemoveRange(item.Comments);
                    }

                    // 3. مسح البوست نفسه
                    _context.Remove(item);

                    // ملاحظة: الـ SaveChangesAsync غالباً بيتم استدعاؤها في الـ Unit of Work
                }
            }
            catch (Exception ex)
            {
                throw; // حافظ على الـ Stack Trace الأصلي
            }
        }

    }
}
