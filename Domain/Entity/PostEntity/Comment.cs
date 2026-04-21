using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.PostEntity
{
    public abstract class Comment:BaseEntity
    {       
        public string Msg { get; set; }
        public bool IsEdited { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; }
    }
    public class ExperiencePostComment : Comment
    {
        public int? ExperiencePostId { get; set; }
        public ExperiencePost? ExperiencePost { get; set; }
    }
    public class HiringPostComment : Comment
    {
        public int? HiringPostId { get; set; }
        public HiringPost? HiringPost { get; set; }
    }
}
