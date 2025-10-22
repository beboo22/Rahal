using Domain.Entity.Identity;
using Domain.Entity.TravelerCompanyEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity.PostEntity
{
    [NotMapped]
    public abstract class Post:BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PhotoUrl { get; set; }
    }
    public class ExperiencePost : Post
    {
        public ICollection<Comment> Comments { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? TipsAndRecommendations { get; set; }
        public decimal? Budget { get; set; }

      
    }
    public class HiringPost : Post
    {
        public ICollection<Comment> Comments { get; set; }
        public int CreatedById { get; set; }
        public TravelCompany CreatedBy { get; set; }
        public string? Requirements { get; set; }
        public string? Status { get; set; }
    }



}
