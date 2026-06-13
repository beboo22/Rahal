using Domain.Entity.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.PostEntity
{
    public class Likes:BaseEntity
    {

        public int postId { get; set; }
        public ExperiencePost post { get; set; }

        public LikeType LikeType { get; set; }

        public  int UserId { get; set; }
        public  User User { get; set; }
    }

    public enum LikeType
    {
        love=0,
        hahaha,
        sad,
        angry,
    }
}
