using Domain.Entity.Identity;
using Domain.Entity.PostEntity;
using Domain.Entity.TravelerCompanyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationBusiness.Fetures.PostService.Query.Response
{
    public class HiringPostTemplate
    {
        public ICollection<TemplateComment> Comments { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Requirements { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
    }
    public class ExperiencePostTemplate
    {
        public ICollection<TemplateComment> Comments { get; set; }

        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string TipsAndRecommendations { get; set; }
        public decimal Budget { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PhotoUrl { get; set; }
    }

  
    public class TemplateComment
    {
        public string Msg { get; set; }
        public bool IsEdited { get; set; } = false;
        public string FullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
